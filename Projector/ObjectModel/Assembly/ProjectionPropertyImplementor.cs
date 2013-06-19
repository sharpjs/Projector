namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    internal struct ProjectionPropertyFixup
    {
        public ProjectionProperty Property;
        public string             PropertyFieldName;
        public string             AbstractGetterName;
        public string             AbstractSetterName;
        public string             InvalidatorName;
    }

    internal struct ProjectionPropertyImplementor
    {
        private TypeBuilder typeBuilder;
        private string      id;
        private string      name;
        private int         bit;
        private Type        propertyType;
        private ProjectionProperty propertyInfo;
        private FieldInfo   propertyField;
        private FieldInfo   valueField;
        private FieldInfo   flagsField;
        private MethodInfo  retainMethod;
        private MethodInfo  forgetMethod;

        internal TypeBuilder TypeBuilder
        {
            get { return typeBuilder; }
            set { typeBuilder = value; }
        }

        internal FieldInfo FlagsField
        {
            get { return flagsField; }
            set { flagsField = value; }
        }

        internal ProjectionPropertyFixup ImplementProjectionProperty(int index, ProjectionProperty projectionProperty)
        {
            id            = index.ToString(CultureInfo.InvariantCulture);
            name          = projectionProperty.Name;
            propertyType  = projectionProperty.PropertyType.UnderlyingType;

            propertyField = ImplementPropertyField();
            valueField    = ImplementValueField();

            bit           = index & (flagsField.FieldType == typeof(long) ? 63 : 31);
            retainMethod  = ImplementRetainMethod(bit);
            forgetMethod  = ImplementForgetMethod(bit);

            var property = typeBuilder.DefineProperty
            (
                projectionProperty.Name,
                PropertyAttributes.None,
                CallingConventions.HasThis,
                projectionProperty.PropertyType,
                null // parameterTypes
            );

            return new ProjectionPropertyFixup
            {
                Property           = projectionProperty,
                PropertyFieldName  = propertyField.Name,
                AbstractGetterName = null,
                AbstractSetterName = null,
                InvalidatorName    = forgetMethod.Name
            };
        }

        private FieldInfo ImplementPropertyField()
        {
            return typeBuilder.DefineField
            (
                GetName(PropertyFieldPrefix),
                typeof(ProjectionProperty),
                FieldAttributes.Public | FieldAttributes.Static
            );
        }

        private FieldInfo ImplementValueField()
        {
            return typeBuilder.DefineField
            (
                GetName(ValueFieldPrefix),
                propertyType,
                FieldAttributes.Private
            );
        }

        private MethodBuilder ImplementPropertyGetMethod()
        {
            var getMethod = typeBuilder.DefineMethod
            (
                string.Concat(GetterPrefix, propertyInfo.Name),
                MethodAttributes.Private   | MethodAttributes.NewSlot |
                MethodAttributes.Virtual   | MethodAttributes.Final   |
                MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                propertyInfo.PropertyType,
                Type.EmptyTypes
            );

            var il = getMethod.GetILGenerator();
            if (!propertyInfo.IsVolatile)
                EmitReturnCachedValue(il, false);
            EmitCallBaseGetter      (il);
     //       EmitReturnTypedOrDefault(il);

            typeBuilder.DefineMethodOverride(getMethod, propertyInfo.UnderlyingGetter);
            return getMethod;
        }

        private void EmitReturnCachedValue(ILGenerator il, bool box)
        {
            var uncached = il.DefineLabel();

            // if ((flags & mask) == 0) goto uncached
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldfld, flagsField);         // [0] $flags
            EmitLoadBitMask(il, flagsField, bit);       // [1] bit
            il.Emit(OpCodes.And);                       // [0] and(flags, bit)
            il.Emit(OpCodes.Brfalse_S, uncached);       // [-] if (!^) goto getValueSlow

            // return value field
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldfld, valueField);         // [0] $value
            if (box) il.Emit(OpCodes.Box, propertyType);// [0] value (boxed)
            il.Emit(OpCodes.Ret);                       // [-] return ^

            il.MarkLabel(uncached);
        }

        private void EmitCallBaseGetter(ILGenerator il)
        {
            var value = il.DeclareLocal(typeof(object)); // loc.0

            // value = GetPropertyValueCore(property, GetterOptions.Virtual)
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldsfld, propertyField);     // [1] $_pN_SomeProperty
            il.Emit(OpCodes.Ldc_I4_1);                  // [2] GetterOptions.Virtual
            il.Emit(OpCodes.Call, BaseGetMethod);       // [0] base.GetPropertyValueCore()
            il.Emit(OpCodes.Stloc_0);                   // [-] value = ^
        }

        //private void EmitReturnTypedOrDefault(ILGenerator il)
        //{
        //    var returnDefault = il.DefineLabel();
        //    var cache = il.DefineLabel();
        //    var type = propertyType;

        //    // if (value is Unknown) return default(type)
        //    il.Emit(OpCodes.Ldloc_0);
        //    il.Emit(OpCodes.Isinst, typeof(Unknown));
        //    il.Emit(OpCodes.Brtrue_S, returnDefault);

        //    if (type.IsValueType)
        //    {
        //        var result = il.DeclareLocal(type); // loc.1

        //        if (propertyInfo.IsVolatile)
        //        {
        //            // return (type) value;
        //            il.Emit(OpCodes.Ldloc_0);
        //            il.Emit(OpCodes.Unbox_Any, type);
        //            il.Emit(OpCodes.Ret);

        //            // return default(type)
        //            il.MarkLabel(returnDefault);
        //            il.Emit(OpCodes.Ldloca_S, result);
        //            il.Emit(OpCodes.Initobj, type);
        //            il.Emit(OpCodes.Ldloc_1);
        //            il.Emit(OpCodes.Ret);
        //        }
        //        else
        //        {
        //        }


        //        if (propertyInfo.IsVolatile)
        //        else
        //        {
        //            il.Emit(OpCodes.Ldloc_0);
        //            il.Emit(OpCodes.Unbox_Any, type);
        //            il.Emit(OpCodes.Stloc_1);
        //            il.Emit(OpCodes.Br_S, cache);
        //        }

        //        // result = default(type)
        //        il.MarkLabel(returnDefault);
        //        il.Emit(OpCodes.Ldloca_S, result);
        //        il.Emit(OpCodes.Initobj, type);

        //        il.MarkLabel(cache);

        //        // $_vN_SomeProperty = value
        //        il.Emit(OpCodes.Ldarg_0);                   // [0] this
        //        il.Emit(OpCodes.Ldloc_1);                   // [1] value
        //        il.Emit(OpCodes.Stfld, valueField);         // [-] $_vN_SomeProperty = ^

        //        // $_flagsN |= bit
        //        il.Emit(OpCodes.Ldarg_0);                   // [0] this
        //        il.Emit(OpCodes.Dup);                       // [1] ^
        //        EmitLoadBitMask(il, flagsField, bit);       // [2] bit, [1] $_flagsN
        //        il.Emit(OpCodes.Or);                        // [1] flags | bit
        //        il.Emit(OpCodes.Stfld, flagsField);         // [-] $_flagsN

        //        // return result
        //        il.Emit(OpCodes.Ldloc_1);                   // [0] result
        //        il.Emit(OpCodes.Ret);                       // [-] return

        //    }
        //    else // is reference type
        //    {
        //        if (propertyInfo.IsVolatile)
        //        {
        //        }
        //        else
        //        {
        //        }

        //        // return (type) value
        //        il.Emit(OpCodes.Ldloc_0);
        //        il.Emit(OpCodes.Castclass, type);
        //        il.Emit(OpCodes.Ret);

        //        // return default(type)
        //        il.MarkLabel(returnDefault);
        //        il.Emit(OpCodes.Ldnull);
        //        il.Emit(OpCodes.Ret);
        //    }
        //}

        private MethodBuilder ImplementPropertySetMethod()
        {
            var setMethod = typeBuilder.DefineMethod
            (
                string.Concat(SetterPrefix, propertyInfo.Name),
                MethodAttributes.Private   | MethodAttributes.NewSlot |
                MethodAttributes.Virtual   | MethodAttributes.Final   |
                MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                null, // void
                new[] { propertyInfo.PropertyType.UnderlyingType }
            );

            setMethod.DefineParameter(1, ParameterAttributes.None, "value");

            var il = setMethod.GetILGenerator();
            EmitCallBaseSetter(il);

            typeBuilder.DefineMethodOverride(setMethod, propertyInfo.UnderlyingSetter);
            return setMethod;
        }

        private void EmitCallBaseSetter(ILGenerator il)
        {
            throw new NotImplementedException();
        }

        private MethodInfo ImplementRetainMethod(int bit)
        {
            var method = typeBuilder.DefineMethod
            (
                GetName(CacheMethodPrefix),
                MethodAttributes.Private | MethodAttributes.HideBySig,
                null, // void
                new[] { propertyType }
            );

            method.DefineParameter(1, ParameterAttributes.None, "value");

            var il = method.GetILGenerator();

            // $value = value
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldarg_1);                   // [1] value
            il.Emit(OpCodes.Stfld, valueField);         // [-] $value = ^

            // $flags |= bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [1] ^
            il.Emit(OpCodes.Ldfld, flagsField);         // [1] $flags
            EmitLoadBitMask(il, flagsField, bit);       // [2] bit,
            il.Emit(OpCodes.Or);                        // [1] flags |= bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $flags = ^

            // return
            il.Emit(OpCodes.Ret);

            return method;
        }

        private MethodInfo ImplementForgetMethod(int bit)
        {
            var method = typeBuilder.DefineMethod
            (
                GetName(InvalidateMethodPrefix),
                MethodAttributes.Private | MethodAttributes.HideBySig,
                null, // void
                Type.EmptyTypes
            );

            var il = method.GetILGenerator();

            // $value = default(type)
            if (propertyType.IsValueType)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldflda, valueField);
                il.Emit(OpCodes.Initobj, propertyType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Stfld, valueField);
            }

            // $_flagsN &= ~bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [1] ^
            il.Emit(OpCodes.Ldfld, flagsField);         // [1] $flags
            EmitLoadBitMask(il, flagsField, bit);       // [2] bit,
            il.Emit(OpCodes.Not);                       // [2] ~bit
            il.Emit(OpCodes.And);                       // [1] flags |&= ~bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $flags = ^

            // return
            il.Emit(OpCodes.Ret);

            return method;
        }

        private static void EmitLoadBitMask(ILGenerator il, FieldInfo flagsField, int bit)
        {
            switch (bit)
            {
                case 0: il.Emit(OpCodes.Ldc_I4_1); return;
                case 1: il.Emit(OpCodes.Ldc_I4_2); return;
                case 2: il.Emit(OpCodes.Ldc_I4_4); return;
                case 3: il.Emit(OpCodes.Ldc_I4_8); return;
            }

            il.Emit(OpCodes.Ldc_I4_1);
            if (flagsField.FieldType == typeof(long))
                il.Emit(OpCodes.Conv_I8);

            switch (bit)
            {
                case 4:  il.Emit(OpCodes.Ldc_I4_4);      break;
                case 5:  il.Emit(OpCodes.Ldc_I4_5);      break;
                case 6:  il.Emit(OpCodes.Ldc_I4_6);      break;
                case 7:  il.Emit(OpCodes.Ldc_I4_7);      break;
                case 8:  il.Emit(OpCodes.Ldc_I4_8);      break;
                default: il.Emit(OpCodes.Ldc_I4_S, bit); break;
            }

            il.Emit(OpCodes.Shl);
        }

        private static string GetPropertyFieldName(ProjectionProperty property, int index)
        {
            return string.Concat("$property", index.ToString(), "_", property.Name);
        }

        private static string GetValueFieldName(ProjectionProperty property, int index)
        {
            return string.Concat("$value", index.ToString(), "_", property.Name);
        }

        private string GetName(string prefix)
        {
            var result = string.Concat(prefix, id, ":", name);
            return result.Length > MaxIdentifierLength
                ? result.Substring(0, MaxIdentifierLength)
                : result;
        }

        private const string
            GetterPrefix           = "get_",
            SetterPrefix           = "set_",
            PropertyFieldPrefix    = "$property:",
            ValueFieldPrefix       = "$value:",
            CacheMethodPrefix      = "$Retain:",
            InvalidateMethodPrefix = "$Forget:";

        private const int
            MaxIdentifierLength = 1023;

        private static readonly MethodInfo
            BaseGetMethod = typeof(Projection).GetMethod
            (
                "GetPropertyValueCore",
                BindingFlags.NonPublic    | BindingFlags.Instance |
                BindingFlags.DeclaredOnly | BindingFlags.ExactBinding,
                null, // binder
                new[] { typeof(ProjectionProperty), typeof(GetterOptions) },
                null // modifiers
            ),
            BaseSetMethod = typeof(Projection).GetMethod
            (
                "SetPropertyValueCore",
                BindingFlags.NonPublic    | BindingFlags.Instance |
                BindingFlags.DeclaredOnly | BindingFlags.ExactBinding,
                null, // binder
                new[] { typeof(ProjectionProperty), typeof(object) },
                null // modifiers
            );
    }
}
