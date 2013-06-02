namespace Projector
{
    using System;

    /// <summary>
    ///   Specifies options that control the creation of projections.
    /// </summary>
    [Flags]
    internal enum ProjectionOptions
    {
        /// <summary>
        ///   Default value, with no options specified.
        /// </summary>
        None = 0,

        /// <summary>
        ///   Allow generated assemblies to be saved.
        /// </summary>
        SaveAssembly = 0x1,

        /// <summary>
        ///   Allow generated assemblies to be garbage collected.
        /// </summary>
        CollectAssembly = 0x2,

        /// <summary>
        ///   Enable derived projection types to inherit attributes of base types.
        ///   An attribute is inherited if its associated
        ///   <c>AttributeUsageAttribute</c> has <c>Inherited</c> = <c>true</c>.
        /// </summary>
        InheritAttributes = 0x4
    }

    internal static class ProjectionOptionsInternal
    {
        public const ProjectionOptions
            AssemblyModes
                = ProjectionOptions.SaveAssembly
                | ProjectionOptions.CollectAssembly
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
