namespace Projector.ObjectModel
{
    using System;
    using System.Collections;

    internal static class TraitOptions
    {
        private static readonly Hashtable
            Options = new Hashtable();

        private static readonly object
            OptionsLock = new object();

        private static readonly ITraitOptions
            Default = new DefaultValues();

        internal static ITraitOptions GetTraitOptions(this object trait)
        {
            var options = trait as ITraitOptions;
            if (options != null)
                return options;

            var attribute = trait as Attribute;
            if (attribute != null)
                return attribute.GetTraitOptions();

            return TraitOptions.Default;
        }

        private static ITraitOptions GetTraitOptions(this Attribute attribute)
        {
            var type = attribute.GetType();
            ITraitOptions options;

            if (null != (options = Options[type] as ITraitOptions))
                return options;

            var newOptions = GetTraitOptionsFromAttributeUsage(type);

            lock (OptionsLock)
            {
                if (null != (options = Options[type] as ITraitOptions))
                    return options;

                Options[type] = newOptions;
                return newOptions;
            }
        }

        private static ITraitOptions GetTraitOptionsFromAttributeUsage(this Type type)
        {
            // Assume type is subclass of Attribute.
            // Therefore, type is guaranteed to have exactly one AttributeUsageAttribute.

            var attributes = type.GetCustomAttributes(typeof(AttributeUsageAttribute), true);
            return new AttributeUsageWrapper(attributes[0] as AttributeUsageAttribute);
        }

        private sealed class AttributeUsageWrapper : ITraitOptions
        {
            private readonly AttributeUsageAttribute usage;

            public AttributeUsageWrapper(AttributeUsageAttribute usage)
            {
                this.usage = usage;
            }

            public AttributeTargets ValidOn
            {
                get { return usage.ValidOn; }
            }

            public bool AllowMultiple
            {
                get { return usage.AllowMultiple; }
            }

            public bool Inherited
            {
                get { return usage.Inherited; }
            }
        }

        private sealed class DefaultValues : ITraitOptions
        {
            public AttributeTargets ValidOn
            {
                get { return AttributeTargets.All; }
            }

            public bool AllowMultiple
            {
                get { return false; }
            }

            public bool Inherited
            {
                get { return true; }
            }
        }
    }
}
