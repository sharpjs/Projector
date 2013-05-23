namespace Projector.Specs
{
    using System;
    using System.Reflection;
    using Projector.ObjectModel;

    public abstract class TraitSpec
    {
        internal TraitSpec() { }

        internal static TraitSpec CreateInstance(Type type)
        {
            TraitSpec spec;

            try
            {
                spec = (TraitSpec) Activator.CreateInstance(type);
            }
            catch (TargetInvocationException e)
            {
                // Constructor threw an exception
                throw Error.TraitSpecCreateFailed(type, e.InnerException ?? e);
            }
            catch (Exception e)
            {
                // Some other problem (visibility, security, etc.)
                throw Error.TraitSpecCreateFailed(type, e);
            }

            spec.Build();
            return spec;
        }

        internal abstract void Build();

        internal abstract void ProvideScopes
        (
            ProjectionType       projectionType,
            Type                 underlyingType,
            ITypeScopeAggregator action
        );
    }
}
