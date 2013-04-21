namespace Projector.ObjectModel
{
    using System;

    /// <summary>
    ///   Specifies the usage of a trait object.
    /// </summary>
    /// <remarks>
    ///   This interface is the trait equivalent to <see cref="AttributeUsageAttribute"/>.
    /// </remarks>
    public interface ITraitOptions
    {
        /// <summary>
        ///   Gets a value indicating the kinds of targets to which the trait can be applied.
        /// </summary>
        /// <value>
        ///    A combination of <see cref="System.AttributeTargets"/> values. The default is <c>All</c>.
        /// </value>
        /// <remarks>
        ///   This property is equivalent to <see cref="AttributeUsageAttribute.ValidOn"/>.
        /// </remarks>
        AttributeTargets ValidOn { get; }

        /// <summary>
        ///   Gets a value indicating whether more than one instance of the trait can be applied
        ///   to a single target.
        /// </summary>
        /// <value>
        ///   <c>true</c> if more than one instance of the trait can be applied to a single target;
        ///   otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        /// <remarks>
        ///   This property is equivalent to <see cref="AttributeUsageAttribute.AllowMultiple"/>.
        /// </remarks>
        bool AllowMultiple { get; }

        /// <summary>
        ///   Gets a value indicating whether the trait is inherited by derived types and
        ///   overriding members.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the trait is inherited by derived classes and overriding members;
        ///   otherwise, <c>false</c>. The default is <c>true</c>.
        /// </value>
        /// <remarks>
        ///   This property is equivalent to <see cref="AttributeUsageAttribute.Inherited"/>.
        /// </remarks>
        bool Inherited { get; }
    }
}
