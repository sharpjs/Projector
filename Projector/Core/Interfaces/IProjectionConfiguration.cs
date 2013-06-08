namespace Projector
{
    using System.Collections.Generic;
    using Projector.ObjectModel;

    public interface IProjectionConfiguration
    {
        ProjectionOptions Options { get; }

        ITraitResolver TraitResolver { get; }

        int? MaxTypesPerAssembly { get; }
    }
}
