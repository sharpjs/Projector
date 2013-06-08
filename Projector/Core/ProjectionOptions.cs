namespace Projector
{
    using System;

    /// <summary>
    ///   Specifies options that control the creation of projections.
    /// </summary>
    [Flags]
    public enum ProjectionOptions
    {
        /// <summary>
        ///   Default value, with no options specified.
        /// </summary>
        None = 0,

        /// <summary>
        ///   Allow generated assemblies to be saved.
        /// </summary>
        SaveAssemblies = 0x1,

        /// <summary>
        ///   Allow generated assemblies to be garbage collected.
        /// </summary>
        CollectAssemblies = 0x2
    }

    internal static class ProjectionOptionsInternal
    {
        public const ProjectionOptions
            AssemblyModes
                = ProjectionOptions.SaveAssemblies
                | ProjectionOptions.CollectAssemblies
                ;
    }

    internal static class ProjectionOptionsExtensions
    {
        public static bool Any(this ProjectionOptions options, ProjectionOptions flag)
        {
            return 0 != (options & flag);
        }

        public static ProjectionOptions Set(this ProjectionOptions options, ProjectionOptions flag, bool value)
        {
            return value
                ? options |  flag
                : options & ~flag;
        }
    }
}
