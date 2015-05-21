namespace Projector.ObjectModel
{
    public interface ITypeBehavior
    {
        void InitializeInstance  (InstanceInitializerInvocation invocation);
        void InitializeProjection(InstanceInitializerInvocation invocation);
    }
}
