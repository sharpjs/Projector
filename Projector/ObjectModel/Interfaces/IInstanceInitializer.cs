namespace Projector.ObjectModel
{
    public interface IInstanceInitializer : IProjectionBehavior
    {
        void InitializeInstance(InstanceInitializerInvocation invocation);
    }
}
