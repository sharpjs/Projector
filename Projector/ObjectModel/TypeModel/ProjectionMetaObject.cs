namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;

    public abstract class ProjectionMetaObject : ProjectionObject
    {
        private readonly AnnotationSet annotations;
        private readonly BehaviorSet   behaviors;
        private          bool          isReadOnly;
        private          object[]      inheritableTraits;

        // Constructor: Set readonlies, get other metaobjects
        internal ProjectionMetaObject()
        {
            annotations = new AnnotationSet();
            behaviors   = new BehaviorSet  ();
        }

        // Initialization Pass 1
        internal virtual void ComputeTraits()
        {
            var aggregator = CreateTraitAggregator();

            aggregator.CollectDeclaredTraits();
            aggregator.CollectInheritedTraits();
            aggregator.ApplyDeferredTraits();

            inheritableTraits = aggregator.InheritableTraits;
        }

        internal abstract TraitAggregator CreateTraitAggregator();

        // Initialization Pass 2
        internal virtual void FreezeTraits()
        {
            isReadOnly = true;
        }

        // Initialization Pass 3
        internal abstract void InvokeInitializers();

        // Initialization Pass 4
        internal abstract void InvokeLateInitializers();

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

        // Traits inherited by derived types
        internal object[] InheritableTraits
        {
            get { return inheritableTraits; }
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
