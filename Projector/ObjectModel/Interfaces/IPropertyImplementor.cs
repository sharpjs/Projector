namespace Projector.ObjectModel
{
    using System;
    using System.Reflection.Emit;

    public interface IPropertyImplementor
    {
        void     ImplementProperty(ProjectionProperty property, TypeBuilder implementationType);
        void PostImplementProperty(ProjectionProperty property, Type        implementationType);
    }
}
