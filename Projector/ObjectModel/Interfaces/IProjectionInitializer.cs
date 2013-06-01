namespace Projector.ObjectModel
{
    public interface IProjectionInitializer : IProjectionBehavior
    {
        void InitializeProjection(ProjectionInitializerInvocation invocation);
    }
}
