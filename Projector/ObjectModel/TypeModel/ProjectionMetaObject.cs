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

        // Initialization Pass 1
        internal virtual void ComputeTraits()
        {
            // Do nothing
        }

        // Initialization Pass 2
        internal virtual void FreezeTraits()
        {
            isReadOnly = true;
        }

        // Initialization Pass 3
        internal virtual void InvokeInitializers()
        {
            // Do nothing
        }

        // Initialization Pass 4
        internal virtual void InvokeLateInitializers()
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
