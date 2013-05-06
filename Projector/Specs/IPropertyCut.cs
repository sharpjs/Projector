namespace Projector.Specs
{
    using System;
    using System.Reflection;
    using Projector.ObjectModel;

    public interface IPropertyCut
    {
        IPropertyCut OfKind(TypeKind kind);
        IPropertyCut OfKind(params TypeKind[] kinds);

        IPropertyCut Named(string name);
        IPropertyCut Named(params string[] names);

        IPropertyCut Matching(Func<PropertyInfo, bool> predicate);
    }
}
