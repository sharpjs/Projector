namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    public interface ITypeCut : ITypeBlock
    {
        ITypeCut Named(string name);
        ITypeCut Named(params string[] names);

        ITypeCut NamedLike(string pattern);

        ITypeCut OfKind(TypeKind kind);
        ITypeCut OfKind(params TypeKind[] kinds);

        ITypeCut Matching(Func<ProjectionType, bool> predicate);
        ITypeCut Matching(ITypeRestriction restriction);
    }
}
