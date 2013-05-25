namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    public interface IPropertyCut : IPropertyScope
    {
        IPropertyCut OfKind(TypeKind kind);
        IPropertyCut OfKind(params TypeKind[] kinds);

        IPropertyCut Named(string name);
        IPropertyCut Named(params string[] names);

        IPropertyCut Matching(Func<ProjectionProperty, bool> predicate);
        IPropertyCut Matching(IPropertyRestriction restriction);
    }
}
