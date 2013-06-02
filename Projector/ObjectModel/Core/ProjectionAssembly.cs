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

            module = 0 == (options & ProjectionOptions.SaveAssembly)
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

        public ProjectionConstructor ImplementProjectionClass(ProjectionStructureType projectionType)
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
            CollectProjectionProperties  (projectionType. Properties);
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
            public string          FieldName;
            public ProjectionProperty Property;
        }

        private static ProjectionPropertyInfo[] CollectProjectionProperties(ProjectionPropertyCollection properties)
        {
            var infos = new ProjectionPropertyInfo[properties.Count];
            var index = 0;

            foreach (var property in properties)
            {
                infos[index++] = new ProjectionPropertyInfo
                {
                    Property  = property,
                    FieldName = GetPropertyFieldName(property, index)
                };
            }

            return infos;
        }

        private static string GetPropertyFieldName(ProjectionProperty property, int index)
        {
            // TODO: Use a name that is illegal in most .NET languages
            return string.Concat
            (
                "_p", index.ToString(),
                "_",  property.Name
            );
        }

        private static void ImplementProjectionProperties(TypeBuilder typeBuilder, ProjectionPropertyInfo[] properties)
        {
            foreach (var item in properties)
                ImplementProjectionProperty(typeBuilder, item.Property, item.FieldName);
        }

        private static void ImplementProjectionProperty(TypeBuilder typeBuilder,
            ProjectionProperty propertyInfo, string propertyFieldName)
        {
            var propertyField = typeBuilder.DefineField
            (
                propertyFieldName,
                typeof(ProjectionProperty),
                FieldAttributes.Public | FieldAttributes.Static
            );

            var property = typeBuilder.DefineProperty
            (
                propertyInfo.Name,
                PropertyAttributes.None,
                CallingConventions.HasThis,
                propertyInfo.PropertyType,
                null // parameterTypes
            );

            if (propertyInfo.CanRead)
            {
                var getMethod = ImplementGetMethod(typeBuilder, propertyInfo, propertyField);
                property.SetGetMethod(getMethod);
            }

            if (propertyInfo.CanWrite)
            {
                var setMethod = ImplementSetMethod(typeBuilder, propertyInfo, propertyField);
                property.SetSetMethod(setMethod);
            }
        }

        private static MethodBuilder ImplementGetMethod(TypeBuilder typeBuilder,
            ProjectionProperty propertyInfo, FieldInfo propertyField)
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
            EmitCallBaseGetter      (il, BaseGetMethod, propertyField);
            EmitReturnTypedOrDefault(il, propertyInfo.PropertyType.UnderlyingType);

            typeBuilder.DefineMethodOverride(getMethod, propertyInfo.UnderlyingGetter);
            return getMethod;
        }

        private static MethodBuilder ImplementSetMethod(TypeBuilder typeBuilder,
            ProjectionProperty propertyInfo, FieldInfo propertyField)
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
            EmitCallBaseSetter(il, BaseSetMethod, propertyField);

            typeBuilder.DefineMethodOverride(setMethod, propertyInfo.UnderlyingSetter);
            return setMethod;
        }

        private static void EmitCallBaseGetter(ILGenerator il, MethodInfo baseMethod, FieldInfo propertyField)
        {
            var value = il.DeclareLocal(typeof(object)); // loc.0

            // value = GetProperty(key, false)
            il.Emit(OpCodes.Ldarg_0);                // [0] this
            il.Emit(OpCodes.Ldsfld, propertyField);  // [1] _FooProperty
            il.Emit(OpCodes.Ldc_I4_1);               // [2] AdaptorOptions.Virtual
            il.Emit(OpCodes.Call, baseMethod);       // base.GetPropertyValueCore()
            il.Emit(OpCodes.Stloc_0);                // value = ^
        }

        private static void EmitCallBaseSetter(ILGenerator il, MethodInfo baseMethod, FieldInfo propertyField)
        {
            // value = GetProperty(key, false)
            il.Emit(OpCodes.Ldarg_0);                // [0] this
            il.Emit(OpCodes.Ldsfld, propertyField);  // [1] _FooProperty
            il.Emit(OpCodes.Ldarg_1);                // [2] value
            il.Emit(OpCodes.Call, baseMethod);       // base.SetPropertyValueCore()
            il.Emit(OpCodes.Pop);                    // discard value
            il.Emit(OpCodes.Ret);                    // return
        }

        private static void EmitReturnTypedOrDefault(ILGenerator il, Type type)
        {
            var returnDefault = il.DefineLabel();

            // if (value is Unknown) return default(type)
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Isinst, typeof(Unknown));
            il.Emit(OpCodes.Brtrue_S, returnDefault);

            if (type.IsValueType)
            {
                var result = il.DeclareLocal(type); // loc.1

                // return (type) value
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Ret);

                // return default(type)
                il.MarkLabel(returnDefault);
                il.Emit(OpCodes.Ldloca_S, result);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ret);
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
                SetStaticField(type, item.FieldName, item.Property);
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
                case ProjectionOptions.None:
                    return AssemblyBuilderAccess.Run;

                case ProjectionOptions.SaveAssembly:
                    return AssemblyBuilderAccess.RunAndSave;

                case ProjectionOptions.CollectAssembly:
                    return AssemblyBuilderAccess.RunAndCollect;

                default:
                    throw Error.InternalError("invalid assembly mode");
            }
        }

        private const string NamePrefix       = "Projector.";
        private const int    NameSuffixLength = 8;

        private const string
            GetterPrefix      = "get_",
            SetterPrefix      = "set_",
            TypeFieldName     = "_type",
            TypePropertyName  = "_Type",
            FactoryMethodName = "_Create";

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
                new[] { typeof(ProjectionProperty), typeof(InvocationOptions) },
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
