namespace Projector.ObjectModel
{
    using System.Collections.Generic;
    using System.Reflection;
    using Projector.Specs;

    public interface IStandardTraitResolverConfiguration
    {
        ICollection<Assembly>  IncludedAssemblies { get; }
        ICollection<TraitSpec> IncludedSpecs      { get; }
    }
}
