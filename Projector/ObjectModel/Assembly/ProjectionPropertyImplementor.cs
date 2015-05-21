namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class ProjectionPropertyFactory
    {
        private readonly TypeBuilder typeBuilder;

        private int cachedPropertyCount;

        private string     name;
        private Type       type;
        private int        index;
        private int        flagsBit;
        private string     memberPrefix;
        private FieldInfo  flagsField;
        private FieldInfo  propertyField;
        private FieldInfo  valueField;
        private MethodInfo encacheMethod;
        private MethodInfo decacheMethod;

        public ProjectionPropertyFactory(TypeBuilder typeBuilder)
        {
            this.typeBuilder = typeBuilder;
        }

        private struct ProjectionPropertyInfo
        {
            public ProjectionProperty Property;
            public string             PropertyFieldName;
        }

        private ProjectionPropertyInfo[] ImplementProjectionProperties(TypeBuilder typeBuilder, ProjectionPropertyCollection properties)
        {
            var infos = new ProjectionPropertyInfo[properties.Count];

            foreach (var property in properties)
            {
                Prepare(index, property);
                ImplementProperty();

                infos[index++] = new ProjectionPropertyInfo
                {
                    Property          = property,
                    PropertyFieldName = GetMemberName("Property")
                };
            }

            return infos;
        }

        private void Prepare(int index, ProjectionProperty property)
        {
            name         = property.Name;
            type         = property.PropertyType.UnderlyingType;
            memberPrefix = GetMemberPrefix(index, name);

            if (!property.IsVolatile)
                AssignFlagsBit();
        }

        // Property 'value cached' flags are stored in packed Int32 bit fields.
        // Each property maps to a particular field/bit, depending on the property's index.

        private void AssignFlagsBit()
        {
            var index = cachedPropertyCount++;
            flagsBit  = GetFlagsBit(index);

            if (flagsBit != 0)
                return; // Reuse flagsField

            flagsField = typeBuilder.DefineField
            (
                GetFlagsFieldName(index),
                typeof(Int32),
                FieldAttributes.Private
            );
        }

        private static int GetFlagsIndex(int propertyIndex)
        {
            return propertyIndex >> 5; // i.e. log2(BitsPerInt32)
        }

        private static int GetFlagsBit(int propertyIndex)
        {
            return propertyIndex & (BitsPerInt32 - 1);
        }

        public void ImplementProperty()
        {
            propertyField = typeBuilder.DefineField
            (
                GetMemberName("Property"),
                typeof(ProjectionProperty<>).MakeGenericType(type),
                FieldAttributes.Public | FieldAttributes.Static
            );

            valueField = typeBuilder.DefineField
            (
                GetMemberName("Value"),
                type,
                FieldAttributes.Private
            );

            ImplementAccessorClass();

            var property = typeBuilder.DefineProperty
            (
                name,
                PropertyAttributes.None,
                CallingConventions.HasThis,
                type,
                null // parameterTypes
            );

                            ImplementIsCachedMethod();
            encacheMethod = ImplementEncacheMethod();
            decacheMethod = ImplementDecacheMethod();

            //if (propertyInfo.CanRead)
            //{
            //    var getMethod = ImplementGetMethod(typeBuilder, index, propertyInfo, propertyField, valueField, flagsField);
            //    property.SetGetMethod(getMethod);
            //}

            //if (propertyInfo.CanWrite)
            //{
            //    var setMethod = ImplementSetMethod(typeBuilder, index, propertyInfo, propertyField, valueField, flagsField);
            //    property.SetSetMethod(setMethod);
            //}
        }

        private void ImplementAccessorClass()
        {
            var accessor = typeBuilder.DefineNestedType
            (
                GetMemberName("Accessor"),
                TypeAttributes.NestedPrivate | TypeAttributes.Class | TypeAttributes.Sealed,
                typeof(DirectPropertyAccessor<,>).MakeGenericType(typeBuilder, type)
            );

            ImplementAccessorConstructor(accessor);
            ImplementAccessorTryGetCachedMethod(accessor);
            ImplementAccessorEncacheMethod(accessor);
            ImplementAccessorDecacheMethod(accessor);
        }

        private void ImplementAccessorConstructor(TypeBuilder accessor)
        {
            var constructor = accessor.DefineConstructor
            (
                MethodAttributes.Private | MethodAttributes.HideBySig,
                CallingConventions.Standard,
                new[] { typeof(ProjectionProperty<>).MakeGenericType(type) }
            );

            var baseConstructor = typeof(DirectPropertyAccessor<,>).MakeGenericType(typeBuilder, type).GetConstructor
            (
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, // binder
                new[] { typeof(ProjectionProperty<>).MakeGenericType(type) },
                null // modifiers
            );

            constructor.DefineParameter(1, ParameterAttributes.None, "property");

            var il = constructor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, baseConstructor);
            il.Emit(OpCodes.Ret);
        }

        private void ImplementAccessorTryGetCachedMethod(TypeBuilder accessor)
        {
            var method = accessor.DefineMethod
            (
                "TryGetCached",
                MethodAttributes.Virtual,
                CallingConventions.HasThis,
                typeof(bool),
                new[] { typeBuilder, type.MakeByRefType() }
            );

            method.DefineParameter(1, ParameterAttributes.In,  "target");
            method.DefineParameter(2, ParameterAttributes.Out, "value" );

            var baseMethod = typeof(DirectPropertyAccessor<,>)
                .MakeGenericType(typeBuilder, type)
                .GetMethod("TryGetCached", BindingFlags.NonPublic | BindingFlags.Instance);

            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, baseMethod);
            il.Emit(OpCodes.Ret);
        }

        private void ImplementAccessorEncacheMethod(TypeBuilder accessor)
        {
            throw new NotImplementedException();
        }

        private void ImplementAccessorDecacheMethod(TypeBuilder accessor)
        {
            throw new NotImplementedException();
        }

        private MethodBuilder ImplementIsCachedMethod()
        {
            var method = typeBuilder.DefineMethod
            (
                GetMemberName("IsCached"),
                MethodAttributes.Public | MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                typeof(bool),
                new[] { type.MakeByRefType() }
            );

            method.DefineParameter(1, ParameterAttributes.Out, "value");

            var il = method.GetILGenerator();
            var uncached = il.DefineLabel();

            // if (($flags & bit) == 0) goto uncached
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            EmitLoadFlagsAndMask(il);                   // [0] $flags, [1] bit
            il.Emit(OpCodes.And);                       // [0] $flags & bit
            il.Emit(OpCodes.Brfalse_S, uncached);       // [-] if (!^) goto uncached

            // value = $value
            il.Emit(OpCodes.Ldarg_1);                   // [0] &value
            il.Emit(OpCodes.Ldarg_0);                   // [0] &value, [0] this
            il.Emit(OpCodes.Ldfld, valueField);         // [0] &value, [0] $value
            il.Emit(OpCodes.Stobj, type);               // [-] value = ^

            // return true
            il.Emit(OpCodes.Ldc_I4_1);                  // [0] true 
            il.Emit(OpCodes.Ret);                       // [-] return ^

            // uncached: value = default(type)
            il.MarkLabel(uncached);
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldarg_1);               // [0] &value
                il.Emit(OpCodes.Initobj, type);         // [-] value = default(type)
            }
            else // (type.IsReferenceType)
            {
                il.Emit(OpCodes.Ldarg_1);               // [0] &value
                il.Emit(OpCodes.Ldnull);                // [0] &value, [1] null
                il.Emit(OpCodes.Stobj, type);           // [-] value = ^
            }

            // return false
            il.Emit(OpCodes.Ldc_I4_0);                  // [0] false 
            il.Emit(OpCodes.Ret);                       // [-] return ^

            return method;
        }

        private MethodBuilder ImplementEncacheMethod()
        {
            var method = typeBuilder.DefineMethod
            (
                GetMemberName("Encache"),
                MethodAttributes.Public | MethodAttributes.HideBySig,
                null, // void
                new[] { type }
            );

            method.DefineParameter(1, ParameterAttributes.In, "value");

            var il = method.GetILGenerator();

            // $value = value
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldarg_1);                   // [0] this, [1] value
            il.Emit(OpCodes.Stfld, valueField);         // [-] $value = ^

            // $flags |= bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [0] this, [1] this
            EmitLoadFlagsAndMask(il);  // [0] this, [1] $flags, [2] bit
            il.Emit(OpCodes.Or);                        // [0] this, [1] $flags | bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $flags = ^

            // return
            il.Emit(OpCodes.Ret);                       // [-] return

            return method;
        }

        private MethodBuilder ImplementDecacheMethod()
        {
            var method = typeBuilder.DefineMethod
            (
                GetMemberName("Decache"),
                MethodAttributes.Public | MethodAttributes.HideBySig,
                null, // void
                Type.EmptyTypes
            );

            var il = method.GetILGenerator();

            // $value = default(type)
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldarg_0);               // [0] this
                il.Emit(OpCodes.Ldflda, valueField);    // [0] &$value
                il.Emit(OpCodes.Initobj, type);         // [-] $value = ^
            }
            else // (type.IsReferenceType)
            {
                il.Emit(OpCodes.Ldarg_0);               // [0] this
                il.Emit(OpCodes.Ldnull);                // [0] this, [1] null
                il.Emit(OpCodes.Stfld, valueField);     // [-] $value = ^
            }

            // $flags &= ~bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [0] this, [1] this
            EmitLoadFlagsAndMask(il);  // [0] this, [1] $flags, [2] bit
            il.Emit(OpCodes.Not);                       // [0] this, [1] $flags, [2] ~bit
            il.Emit(OpCodes.And);                       // [0] this, [1] $flags & ~bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $flags = ^

            // return
            il.Emit(OpCodes.Ret);                       // [-] return

            return method;
        }

        private void EmitLoadFlagsAndMask(ILGenerator il)
        {
            il.Emit(OpCodes.Ldfld, flagsField); // [0] $flags

            switch (flagsBit)
            {
                case 0:  il.Emit(OpCodes.Ldc_I4_1); break;
                case 1:  il.Emit(OpCodes.Ldc_I4_2); break;
                case 2:  il.Emit(OpCodes.Ldc_I4_4); break;
                case 3:  il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if      (flagsBit < 8)  { il.Emit(OpCodes.Ldc_I4_S, 1  << flagsBit); break;  }
                    else if (flagsBit < 32) { il.Emit(OpCodes.Ldc_I4,   1  << flagsBit); break;  }
                    else               { il.Emit(OpCodes.Ldc_I8,   1L << flagsBit); return; }
            }

            if (flagsField.FieldType == typeof(long))
                il.Emit(OpCodes.Conv_I8);
            
            // [0] $flags, [1] mask
        }

        private static string GetFlagsFieldName(int index)
        {
            return string.Concat("$Flags[", GetFlagsIndex(index).ToStringInvariant(), "]");
        }

        private static string GetMemberPrefix(int index, string name)
        {
            return string.Concat("$Props[", index.ToStringInvariant(), "]<", name, ">.");
        }

        private string GetMemberName(string suffix)
        {
            return string.Concat(memberPrefix, suffix);
        }

        private const int
            BitsPerInt32 = sizeof(int) * 8;
    }
}
