namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;

    // Algorithm for multiple inheritance of traits.
    // Each subclass provides the strategy for working with a particular metatype
    //
    internal abstract class TraitAggregator<TMetaObject, TSourceKey>
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

        public void CollectDeclaredTraits()
        {
            foreach (var trait in GetDeclaredTraits(target))
                if (trait != null && ShouldCollect(trait))
                    AddTrait(trait, true);
        }

        public void CollectInheritedTraits()
        {
            foreach (var source in GetInheritanceSources(target))
            foreach (var trait  in GetInheritableTraits (source))
                if (trait != null && ShouldInherit(trait.GetType(), source))
                    AddTrait(trait, false);
        }

        public void ApplyDeferredTraits()
        {
            if (singletonTraits != null)
                foreach (var item in singletonTraits.Values)
                    target.Apply(item.Trait);
        }

        public object[] InheritableTraits
        {
            get { return inheritableTraits == null ? NoTraits : inheritableTraits.ToArray(); }
        }

        private bool ShouldCollect(object trait)
        {
            InheritFromAttribute inheritDirective;
            SuppressAttribute    suppressDirective;

            if (null != (inheritDirective = trait as InheritFromAttribute))
            {
                AddInheritDirective(inheritDirective);
                return false;
            }

            if (null != (suppressDirective = trait as SuppressAttribute))
            {
                AddSuppressDirective(suppressDirective);
                return false;
            }

            return true;
        }

        private void AddTrait(object trait, bool declared)
        {
            var options = trait.GetTraitOptions();

            var added = options.AllowMultiple
                ? AddTraitAllowMultiple(trait)
                : AddTraitAllowSingle(trait, declared);

            if (added && options.Inherited)
                AddInheritableTrait(trait);
        }

        private bool AddTraitAllowMultiple(object trait)
        {
            target.Apply(trait);
            return true;
        }

        private bool AddTraitAllowSingle(object trait, bool declared)
        {
            SingletonTrait singleton;
            var traitType = trait.GetType();

            if (singletonTraits == null)
                singletonTraits = new Dictionary<Type, SingletonTrait>();

            if (declared || !singletonTraits.TryGetValue(traitType, out singleton))
            {
                singletonTraits[traitType] = new SingletonTrait(trait, declared);
                return true;
            }
            else
            {
                if (!(singleton.Declared || singleton.Trait == trait))
                    HandleTraitConflict(traitType);
                return false;
            }
        }

        private void AddInheritableTrait(object trait)
        {
            if (inheritableTraits == null)
                inheritableTraits = new List<object>();
            inheritableTraits.Add(trait);
        }

        private void AddInheritDirective(InheritFromAttribute directive)
        {
            var traitType = directive.AttributeType;
            if (traitType == null)
            {
                if (generalSources == null)
                    generalSources = new HashSet<TSourceKey>(SourceKeyComparer);
                generalSources.Add(GetSourceKey(directive));
            }
            else
            {
                HashSet<TSourceKey> sources;
                if (specificSources == null)
                    specificSources = new Dictionary<Type, HashSet<TSourceKey>>();
                if (specificSources.TryGetValue(traitType, out sources) == false)
                    specificSources[traitType] = sources = new HashSet<TSourceKey>(SourceKeyComparer);
                sources.Add(GetSourceKey(directive));
            }
        }

        private void AddSuppressDirective(SuppressAttribute directive)
        {
            if (suppressedTraits == null)
                suppressedTraits = new HashSet<Type>();
            suppressedTraits.Add(directive.Type);
        }

        private bool ShouldInherit(Type traitType, TMetaObject source)
        {
            if (suppressedTraits != null && suppressedTraits.Contains(traitType))
                return false;

            HashSet<TSourceKey> sources;
            return (specificSources != null && specificSources.TryGetValue(traitType, out sources))
                ? sources.Contains(GetSourceKey(source))
                : generalSources == null || generalSources.Contains(GetSourceKey(source));
        }

        protected virtual IEqualityComparer<TSourceKey> SourceKeyComparer
        {
            get { return EqualityComparer<TSourceKey>.Default; }
        }

        protected abstract object[]                 GetDeclaredTraits    (TMetaObject obj);
        protected abstract object[]                 GetInheritableTraits (TMetaObject obj);
        protected abstract IEnumerable<TMetaObject> GetInheritanceSources(TMetaObject obj);

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
