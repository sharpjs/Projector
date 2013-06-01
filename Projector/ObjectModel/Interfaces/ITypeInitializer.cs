namespace Projector.ObjectModel
{
    public interface ITypeInitializer : IProjectionBehavior
    {
        void InitializeType(TypeInitializerInvocation invocation);
    }
}
