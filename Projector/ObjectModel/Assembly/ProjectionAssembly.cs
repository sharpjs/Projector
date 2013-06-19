namespace Projector.ObjectModel
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    internal sealed class ProjectionAssembly
    {
        private readonly string          name;
        private readonly AssemblyBuilder assembly;
        private readonly ModuleBuilder   module;

        public ProjectionAssembly(string name, ProjectionOptions options)
        {
            this.name = name;

            assembly = AppDomain.CurrentDomain.DefineDynamicAssembly
            (
                new AssemblyName(name),
                GetAssemblyAccess(options)
            );

            module = 0 == (options & ProjectionOptions.SaveAssemblies)
                ? assembly.DefineDynamicModule(name)
                : assembly.DefineDynamicModule(name, FileName);
        }

        public string Name
        {
            get { return name; }
        }

        public string FileName
        {
            get { return name + ".dll"; }
        }

        public void Save()
        {
            assembly.Save(FileName);
        }

        internal ProjectionConstructor ImplementProjectionClass(ProjectionStructureType projectionType)
        {
            var typeBuilder = module.DefineType
            (
                GetProjectionClassName(projectionType.UnderlyingType),
                TypeAttributes.Public | TypeAttributes.Class |
                TypeAttributes.Sealed | TypeAttributes.Serializable
            );

            typeBuilder.SetParent(typeof(Projection));
            typeBuilder.AddInterfaceImplementation(projectionType.UnderlyingType);

            // TODO: Implement mixin attributes
            //AddDebuggerDisplayAttribute(typeBuilder);

            var constructor =
            ImplementConstructor  (typeBuilder);
            ImplementFactoryMethod(typeBuilder, constructor);
            ImplementTypeProperty (typeBuilder);

            var properties =
            CollectProjectionProperties  (projectionType.Properties);
            ImplementProjectionProperties(typeBuilder, properties);

            var projectionClass = typeBuilder.CreateType();

            InitializeTypeProperty        (projectionClass, projectionType);
            InitializeProjectionProperties(projectionClass, properties);

            return CreateFactoryDelegate(projectionClass);
        }

        private static string GetProjectionClassName(Type type)
        {
            return string.Concat
            (
                type.Namespace,
                ".",
                type.Name.RemoveInterfacePrefix(),
                "Projection"
            );
        }

        private static ConstructorInfo ImplementConstructor(TypeBuilder typeBuilder)
        {
            var constructor = typeBuilder.DefineConstructor
            (
                MethodAttributes.Private | MethodAttributes.HideBySig,
                CallingConventions.Standard,
                ConstructorParameterTypes
            );

            var baseConstructor = typeof(Projection).GetConstructor
            (
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, // binder
                ConstructorParameterTypes,
                null // modifiers
            );

            constructor.DefineParameter(1, ParameterAttributes.None, "instance");

            var il = constructor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, baseConstructor);
            il.Emit(OpCodes.Ret);

            return constructor;
        }

        private static void ImplementFactoryMethod(TypeBuilder typeBuilder, ConstructorInfo constructor)
        {
            var factory = typeBuilder.DefineMethod
            (
                FactoryMethodName,
                MethodAttributes.Public | MethodAttributes.Static |
                MethodAttributes.HideBySig,
                typeof(Projection),
                ConstructorParameterTypes
            );

            factory.DefineParameter(1, ParameterAttributes.None, "instance");

            var il = factory.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);
        }

        private static void ImplementTypeProperty(TypeBuilder typeBuilder)
        {
            var field = typeBuilder.DefineField
            (
                TypeFieldName,
                BaseTypeProperty.PropertyType,
                FieldAttributes.Public | FieldAttributes.Static
            );

            var property = typeBuilder.DefineProperty
            (
                TypePropertyName,
                BaseTypeProperty.Attributes,
                CallingConventions.HasThis,
                BaseTypeProperty.PropertyType,
                null // parameterTypes
            );

            var baseGetMethod = BaseTypeProperty.GetGetMethod(nonPublic: true);

            var getMethod = typeBuilder.DefineMethod
            (
                string.Concat(GetterPrefix, TypePropertyName),
                baseGetMethod.Attributes   & MethodAttributes.MemberAccessMask |
                MethodAttributes.Virtual   | MethodAttributes.Final            |
                MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                BaseTypeProperty.PropertyType,
                Type.EmptyTypes
            );

            var il = getMethod.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, field);
            il.Emit(OpCodes.Ret);

            property.SetGetMethod(getMethod);
            typeBuilder.DefineMethodOverride(getMethod, baseGetMethod);
        }

        private struct ProjectionPropertyInfo
        {
            public ProjectionProperty Property;
            public string             PropertyFieldName;
        }

        private static ProjectionPropertyInfo[] CollectProjectionProperties(ProjectionPropertyCollection properties)
        {
            var infos = new ProjectionPropertyInfo[properties.Count];
            var index = 0;

            foreach (var property in properties)
            {
                infos[index++] = new ProjectionPropertyInfo
                {
                    Property          = property,
                    PropertyFieldName = GetPropertyFieldName(property, index),
                };
            }

            return infos;
        }

        private static string GetPropertyFieldName(ProjectionProperty property, int index)
        {
            return string.Concat("$_p", index.ToString(), "_", property.Name);
        }

        private static string GetValueFieldName(ProjectionProperty property, int index)
        {
            return string.Concat("$_v", index.ToString(), "_", property.Name);
        }

        private static string GetFlagsFieldName(int index)
        {
            return string.Concat("$_flags" + index.ToString());
        }

        private static void ImplementProjectionProperties(TypeBuilder typeBuilder, ProjectionPropertyInfo[] properties)
        {
            var flagsType  = properties.Length < 32 ? typeof(int) : typeof(long);
            var flagsField = null as FieldInfo;

            for (var i = 0; i < properties.Length; i++)
            {
                if ((i & 63) == 0)
                {
                    flagsField = typeBuilder.DefineField
                    (
                        GetFlagsFieldName(i),
                        flagsType,
                        FieldAttributes.Private
                    );
                }
                var item = properties[i];
                ImplementProjectionProperty(typeBuilder, i, flagsField, item.Property, item.PropertyFieldName);
            }
        }

        private static void ImplementProjectionProperty(TypeBuilder typeBuilder, int index,
            FieldInfo flagsField, ProjectionProperty propertyInfo, string propertyFieldName)
        {
            var propertyField = typeBuilder.DefineField
            (
                propertyFieldName,
                typeof(ProjectionProperty),
                FieldAttributes.Public | FieldAttributes.Static
            );

            var valueField = typeBuilder.DefineField
            (
                GetValueFieldName(propertyInfo, index),
                propertyInfo.PropertyType.UnderlyingType,
                FieldAttributes.Private
            );

            var property = typeBuilder.DefineProperty
            (
                propertyInfo.Name,
                PropertyAttributes.None,
                CallingConventions.HasThis,
                propertyInfo.PropertyType,
                null // parameterTypes
            );

            var cacheMethod = ImplementCacheMethod(typeBuilder, propertyInfo, valueField, flagsField, index);
            var invalidateMethod = ImplementInvalidateMethod(typeBuilder, propertyInfo, valueField, flagsField, index);

            if (propertyInfo.CanRead)
            {
                var getMethod = ImplementGetMethod(typeBuilder, propertyInfo, propertyField, valueField, flagsField, index);
                property.SetGetMethod(getMethod);
            }

            if (propertyInfo.CanWrite)
            {
                var setMethod = ImplementSetMethod(typeBuilder, propertyInfo, propertyField, valueField, flagsField, index, invalidateMethod);
                property.SetSetMethod(setMethod);
            }

            // implement untyped getter
            // implement untyped setter
        }

        private static MethodBuilder ImplementGetMethod(TypeBuilder typeBuilder,
            ProjectionProperty propertyInfo, FieldInfo propertyField, FieldInfo valueField,
            FieldInfo flagsField, int index)
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
            EmitCallBaseGetter      (il, BaseGetMethod, propertyField, valueField, flagsField, index);
            EmitReturnTypedOrDefault(il, propertyInfo.PropertyType.UnderlyingType, valueField, flagsField, index);

            typeBuilder.DefineMethodOverride(getMethod, propertyInfo.UnderlyingGetter);
            return getMethod;
        }

        private static MethodBuilder ImplementSetMethod(TypeBuilder typeBuilder,
            ProjectionProperty propertyInfo, FieldInfo propertyField, FieldInfo valueField,
            FieldInfo flagsField, int index, MethodInfo invalidateMethod)
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
            EmitCallBaseSetter(il, BaseSetMethod, propertyField, valueField, flagsField, index, invalidateMethod);

            typeBuilder.DefineMethodOverride(setMethod, propertyInfo.UnderlyingSetter);
            return setMethod;
        }

        private static void EmitCallBaseGetter(ILGenerator il, MethodInfo baseMethod,
            FieldInfo propertyField, FieldInfo valueField, FieldInfo flagsField, int bit)
        {
            var value = il.DeclareLocal(typeof(object)); // loc.0
            var uncached = il.DefineLabel();

            // if ((flags & mask) == 0) goto uncached
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            EmitLoadFlagsAndMask(il, flagsField, bit);  // [0] flags, [1] bit
            il.Emit(OpCodes.And);                       // [0] and(flags, bit)
            il.Emit(OpCodes.Brfalse_S, uncached);       // [-] if (!^) goto getValueSlow

            // return value field
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldfld, valueField);         // [0] $_vN_SomeProperty
            il.Emit(OpCodes.Ret);                       // [-] return ^

            // value = GetPropertyValueCore(property, GetterOptions.Virtual)
            il.MarkLabel(uncached);
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldsfld, propertyField);     // [1] $_pN_SomeProperty
            il.Emit(OpCodes.Ldc_I4_1);                  // [2] GetterOptions.Virtual
            il.Emit(OpCodes.Call, baseMethod);          // [0] base.GetPropertyValueCore()
            il.Emit(OpCodes.Stloc_0);                   // [-] value = ^
        }

        private static void EmitAbstractGetter(ILGenerator il, MethodInfo baseMethod,
            FieldInfo propertyField, FieldInfo valueField, FieldInfo flagsField, int bit)
        {
            var value = il.DeclareLocal(typeof(object)); // loc.0
            var uncached = il.DefineLabel();
            var type = valueField.FieldType;

            // if ((flag & mask) == 0) goto uncached
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            EmitLoadFlagsAndMask(il, flagsField, bit);  // [0] flags, [1] bit
            il.Emit(OpCodes.And);                       // [0] and(flags, bit)
            il.Emit(OpCodes.Brfalse_S, uncached);       // [-] if (!^) goto getValueSlow

            // return cached value
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldfld, valueField);         // [0] $_vN_SomeProperty
            if (type.IsValueType)
                il.Emit(OpCodes.Box, type);
            il.Emit(OpCodes.Ret);                       // [-] return ^

            // value = GetPropertyValueCore(property, GetterOptions.Virtual)
            il.MarkLabel(uncached);
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldsfld, propertyField);     // [1] $_pN_SomeProperty
            il.Emit(OpCodes.Ldc_I4_1);                  // [2] GetterOptions.Virtual
            il.Emit(OpCodes.Call, baseMethod);          // [0] base.GetPropertyValueCore()
            il.Emit(OpCodes.Stloc_0);                   // [-] value = ^

            // ...copied form lower method
            // but ugh, we have to set the typed field anyway

            var returnDefault = il.DefineLabel();

            // if (value is Unknown) return default(type)
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Isinst, typeof(Unknown));
            il.Emit(OpCodes.Brtrue_S, returnDefault);
            il.Emit(OpCodes.Ret);

            if (type.IsValueType)
            {
                var result = il.DeclareLocal(type); // loc.1

                // return default(type)
                il.MarkLabel(returnDefault);
                il.Emit(OpCodes.Ldloca_S, result);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ret);
            }
            else // is reference type
            {
                // return default(type)
                il.MarkLabel(returnDefault);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
            }

            if (valueField.FieldType.IsValueType)
                il.Emit(OpCodes.Box, valueField.FieldType);
            il.Emit(OpCodes.Ret);                       // [-] return ^
        }

        private static void EmitLoadFlagsAndMask(ILGenerator il, FieldInfo flagsField, int bit)
        {
            il.Emit(OpCodes.Ldfld, flagsField);         // [0] $_flagsN

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

        private static void EmitCallBaseSetter(ILGenerator il, MethodInfo baseMethod,
            FieldInfo propertyField, FieldInfo valueField, FieldInfo flagsField, int bit, MethodInfo invalidateMethod)
        {
            var uncacheable = il.DefineLabel();
            var type = valueField.FieldType;

            // if (!base.SetPropertyValueCore(property, value)) goto done
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldsfld, propertyField);     // [1] $_pN_SomeProperty
            il.Emit(OpCodes.Ldarg_1);                   // [2] value
            if (type.IsValueType)                       //
                il.Emit(OpCodes.Box, type);             // [2] value (boxed)
            il.Emit(OpCodes.Call, baseMethod);          // [0] base.SetPropertyValueCore()
            il.Emit(OpCodes.Brfalse_S, uncacheable);           // [-] if (!^) goto done

            // $_vN_SomeProperty = value
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldarg_1);                   // [2] value
            il.Emit(OpCodes.Stfld, valueField);         // [-] $_vN_SomeProperty = ^

            // $_flagsN |= bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [1] ^
            EmitLoadFlagsAndMask(il, flagsField, bit);  // [2] bit, [1] $_flagsN
            il.Emit(OpCodes.Or);                        // [1] flags | bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $_flagsN
            il.Emit(OpCodes.Ret);                       // [-] return

            // done: return
            il.MarkLabel(uncacheable);
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Call, invalidateMethod);    // [-] $Invalidate()
            il.Emit(OpCodes.Ret);                       // [-] return
        }

        private static void EmitAbstractSetter(ILGenerator il, MethodInfo baseMethod,
            FieldInfo propertyField, FieldInfo valueField, FieldInfo flagsField, int index)
        {
            var done = il.DefineLabel();
            var fail = il.DefineLabel();
            var type = valueField.FieldType;

            // if (value is type == false) goto error;
            il.Emit(OpCodes.Ldarg_1);                   // [0] value (boxed)
            il.Emit(OpCodes.Isinst, type);              // [0] is assignable
            il.Emit(OpCodes.Brfalse_S, done);           // [-] if (!^) goto fail

            // if (!base.SetPropertyValueCore(property, value)) goto done
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldsfld, propertyField);     // [1] $_pN_SomeProperty
            il.Emit(OpCodes.Ldarg_1);                   // [2] value (boxed)
            il.Emit(OpCodes.Call, baseMethod);          // [0] base.SetPropertyValueCore()
            il.Emit(OpCodes.Brfalse_S, done);           // [-] if (!^) goto done

            // $_vN_SomeProperty = value
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldarg_1);                   // [2] value (boxed)
            if (type.IsValueType)                       //
                il.Emit(OpCodes.Unbox_Any, type);       // [2] value
            il.Emit(OpCodes.Stfld, valueField);         // [-] $_vN_SomeProperty = ^

            // $_flagsN |= bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [1] ^
            EmitLoadFlagsAndMask(il, flagsField, index);// [2] bit, [1] $_flagsN
            il.Emit(OpCodes.Or);                        // [1] flags | bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $_flagsN

            // done: return
            il.MarkLabel(done);
            il.Emit(OpCodes.Ret);                       // [-] return

            // fail: throw InvalidCastException
            il.MarkLabel(fail);
            il.ThrowException(typeof(InvalidCastException));
        }

        private static void EmitReturnTypedOrDefault(ILGenerator il, Type type,
            FieldInfo valueField, FieldInfo flagsField, int bit)
        {
            var returnDefault = il.DefineLabel();
            var cache = il.DefineLabel();

            // if (value is Unknown) return default(type)
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Isinst, typeof(Unknown));
            il.Emit(OpCodes.Brtrue_S, returnDefault);

            if (type.IsValueType)
            {
                var result = il.DeclareLocal(type); // loc.1

                // result = (type) value; goto label2
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Br_S, cache);

                // result = default(type)
                il.MarkLabel(returnDefault);
                il.Emit(OpCodes.Ldloca_S, result);
                il.Emit(OpCodes.Initobj, type);

                il.MarkLabel(cache);

                // $_vN_SomeProperty = value
                il.Emit(OpCodes.Ldarg_0);                   // [0] this
                il.Emit(OpCodes.Ldloc_1);                   // [1] value
                il.Emit(OpCodes.Stfld, valueField);         // [-] $_vN_SomeProperty = ^

                // $_flagsN |= bit
                il.Emit(OpCodes.Ldarg_0);                   // [0] this
                il.Emit(OpCodes.Dup);                       // [1] ^
                EmitLoadFlagsAndMask(il, flagsField, bit);  // [2] bit, [1] $_flagsN
                il.Emit(OpCodes.Or);                        // [1] flags | bit
                il.Emit(OpCodes.Stfld, flagsField);         // [-] $_flagsN

                // return result
                il.Emit(OpCodes.Ldloc_1);                   // [0] result
                il.Emit(OpCodes.Ret);                       // [-] return

            }
            else // is reference type
            {
                // return (type) value
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Castclass, type);
                il.Emit(OpCodes.Ret);

                // return default(type)
                il.MarkLabel(returnDefault);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
            }
        }

        private static MethodBuilder ImplementCacheMethod(TypeBuilder typeBuilder,
            ProjectionProperty propertyInfo, FieldInfo valueField, FieldInfo flagsField, int bit)
        {
            var method = typeBuilder.DefineMethod
            (
                string.Concat("$Cache_P", bit.ToString(), "_", propertyInfo.Name),
                MethodAttributes.Private | MethodAttributes.HideBySig,
                null, // void
                new[] { propertyInfo.PropertyType.UnderlyingType }
            );

            method.DefineParameter(1, ParameterAttributes.None, "value");

            var il = method.GetILGenerator();

            // $_vN_SomeProperty = value
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldarg_1);                   // [2] value
            il.Emit(OpCodes.Stfld, valueField);         // [-] $_vN_SomeProperty = ^

            // $_flagsN |= bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [1] ^
            //il.Emit(OpCodes.Ldfld, flagsField);         // [0] $_flagsN
            EmitLoadFlagsAndMask(il, flagsField, bit);  // [2] bit, [1] $_flagsN
            il.Emit(OpCodes.Or);                        // [1] flags | bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $_flagsN

            // return
            il.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodBuilder ImplementInvalidateMethod(TypeBuilder typeBuilder,
            ProjectionProperty propertyInfo, FieldInfo valueField, FieldInfo flagsField, int bit)
        {
            var method = typeBuilder.DefineMethod
            (
                string.Concat("$Invalidate_P", bit.ToString(), "_", propertyInfo.Name),
                MethodAttributes.Private | MethodAttributes.HideBySig,
                null, // void
                Type.EmptyTypes
            );

            var il = method.GetILGenerator();

            if (valueField.FieldType.IsValueType)
            {
                // $_vN_SomeProperty = default(type)
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldflda, valueField);
                il.Emit(OpCodes.Initobj, valueField.FieldType);
            }
            else
            {
                // $vN_SomeProperty = default(type)
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Stfld, valueField);
            }

            // $_flagsN &= ~bit
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Dup);
            //il.Emit(OpCodes.Ldfld, flagsField);         // [0] $_flagsN
            EmitLoadFlagsAndMask(il, flagsField, bit);
            il.Emit(OpCodes.Not);
            il.Emit(OpCodes.And);
            il.Emit(OpCodes.Stfld, flagsField);
            il.Emit(OpCodes.Ret);

            return method;
        }

        private static ProjectionConstructor CreateFactoryDelegate(Type type)
        {
            return (ProjectionConstructor) Delegate.CreateDelegate
            (
                typeof(ProjectionConstructor),
                type,
                FactoryMethodName
            );
        }

        private static void InitializeTypeProperty(Type type, ProjectionType projectionType)
        {
            SetStaticField(type, TypeFieldName, projectionType);
        }

        private static void InitializeProjectionProperties(Type type, ProjectionPropertyInfo[] properties)
        {
            foreach (var item in properties)
                SetStaticField(type, item.PropertyFieldName, item.Property);
        }

        private static void SetStaticField(Type type, string name, object value)
        {
            type.GetField(name, BindingFlags.Public | BindingFlags.Static)
                .SetValue(null, value);
        }

        private static MethodAttributes ImplementAbstract(MethodAttributes attributes)
        {
            attributes &= ~ (MethodAttributes.Abstract | MethodAttributes.NewSlot);
            attributes |=  MethodAttributes.Virtual
                       |   MethodAttributes.ReuseSlot
                       |   MethodAttributes.Final;
            return attributes;
        }

        private static AssemblyBuilderAccess GetAssemblyAccess(ProjectionOptions options)
        {
            switch (options & ProjectionOptionsInternal.AssemblyModes)
            {
                default:
                case ProjectionOptions.None:
                    return AssemblyBuilderAccess.Run;

                case ProjectionOptions.SaveAssemblies:
                    return AssemblyBuilderAccess.RunAndSave;

                case ProjectionOptions.CollectAssemblies:
                    return AssemblyBuilderAccess.RunAndCollect;
            }
        }

        private const string NamePrefix       = "Projector.";
        private const int    NameSuffixLength = 8;

        private const string
            GetterPrefix      = "get_",
            SetterPrefix      = "set_",
            TypeFieldName     = "$_type",
            TypePropertyName  = "$_Type",
            FactoryMethodName = "$_Create";

        private static readonly Type[]
            ConstructorParameterTypes = { typeof(ProjectionInstance) };

        private const BindingFlags PrivateStaticFlags
            = BindingFlags.NonPublic
            | BindingFlags.Static;

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

        private static readonly PropertyInfo
            BaseTypeProperty = typeof(Projection).GetProperty
            (
                "Type",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
            );
    }
}
