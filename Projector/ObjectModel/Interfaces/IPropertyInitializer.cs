namespace Projector.ObjectModel
{
    public interface IPropertyInitializer : IProjectionBehavior
    {
        void     InitializeProperty(    PropertyInitializerInvocation invocation);
        void PostInitializeProperty(PropertyPostInitializerInvocation invocation);
    }
}
