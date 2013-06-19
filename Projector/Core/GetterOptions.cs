namespace Projector //.ObjectModel
{
    /// <summary>
    ///   Specifies options for the invocation of projection members.
    /// </summary>
    public enum GetterOptions
    {
        /// <summary>
        ///   Default value, with no options specified.
        /// </summary>
        None = 0,

        /// <summary>
        ///   Return a virtual object instead of null, if the return type is virtualizable.
        /// </summary>
        Virtual = 1 << 0,

        /// <summary>
        ///   Force invocation of behaviors and storage, even if a value is cached.
        /// </summary>
        Uncached = 1 << 1
    }
}
