namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;

    public abstract class ProjectionMetaObject : ProjectionObject
    {
        private readonly AnnotationSet annotations;
        private readonly BehaviorSet   behaviors;
        private          bool          isReadOnly;

        // Constructor: Set readonlies, get other metaobjects
        internal ProjectionMetaObject()
        {
            annotations = new AnnotationSet();
            behaviors   = new BehaviorSet  ();
        }

        // Init pass 1: Compute traits
        internal virtual void InitializePass1()
        {
            // Do nothing
        }

        // Init pass 2: Freeze traits
        internal void InitializePass2()
        {
            isReadOnly = true;
        }

        // Init pass 3: Invoke initializers
        internal virtual void InitializePass3()
        {
            // Do nothing
        }

        // Init pass 3: Invoke late initializers
        internal virtual void InitializePass4()
        {
            // Do nothing
        }

        public IEnumerable<object> Annotations
        {
            get { return annotations; }
        }
 
        internal Cell<object> FirstAnnotation
        {
            get { return annotations.First; }
        }
 
        public IEnumerable<IProjectionBehavior> Behaviors
        {
            get { return behaviors; }
        }
 
        internal Cell<IProjectionBehavior> FirstBehavior
        {
            get { return behaviors.First; }
        }

        internal void Apply(object trait)
        {
            if (isReadOnly)
                throw Error.ReadOnlyTraits();

            var builder = trait as ITraitBuilder;
            if (builder == null)
            {
                var behavior = trait as IProjectionBehavior;
                if (behavior == null)
                    annotations.Apply(trait);
                else
                    behaviors.Apply(behavior);
            }
            else
            {
                builder.ApplyTraits(this, new TraitApplicator(this));
            }
        }
    }
}
