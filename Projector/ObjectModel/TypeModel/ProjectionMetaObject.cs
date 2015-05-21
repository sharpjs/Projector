namespace Projector.ObjectModel
{
    public abstract class ProjectionMetaObject : ProjectionObject
    {
        private readonly TraitCollection    traits;
        private readonly BehaviorCollection behaviors;
        private bool                        frozen;

        // Constructor: Set readonlies, get other metaobjects
        internal ProjectionMetaObject()
        {
            traits    = new TraitCollection();
            behaviors = new BehaviorCollection();
        }

        // Initialization Pass 1
        internal abstract void ComputeTraits();

        // Initialization Pass 2
        internal virtual void FreezeTraits() { frozen = true; }

        // Initialization Pass 3
        internal abstract void InvokeInitializers();

        // Initialization Pass 4
        internal abstract void InvokePostInitializers();

        public TraitCollection Traits
        {
            get { return traits; }
        }

        public BehaviorCollection Behaviors
        {
            get { return behaviors; }
        }

        internal void ApplyTrait(object trait, bool inheritable)
        {
            if (frozen)
                throw Error.TraitsReadOnly();

            traits.AddInternal(trait, inheritable);

            var behavior = trait as IProjectionBehavior;
            if (behavior != null)
                ApplyBehavior(behavior);

            var builder = trait as ITraitBuilder;
            if (builder != null)
                ApplyTraitBuilder(builder);
        }

        protected virtual void ApplyBehavior(IProjectionBehavior behavior)
        {
            behaviors.AddInternal(behavior);
        }

        protected abstract void ApplyTraitBuilder(ITraitBuilder builder);
    }
}
