namespace Projector.ObjectModel
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    //public interface IFoo
    //{
    //    int Count { get; set; }
    //}

    //public class FooProjection : Projection, IFoo
    //{
    //    public FooProjection(ProjectionInstance instance) : base(instance) { }

    //    internal static ProjectionType type;
    //    public override ProjectionType Type { get { return type; } }

    //    private int flags0;

    //    private int p1_Bar_value;
    //    internal static ProjectionProperty<int> p1_Bar_property;

    //    public int Bar
    //    {
    //        get
    //        {
    //            int value;
    //            if (!p1_Bar_TryGetCached(out value))
    //                return value;
    //            if (!p1_Bar_property.GetValue(this, GetterOptions.Virtual, out value))
    //                return value;
    //            p1_Bar_Encache(value);
    //            return value;
    //        }
    //        set
    //        {
    //            if (p1_Bar_property.SetValue(this, value))
    //                p1_Bar_Encache(value);
    //            else
    //                p1_Bar_Decache();
    //        }
    //    }

    //    private bool p1_Bar_TryGetCached(out int value)
    //    {
    //        if ((flags0 & 2) != 0)
    //            { value = p1_Bar_value; return true; }
    //        else
    //            { value = default(int); return false; }
    //    }

    //    private void p1_Bar_Encache(int value)
    //    {
    //        flags0 |= 2;
    //        p1_Bar_value = value;
    //    }

    //    private void p1_Bar_Decache()
    //    {
    //        flags0 &= ~2;
    //        p1_Bar_value = default(int);
    //    }
    //}

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

            var constructor =
            ImplementConstructor  (typeBuilder);
            ImplementFactoryMethod(typeBuilder, constructor);
            ImplementTypeProperty (typeBuilder);

            var properties =
            ImplementProjectionProperties(typeBuilder, projectionType.Properties);

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

        private static ProjectionPropertyFixup[] ImplementProjectionProperties(TypeBuilder typeBuilder, ProjectionPropertyCollection properties)
        {
            var implementor = new ProjectionPropertyImplementor(typeBuilder);

            var infos      = new ProjectionPropertyFixup[properties.Count];
            var index      = 0;
            //var flagsField = null as FieldInfo;

            foreach (var property in properties)
            {
                //var flagsBit = GetFlagsBit(index);
                //if (flagsBit == 0)
                //    flagsField = ImplementFlagsField(typeBuilder, index, infos.Length);

                //infos[index++] = ImplementProjectionProperty(typeBuilder, index, property, flagsField, flagsBit);

                infos[index++] = implementor.ImplementProjectionProperty(property, index);
            }

            return infos;
        }

        private class ProjectionPropertyImplementor
        {
            private readonly TypeBuilder typeBuilder;

            private ProjectionProperty propertyInfo;
            private string             namePrefix;
            private Type               valueType;

            private int                cachedPropertyCount;
            private int                flagsIndex;
            private int                flagsBit;
            private FieldInfo          flagsField;

            private Type               accessorType;
            private MethodInfo         tryGetCachedMethod, encacheMethod, decacheMethod;

            public ProjectionPropertyImplementor(TypeBuilder typeBuilder)
            {
                this.typeBuilder = typeBuilder;
            }

            public ProjectionPropertyFixup ImplementProjectionProperty(ProjectionProperty propertyInfo, int index)
            {
                this.propertyInfo = propertyInfo;

                namePrefix = GetPropertyMemberPrefix(propertyInfo, index);
                valueType  = propertyInfo.PropertyType.UnderlyingType;

                if (!propertyInfo.IsVolatile)
                {
                    AssignCacheFlag();
                }
            }

            private void AssignCacheFlag()
            {
                var index = cachedPropertyCount++;

                flagsBit = GetFlagsBit(index);
                if (flagsBit == 0)
                    flagsField = ImplementFlagsField(index);
            }

            private FieldInfo ImplementFlagsField(int index)
            {
                flagsField = typeBuilder.DefineField
                (
                    GetFlagsFieldName(GetFlagsIndex(index)),
                    count - index < BitsPerInt32 ? typeof(Int32) : typeof(Int64),
                    FieldAttributes.Private
                );
            }

            public ProjectionPropertyImplementor ImplementPropertyField()
            {
                var propertyField = typeBuilder.DefineField
                (
                    namePrefix + "Accessor",
                    typeof(ProjectionProperty<>).MakeGenericType(valueType),
                    FieldAttributes.Public | FieldAttributes.Static
                );

                return this;
            }
        }

        // Property 'value cached' flags are stored in packed bit fields.
        // Bit fields are Int64 or Int32, depending on the required quantity of bits.
        // Each property maps to a particular field/bit, depending on the property's index.

        private static int GetFlagsIndex(int propertyIndex)
        {
            return propertyIndex >> 6; // i.e. log2(BitsPerInt64)
        }

        private static int GetFlagsBit(int propertyIndex)
        {
            return propertyIndex & (BitsPerInt64 - 1);
        }

        private static FieldInfo ImplementFlagsField(TypeBuilder typeBuilder, int index, int count)
        {
            return typeBuilder.DefineField
            (
                GetFlagsFieldName(GetFlagsIndex(index)),
                count - index < BitsPerInt32 ? typeof(Int32) : typeof(Int64),
                FieldAttributes.Private
            );
        }

        private class ProjectionPropertyFixup
        {
            public ProjectionProperty Property;
            public string             PropertyFieldName;
            public Type               PropertyAccessorType;
        }

        private static ProjectionPropertyFixup ImplementProjectionProperty(TypeBuilder typeBuilder, int index,
            ProjectionProperty propertyInfo, FieldInfo flagsField, int flagsBit)
        {
            var prefix    = GetPropertyMemberPrefix(propertyInfo, index);
            var valueType = propertyInfo.PropertyType.UnderlyingType;

            var propertyField = typeBuilder.DefineField
            (
                prefix + "Accessor",
                typeof(ProjectionProperty<>).MakeGenericType(valueType),
                FieldAttributes.Public | FieldAttributes.Static
            );

            var valueField = typeBuilder.DefineField
            (
                prefix + "Value",
                valueType,
                FieldAttributes.Private
            );

            var tryGetCachedMethod = ImplementTryGetCachedMethod(typeBuilder, prefix, valueField, flagsField, flagsBit);
            var encacheMethod      = ImplementEncacheMethod     (typeBuilder, prefix, valueField, flagsField, flagsBit);
            var decacheMethod      = ImplementDecacheMethod     (typeBuilder, prefix, valueField, flagsField, flagsBit);

            var accessorType = ImplementAccessorClass(typeBuilder, prefix, valueType, tryGetCachedMethod, encacheMethod, decacheMethod);

            var property = typeBuilder.DefineProperty
            (
                propertyInfo.Name,
                PropertyAttributes.None,
                propertyInfo.PropertyType,
                null // parameterTypes
            );

            if (propertyInfo.CanRead)
            {
                var getMethod = ImplementGetMethod(typeBuilder, index, propertyInfo, propertyField, valueField, flagsField);
                property.SetGetMethod(getMethod);
            }

            if (propertyInfo.CanWrite)
            {
                var setMethod = ImplementSetMethod(typeBuilder, index, propertyInfo, propertyField, valueField, flagsField);
                property.SetSetMethod(setMethod);
            }

            return new ProjectionPropertyFixup
            {
                Property             = propertyInfo,
                PropertyAccessorType = accessorType,
                PropertyFieldName    = propertyField.Name
            };
        }

        private static Type ImplementAccessorClass(TypeBuilder parentTypeBuilder, string prefix, Type valueType,
            MethodInfo tryGetCachedMethod, MethodInfo encacheMethod, MethodInfo decacheMethod)
        {
            var typeBuilder = parentTypeBuilder.DefineNestedType
            (
                prefix + "AccessorT",
                TypeAttributes.NestedPrivate |
                TypeAttributes.Class         |
                TypeAttributes.Sealed,
                typeof(DirectPropertyAccessor<,>).MakeGenericType(parentTypeBuilder, valueType)
            );

            var constructor = ImplementConstructor(typeBuilder);

            return null;
        }

        private static MethodBuilder ImplementGetMethod(TypeBuilder typeBuilder, int index,
            ProjectionProperty propertyInfo, FieldInfo propertyField, FieldInfo valueField, FieldInfo flagsField)
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
            EmitGetMethod(il, BaseGetMethod, propertyField, valueField, flagsField, index & 63);

            typeBuilder.DefineMethodOverride(getMethod, propertyInfo.UnderlyingGetter);
            return getMethod;
        }

        private static MethodBuilder ImplementSetMethod(TypeBuilder typeBuilder, int index,
            ProjectionProperty propertyInfo, FieldInfo propertyField, FieldInfo valueField, FieldInfo flagsField)
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
            EmitSetMethod(il, BaseSetMethod, propertyField, valueField, flagsField, index & 63, null);

            typeBuilder.DefineMethodOverride(setMethod, propertyInfo.UnderlyingSetter);
            return setMethod;
        }

        private static void EmitGetMethod(ILGenerator il, MethodInfo baseMethod,
            FieldInfo propertyField, FieldInfo valueField, FieldInfo flagsField, int flagsBit)
        {
            var value    = il.DeclareLocal(typeof(object)); // loc.0
            var uncached = il.DefineLabel();
            var done     = il.DefineLabel();

            // if (($flags & bit) == 0) goto uncached
            il.Emit(OpCodes.Ldarg_0);                       // [0] this
            EmitLoadFlagsAndMask(il, flagsField, flagsBit); // [0] $flags, [1] bit
            il.Emit(OpCodes.And);                           // [0] $flags & bit
            il.Emit(OpCodes.Brfalse_S, uncached);           // [-] if (!^) goto uncached

            // return $value
            il.Emit(OpCodes.Ldarg_0);                       // [0] this
            il.Emit(OpCodes.Ldfld, valueField);             // [0] $value
            il.Emit(OpCodes.Ret);                           // [-] return ^

            // uncached: cacheable = accessor.Property.GetValue(this, Options.Virtual, out value)

            // uncached: cacheable = property.GetValue(this, Options.Virtual, out value)
            il.MarkLabel(uncached);
            il.Emit(OpCodes.Ldsfld, propertyField);     // [0] $prop
            il.Emit(OpCodes.Ldarg_0);                   // [0] $prop, [1] this
            il.Emit(OpCodes.Ldc_I4_1);                  // [0] $prop, [1] this, [2] Options.Virtual
            il.Emit(OpCodes.Ldarga_S, value);           // [0] $prop, [1] this, [2] Options.Virtual, [3] &value
            il.Emit(OpCodes.Call, baseMethod);          // [0] cacheable

            // if (!cacheable) goto done
            il.Emit(OpCodes.Brfalse_S, done);           // [-] if (!^) goto done

            // $value = value, $flags |= bit
            EmitEncache(il, valueField, flagsField, flagsBit);

            // return value
            il.MarkLabel(done);
            il.Emit(OpCodes.Ldloc_0);                   // [0] value
            il.Emit(OpCodes.Ret);                       // [-] return ^
        }

        private static void EmitSetMethod(ILGenerator il, MethodInfo baseMethod,
            FieldInfo propertyField, FieldInfo valueField, FieldInfo flagsField, int bit, MethodInfo invalidateMethod)
        {
            var invalidate = il.DefineLabel();

            // cacheable = property.SetValue(this, value)
            il.Emit(OpCodes.Ldsfld, propertyField);     // [0] $prop
            il.Emit(OpCodes.Ldarg_0);                   // [0] $prop, [1] this
            il.Emit(OpCodes.Ldarg_1);                   // [0] $prop, [1] this, [2] value
            il.Emit(OpCodes.Call, baseMethod);          // [0] cacheable

            // if (!cacheable) goto invalidate
            il.Emit(OpCodes.Brfalse_S, invalidate);     // [-] if (!^) goto invalidate

            // $value = value, $flags |= bit
            EmitEncache(il, valueField, flagsField, bit);

            // return
            il.Emit(OpCodes.Ret);                       // [-] return

            // invalidate:
            il.MarkLabel(invalidate);

            // $value = default, $flags &= ~bit
            EmitDecache(il, valueField, flagsField, bit);

            // return
            il.Emit(OpCodes.Ret);                       // [-] return
        }

        private static void EmitEncache(ILGenerator il, FieldInfo valueField, FieldInfo flagsField, int bit)
        {
            // $value = value
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Ldloc_0);                   // [0] this, [1] value
            il.Emit(OpCodes.Stfld, valueField);         // [-] $value = ^

            // $flags |= bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [0] this, [1] this
            EmitLoadFlagsAndMask(il, flagsField, bit);  // [0] this, [1] $flags, [2] bit
            il.Emit(OpCodes.Or);                        // [0] this, [1] $flags | bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $flags = ^
        }

        private static void EmitDecache(ILGenerator il, FieldInfo valueField, FieldInfo flagsField, int bit)
        {
            // $value = default(type)
            var type = valueField.FieldType;
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldarg_0);               // [0] this
                il.Emit(OpCodes.Ldflda, valueField);    // [0] &$value
                il.Emit(OpCodes.Initobj, type);         // [-] $value = ^
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);               // [0] this
                il.Emit(OpCodes.Ldnull);                // [0] this, [1] null
                il.Emit(OpCodes.Stfld, valueField);     // [-] $value = ^
            }

            // $flags &= ~bit
            il.Emit(OpCodes.Ldarg_0);                   // [0] this
            il.Emit(OpCodes.Dup);                       // [0] this, [1] this
            EmitLoadFlagsAndMask(il, flagsField, bit);  // [0] this, [1] $flags, [2] bit
            il.Emit(OpCodes.Not);                       // [0] this, [1] $flags, [2] ~bit
            il.Emit(OpCodes.And);                       // [0] this, [1] $flags & ~bit
            il.Emit(OpCodes.Stfld, flagsField);         // [-] $flags = ^
        }

        private static MethodBuilder ImplementTryGetCachedMethod(TypeBuilder typeBuilder,
            string prefix, FieldBuilder valueField, FieldInfo flagsField, int flagsBit)
        {
            var valueType = valueField.FieldType;

            var method = typeBuilder.DefineMethod
            (
                prefix + "IsCached",
                MethodAttributes.Public | MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                typeof(bool),
                new[] { valueType.MakeByRefType() }
            );

            method.DefineParameter(1, ParameterAttributes.Out, "value");

            var il = method.GetILGenerator();
            var uncached = il.DefineLabel();

            // if (($flags & bit) == 0) goto uncached
            il.Emit(OpCodes.Ldarg_0);                       // [0] this
            EmitLoadFlagsAndMask(il, flagsField, flagsBit); // [0] $flags, [1] bit
            il.Emit(OpCodes.And);                           // [0] $flags & bit
            il.Emit(OpCodes.Brfalse_S, uncached);           // [-] if (!^) goto uncached

            // value = $value
            il.Emit(OpCodes.Ldarg_1);                       // [0] &value
            il.Emit(OpCodes.Ldarg_0);                       // [0] &value, [1] this
            il.Emit(OpCodes.Ldfld, valueField);             // [0] &value, [1] $value
            il.Emit(OpCodes.Stobj, valueType);              // [-] value = ^

            // return true
            il.Emit(OpCodes.Ldc_I4_1);                      // [0] true 
            il.Emit(OpCodes.Ret);                           // [-] return ^

            // uncached: value = default(type)
            il.MarkLabel(uncached);
            if (valueType.IsValueType)
            {
                il.Emit(OpCodes.Ldarg_1);                   // [0] &value
                il.Emit(OpCodes.Initobj, valueType);        // [-] value = default(type)
            }
            else // (type.IsReferenceType)
            {
                il.Emit(OpCodes.Ldarg_1);                   // [0] &value
                il.Emit(OpCodes.Ldnull);                    // [0] &value, [1] null
                il.Emit(OpCodes.Stobj, valueType);          // [-] value = ^
            }

            // return false
            il.Emit(OpCodes.Ldc_I4_0);                      // [0] false 
            il.Emit(OpCodes.Ret);                           // [-] return ^

            return method;
        }

        private static MethodBuilder ImplementEncacheMethod(TypeBuilder typeBuilder,
            string prefix, FieldBuilder valueField, FieldInfo flagsField, int flagsBit)
        {
            var valueType = valueField.FieldType;

            var method = typeBuilder.DefineMethod
            (
                prefix + "Encache",
                MethodAttributes.Public | MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                null, // void
                new[] { valueType }
            );

            method.DefineParameter(1, ParameterAttributes.In, "value");

            var il = method.GetILGenerator();

            // $value = value
            il.Emit(OpCodes.Ldarg_0);                       // [0] this
            il.Emit(OpCodes.Ldarg_1);                       // [0] this, [1] value
            il.Emit(OpCodes.Stfld, valueField);             // [-] $value = ^

            // $flags |= bit
            il.Emit(OpCodes.Ldarg_0);                       // [0] this
            il.Emit(OpCodes.Dup);                           // [0] this, [1] this
            EmitLoadFlagsAndMask(il, flagsField, flagsBit); // [0] this, [1] $flags, [2] bit
            il.Emit(OpCodes.Or);                            // [0] this, [1] $flags | bit
            il.Emit(OpCodes.Stfld, flagsField);             // [-] $flags = ^

            // return
            il.Emit(OpCodes.Ret);                           // [-] return

            return method;
        }

        private static MethodBuilder ImplementDecacheMethod(TypeBuilder typeBuilder,
            string prefix, FieldBuilder valueField, FieldInfo flagsField, int flagsBit)
        {
            var valueType = valueField.FieldType;

            var method = typeBuilder.DefineMethod
            (
                prefix + "Decache",
                MethodAttributes.Public | MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                null, // void
                Type.EmptyTypes
            );

            var il = method.GetILGenerator();

            // $value = default(type)
            var type = valueField.FieldType;
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldarg_0);                   // [0] this
                il.Emit(OpCodes.Ldflda, valueField);        // [0] &$value
                il.Emit(OpCodes.Initobj, type);             // [-] $value = ^
            }
            else // (type.IsReferenceType)
            {
                il.Emit(OpCodes.Ldarg_0);                   // [0] this
                il.Emit(OpCodes.Ldnull);                    // [0] this, [1] null
                il.Emit(OpCodes.Stfld, valueField);         // [-] $value = ^
            }

            // $flags &= ~bit
            il.Emit(OpCodes.Ldarg_0);                       // [0] this
            il.Emit(OpCodes.Dup);                           // [0] this, [1] this
            EmitLoadFlagsAndMask(il, flagsField, flagsBit); // [0] this, [1] $flags, [2] bit
            il.Emit(OpCodes.Not);                           // [0] this, [1] $flags, [2] ~bit
            il.Emit(OpCodes.And);                           // [0] this, [1] $flags & ~bit
            il.Emit(OpCodes.Stfld, flagsField);             // [-] $flags = ^

            // return
            il.Emit(OpCodes.Ret);                           // [-] return

            return method;
        }

        private static void EmitLoadFlagsAndMask(ILGenerator il, FieldInfo field, int bit)
        {
            il.Emit(OpCodes.Ldfld, field); // [0] $flags

            switch (bit)
            {
                case 0:  il.Emit(OpCodes.Ldc_I4_1); break;
                case 1:  il.Emit(OpCodes.Ldc_I4_2); break;
                case 2:  il.Emit(OpCodes.Ldc_I4_4); break;
                case 3:  il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if      (bit < 8)  { il.Emit(OpCodes.Ldc_I4_S, 1  << bit); break;  }
                    else if (bit < 32) { il.Emit(OpCodes.Ldc_I4,   1  << bit); break;  }
                    else               { il.Emit(OpCodes.Ldc_I8,   1L << bit); return; }
            }

            if (field.FieldType == typeof(long))
                il.Emit(OpCodes.Conv_I8);
            
            // [0] $flags, [1] mask
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

        private static void InitializeProjectionProperties(Type type, ProjectionPropertyFixup[] properties)
        {
            foreach (var item in properties)
            {
                SetStaticField(type, item.PropertyFieldName, item.Property);
                ImplementProjectionPropertyAccessor(type, item);
            }
        }

        private static void ImplementProjectionPropertyAccessor(Type type, ProjectionPropertyFixup item)
        {
            throw new NotImplementedException();
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

        private static string GetPropertyMemberPrefix(ProjectionProperty property, int index)
        {
            return string.Concat("$P", index.ToString(), ":", property.Name, ":");
        }

        private static string GetFlagsFieldName(int index)
        {
            return string.Concat("$Flags:", index.ToString());
        }

        private const string NamePrefix       = "Projector.";

        private const int
            BitsPerInt32     = sizeof(Int32) * 8,
            BitsPerInt64     = sizeof(Int64) * 8,
            NameSuffixLength = 8;

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
