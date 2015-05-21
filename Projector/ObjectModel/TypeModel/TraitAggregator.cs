namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;

    // Algorithm for multiple inheritance of traits.
    // Each subclass provides the strategy for working with a particular metatype
    //
    internal abstract class TraitAggregator
    {
        protected TraitAggregator() { }

        public void ComputeTraits()
        {
            CollectDeclaredTraits();
            CollectInheritedTraits();
            ApplyDeferredTraits();
        }

        protected abstract void CollectDeclaredTraits();
        protected abstract void CollectInheritedTraits();
        protected abstract void ApplyDeferredTraits();
    }

    internal abstract class TraitAggregator<TMetaObject, TSourceKey> : TraitAggregator
        where TMetaObject : ProjectionMetaObject // which receives the aggregated traits
        // && TSourceKey  : unique identifier for TMetaObject from which traits are inherited
    {
        private readonly TMetaObject                  target;
        private HashSet<TSourceKey>                   generalSources;
        private Dictionary<Type, HashSet<TSourceKey>> specificSources;
        private HashSet<Type>                         suppressedTraits;
        private Dictionary<Type, SingletonTrait>      singletonTraits;
        private List<object>                          inheritableTraits;

        private static readonly object[]
            NoTraits = { };

        protected TraitAggregator(TMetaObject target)
        {
            this.target = target;
        }

        public TMetaObject Target
        {
            get { return target; }
        }

        protected void CollectDeclaredTrait(object trait)
        {
            InheritFromAttribute inheritDirective;
            SuppressAttribute    suppressDirective;

            if (trait == null)
                return; // TODO: Should we be lenient if resolver provides a null trait?
            else if (null != (inheritDirective = trait as InheritFromAttribute))
                AddInheritDirective(inheritDirective);
            else if (null != (suppressDirective = trait as SuppressAttribute))
                AddSuppressDirective(suppressDirective);
            else
                AddTrait(trait, true);
        }

        protected void CollectInheritedTraits(TMetaObject source)
        {
            var traits = source.Traits;
            object trait;

            for (var i = 0; (i = traits.FindInheritable(i, out trait)) >= 0;)
                AddTrait(trait, false);
        }

        private void AddTrait(object trait, bool declared)
        {
            var options = trait.GetTraitOptions();

            var added = options.AllowMultiple
                ? AddTraitAllowMultiple(trait, options.Inherited)
                : AddTraitAllowSingle(trait, declared);

            if (added && options.Inherited)
                AddInheritableTrait(trait);
        }

        private bool AddTraitAllowMultiple(object trait, bool inheritable)
        {
            target.ApplyTrait(trait, inheritable);
            return true;
        }

        private bool AddTraitAllowSingle(object trait, bool declared)
        {
            SingletonTrait singleton;
            var traitType = trait.GetType();

            var singletonTraits = this.singletonTraits;
            if (singletonTraits == null)
                singletonTraits = this.singletonTraits = new Dictionary<Type, SingletonTrait>();

            if (declared || !singletonTraits.TryGetValue(traitType, out singleton))
            {
                singletonTraits[traitType] = new SingletonTrait(trait, declared);
                return true;
            }
            else // !declared && singleton found
            {
                if (!singleton.Declared && singleton.Trait != trait)
                    HandleTraitConflict(traitType);
                return false;
            }
        }

        private void AddInheritableTrait(object trait)
        {
            var inheritableTraits = this.inheritableTraits;
            if (inheritableTraits == null)
                inheritableTraits = this.inheritableTraits = new List<object>();
            inheritableTraits.Add(trait);
        }

        private void AddInheritDirective(InheritFromAttribute directive)
        {
            HashSet<TSourceKey> sources;

            var traitType = directive.AttributeType;
            if (traitType == null)
            {
                sources = this.generalSources;
                if (sources == null)
                    sources = this.generalSources = new HashSet<TSourceKey>(SourceKeyComparer);
            }
            else
            {
                var specificSources = this.specificSources;
                if (specificSources == null)
                    specificSources = this.specificSources = new Dictionary<Type, HashSet<TSourceKey>>();
                if (specificSources.TryGetValue(traitType, out sources) == false)
                    specificSources[traitType] = sources = new HashSet<TSourceKey>(SourceKeyComparer);
            }

            sources.Add(GetSourceKey(directive));
        }

        private void AddSuppressDirective(SuppressAttribute directive)
        {
            var suppressedTraits = this.suppressedTraits;
            if (suppressedTraits == null)
                suppressedTraits = this.suppressedTraits = new HashSet<Type>();
            suppressedTraits.Add(directive.Type);
        }

        private bool ShouldInherit(Type traitType, TMetaObject source)
        {
            var suppressedTraits = this.suppressedTraits;
            if (suppressedTraits != null && suppressedTraits.Contains(traitType))
                return false;

            HashSet<TSourceKey> sources;
            var specificSources = this.specificSources;
            if (specificSources != null && specificSources.TryGetValue(traitType, out sources))
                return sources.Contains(GetSourceKey(source));

            sources = this.generalSources;
            return sources == null
                || sources.Contains(GetSourceKey(source));
        }

        protected sealed override void ApplyDeferredTraits()
        {
            if (singletonTraits != null)
                foreach (var item in singletonTraits.Values)
                    target.ApplyTrait(item.Trait, true); // TODO: pass inheritable
        }

        protected virtual IEqualityComparer<TSourceKey> SourceKeyComparer
        {
            get { return EqualityComparer<TSourceKey>.Default; }
        }

        protected abstract TSourceKey GetSourceKey(TMetaObject          obj);
        protected abstract TSourceKey GetSourceKey(InheritFromAttribute directive);

        protected abstract void HandleTraitConflict(Type traitType);

        private struct SingletonTrait
        {
            public readonly object Trait;
            public readonly bool   Declared;

            public SingletonTrait(object trait, bool declared)
            {
                Trait    = trait;
                Declared = declared;
            }
        }
    }
}
