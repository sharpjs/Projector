﻿namespace Projector
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Globalization;
    using Projector.ObjectModel;
    using Projector.Specs;

    internal static class Error
    {
        internal static Exception ArgumentNull(string name)
        {
            return new ArgumentNullException(name);
        }

        internal static Exception ArgumentOutOfRange(string name)
        {
            return new ArgumentOutOfRangeException(name);
        }

        //internal static Exception ArgumentArrayTooSmall(string name)
        //{
        //    var message = "Destination array is too small to copy the specified items.";
        //    throw new ArgumentException(message, name);
        //}

        //internal static Exception InvalidOperation()
        //{
        //    throw new InvalidOperationException();
        //}

        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        internal static Exception ReadOnlyCollection()
        {
            return new NotSupportedException("The collection is read-only.");
        }

        internal static Exception EnumeratorNoCurrentItem()
        {
            return new InvalidOperationException("The enumerator has no current item.");
        }

        internal static Exception ReadOnlyTraits()
        {
            return new InvalidOperationException("Traits for the object are frozen.");
        }

        internal static Exception SaveAndCollectAssembliesNotSupported()
        {
            const string message
                = "Saving and garbage collection of dynamic assemblies cannot be enabled simultaneously.";

            return new NotSupportedException(message);
        }

        internal static Exception InvalidProjectionType(Type type)
        {
            var message = string.Format
            (
                "Invalid projection type: {0}",
                type
            );
            return new ProjectionException(message);
        }

        internal static Exception UnsupportedCollectionType(Type type)
        {
            var message = string.Format
            (
                "Unsupported collection type: {0}",
                type.GetPrettyName(true)
            );
            return new NotSupportedException(message);
        }

        internal static Exception NotPropertyExpression(string name)
        {
            return new ArgumentOutOfRangeException(name, "The argument was not a property expression.");
        }

        internal static Exception TraitSpecCreateFailed(Type type, Exception innerException)
        {
            var message = string.Format
            (
                "Failed to create trait spec instance: {0}",
                type.GetPrettyName(true)
            );
            return new TraitSpecException(message, innerException);
        }

        internal static Exception TraitFactoryReturnedNull()
        {
            return new TraitSpecException("Trait factory returned null.");
        }

        // TODO: 'trait' instead of 'attribute'?
        internal static Exception AttributeConflict(ProjectionType projectionType, Type attributeType)
        {
            var message = string.Format
            (
                "Conflicting '{0}' attributes inherited by type {1}.  " +
                "Override the attribute, or use inheritance directives to resolve the conflict.",
                attributeType.GetPrettyName(false).RemoveSuffix(AttributeSuffix),
                projectionType.UnderlyingType.GetPrettyName(true)
            );

            return new ProjectionException(message);
        }

        // TODO: 'trait' instead of 'attribute'?
        internal static Exception AttributeConflict(ProjectionProperty property, Type attributeType)
        {
            var message = string.Format
            (
                "Conflicting '{0}' attributes inherited by property {1}.{2}.  " +
                "Override the attribute, or use inheritance directives to resolve the conflict.",
                attributeType.GetPrettyName(false).RemoveSuffix(AttributeSuffix),
                property.DeclaringType.UnderlyingType.GetPrettyName(true),
                property.Name
            );

            return new ProjectionException(message);
        }

        // TODO: 'trait' instead of 'attribute'?
        internal static Exception InvalidOverride(string name, Type declaringType, ProjectionProperty newProperty)
        {
            var message = string.Format
            (
                "Invalid 'Override' attribute on property {0}.{1}.  " +
                "The specified base property was not found: {2}.{3}.",
                newProperty.DeclaringType.UnderlyingType.GetPrettyName(true),
                newProperty.Name,
                declaringType.GetPrettyName(true),
                name
            );

            return new ProjectionException(message);
        }

        internal static Exception AssociatedObjectNotFound(object key, Type type, ProjectionObject projectionObject)
        {
            var message = string.Format
            (
                "Associated object not found for key '{0}' in {1}.  Expected type: {2}",
                key,
                projectionObject.ToString(),
                type.GetPrettyName(qualified: true)
            );

            return new KeyNotFoundException(message);
        }

        //internal static Exception NoStorageProvider()
        //{
        //    var message = string.Format
        //    (
        //        ""
        //    );

        //    return new NotSupportedException(message);
        //}

        //internal static Exception StoreNotFound(Type type)
        //{
        //    var message = string.Format
        //    (
        //        "Projection storage type not found: {0}.",
        //        type.GetPrettyName(true)
        //    );

        //    return new KeyNotFoundException(message);
        //}

        //internal static Exception DuplicateReferenceId(int id, string node)
        //{
        //    var message = string.Format
        //    (
        //        "Duplicate reference ID {0} at node '{1}'.",
        //        id,
        //        node
        //    );

        //    return new ProjectionException(message);
        //}

        //internal static Exception ReferenceIdNotFound(int id, string node)
        //{
        //    var message = string.Format
        //    (
        //        "Reference ID {0} not found at node '{1}'.",
        //        id,
        //        node
        //    );

        //    return new ProjectionException(message);
        //}

        //internal static Exception NoStorageProviders()
        //{
        //    throw new NotImplementedException();
        //}

        internal static Exception InternalError(string description)
        {
            var message = string.Format
            (
                "An internal error occurred in Projector.  " +
                "This is probably a bug.  " +
                "Please report this incident to the developer.  " +
                "Description: '{0}'",
                description
            );

            return new InvalidOperationException(message);
        }

        internal static Exception TodoError()
        {
            return new NotImplementedException("Work in progress.");
        }

        private const string
            AttributeSuffix = "Attribute";
    }
}
