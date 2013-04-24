namespace Projector.ObjectModel
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   Base class for projections and and metaobjects.
    ///   Provides a facility to extend objects with arbitrary associated objects.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Built-in behaviors, custom behaviors, and application code
    ///       can use associated objects to add arbitrary extended information
    ///       to projections, projection instances, and projection metaobjects.
    ///     
    ///     Any instance of a reference type can serve as an associated object.
    ///     Associated objects are identified by keys.  Any object can be a key.
    ///     
    ///     To associate an object, call <see cref="SetAssociatedObject{T}"/>.
    ///     To retrieve an associated object, call <see cref="GetAssociatedObject{T}"/>
    ///       or <see cref="TryGetAssociatedObject{T}"/>.
    ///     To retrieve all associated objects for an object,
    ///       use the <see cref="AssociatedObjects"/> property.
    ///       
    ///     Setting and retrieving associated objects is thread-safe,
    ///     but enumerating them is not thread-safe.
    ///   </para>
    /// </remarks>
    /// <threadsafety static="true" insteance="true"/>
    public abstract class ProjectionObject
    {
        // Why Hashtable?
        // - Unlike generic dictionary, it supports single-reader/multi-writer concurrency
        // - We actually DO want to store any type of object.
        //
        // Why no remove?
        // - http://blogs.msdn.com/b/brada/archive/2003/04/13/49969.aspx
        // - "The document says Hashtable is thread-safe for a single writer and concurrent readers,
        //      but unfortunately the current implementation doesn’t completely hold that up."
        // - "Remove and Clear are the only functions which will free a bucket."
        // - "You can get a value which belongs to another key."
        //
        private Hashtable associations;

        internal ProjectionObject() { }

        /// <summary>
        ///   Gets the factory that created this object.
        /// </summary>
        public abstract ProjectionFactory Factory { get; }

        /// <summary>
        ///   Gets the objects associated with this object.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This property is thread-safe,
        ///     but enumerating the associated objects is NOT thread-safe.
        ///   </para>
        /// </remarks>
        public IEnumerable<object> AssociatedObjects
        {
            get
            {
                var objects = associations;
                if (objects == null)
                    yield break;
                foreach (DictionaryEntry entry in objects)
                    if (entry.Value != null)
                        yield return entry.Value;
            }
        }

        /// <summary>
        ///   Determines whether this object has an associated object
        ///   with the specified key and type.
        /// </summary>
        /// <typeparam name="T">The type of the associated object.</typeparam>
        /// <param name="key">The key identifying the associated object.</param>
        /// <returns>
        ///   <c>true</c> if an object of type <typeparamref name="T"/>
        ///     identified by <paramref name="key"/> is associated with this object;
        ///   <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     This method is thread-safe.
        ///   </para>
        /// </remarks>
        public bool HasAssociatedObject<T>(object key)
            where T : class
        {
            T value;
            return TryGetAssociatedObject(key, out value);
        }

        /// <summary>
        ///   Gets the associated object with the specified key and type.
        /// </summary>
        /// <typeparam name="T">The type of the associated object.</typeparam>
        /// <param name="key">The key identifying the associated object.</param>
        /// <returns>
        ///   The object of type <typeparamref name="T"/>
        ///   identified by <paramref name="key"/> that is associated with this object.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        ///   No object identified by <paramref name="key"/> is associated with this object, or
        ///   the associated object is not of type <typeparamref name="T"/>.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     This method is thread-safe.
        ///   </para>
        /// </remarks>
        public T GetAssociatedObject<T>(object key)
            where T : class
        {
            T value;
            if (TryGetAssociatedObject(key, out value))
                return value;

            throw Error.AssociatedObjectNotFound(key, typeof(T), this);
        }

        /// <summary>
        ///   Gets the associated object with the specified key and type.
        /// </summary>
        /// <typeparam name="T">The type of the associated object.</typeparam>
        /// <param name="key">The key identifying the associated object.</param>
        /// <param name="obj">
        ///   When this method returns, contains the object of type <typeparamref name="T"/>
        ///   identified by <paramref name="key"/> that is associated with this object,
        ///   if such an object is found; otherwise, the default value of type <typeparamref name="T"/>.
        ///   This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///   <c>true</c> if an object of type <typeparamref name="T"/>
        ///     identified by <paramref name="key"/> is associated with this object;
        ///   <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     This method is thread-safe.
        ///   </para>
        /// </remarks>
        public virtual bool TryGetAssociatedObject<T>(object key, out T obj)
            where T : class
        {
            if (key == null)
                throw Error.ArgumentNull("key");

            var objects = associations;

            if (objects == null)
            {
                obj = default(T);
                return false;
            }

            return null !=
            (
                obj = objects[key] as T
            );
        }

        /// <summary>
        ///   Sets or clears the associated object with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the associated object.</typeparam>
        /// <param name="key">The key identifying the associated object.</param>
        /// <param name="obj">
        ///   The object to associate, or
        ///   <c>null</c> to clear (disassociate) the the object
        ///     identified by <paramref name="key"/>, if any.
        /// </param>
        /// <returns>
        ///   <c>true</c>  if the associated object was set;
        ///   <c>false</c> if the associated object was cleared.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     This method is thread-safe.
        ///   </para>
        /// </remarks>
        public virtual bool SetAssociatedObject<T>(object key, T obj)
            where T : class
        {
            if (key == null)
                throw Error.ArgumentNull("key");

            var objects = associations;

            if (obj != null)
            {
                if (objects == null)
                    objects = Concurrent.Ensure(ref associations, new Hashtable());
                objects[key] = obj;
                return true;
            }

            if (objects != null)
                objects[key] = null;

            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return GetType().GetPrettyName(qualified: false);
        }
    }
}
