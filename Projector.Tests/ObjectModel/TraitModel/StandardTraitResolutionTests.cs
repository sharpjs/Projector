namespace Projector.ObjectModel.StandardTraitResolutionTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Projector.Specs;

        //[Test]
        //public void Resolve_NotDetected()
        //{
        //    var resolution = Resolve<Fakes.WithoutTraitSpecs.ITypeA>();

        //    Assert.That(resolution, Is.Not.Null);

        //    Assert.That(resolution.ProjectionType, Is.SameAs(TypeOf<Fakes.WithTraitSpecs.ITypeA>()));
        //    Assert.That(resolution.UnderlyingType, Is.SameAs(typeof(Fakes.WithTraitSpecs.ITypeA)));
        //}

        //[Test]
        //public void Resolve_DetectedInContainingAssembly()
        //{
        //    var resolution = Resolve<Fakes.WithTraitSpecs.ITypeA>();

        //    Assert.That(resolution, Is.Not.Null);
        //}

    //[TestFixture]
    //public class Empty : ResolutionCase
    //{
    //    [Test]
    //    public void TypeTraits()
    //    {
    //        Assert.That(GetTypeTraits(), Is.Empty);
    //    }

    //    [Test]
    //    public void PropertyTraits()
    //    {
    //        Assert.That(GetPropertyATraits(), Is.Empty);
    //    }
    //}

    //[TestFixture]
    //public class Included_TypeSpec_TypeTraits : ResolutionCase
    //{
    //    public static readonly object
    //        TraitA = new object();

    //    public class SpecA : TypeTraitSpec<ITypeA>
    //    {
    //        public SpecA() { Apply(TraitA); }
    //    }

    //    protected override void Configure(StandardTraitResolverConfiguration configuration)
    //    {
    //        configuration.IncludeSpec<SpecA>();
    //    }

    //    [Test]
    //    public void TypeTraits()
    //    {
    //        Assert.That(GetTypeTraits(), HasTraits(TraitA));
    //    }
    //}

    //public abstract class ResolutionCase
    //{
    //    private ITraitResolution resolution;

    //    protected virtual void Configure(StandardTraitResolverConfiguration configuration) { }

    //    [SetUp]
    //    public void SetUp()
    //    {
    //        resolution = new StandardTraitResolver(Configure)
    //            .Resolve(TypeOf<ITypeA>(), typeof(ITypeA));
    //    }

    //    protected List<object> GetTypeTraits()
    //    {
    //        var aggregator = new FakeTraitAggregator();
    //        resolution.ProvideTypeTraits(aggregator);
    //        return aggregator.Traits;
    //    }

    //    protected List<object> GetPropertyATraits()
    //    {
    //        return GetPropertyTraits("PropertyA");
    //    }

    //    protected List<object> GetPropertyBTraits()
    //    {
    //        return GetPropertyTraits("PropertyB");
    //    }

    //    private List<object> GetPropertyTraits(string name)
    //    {
    //        var projectionProperty = PropertyOf<ITypeA>(name);
    //        var underlyingProperty = typeof(ITypeA).GetProperty(name);
    //        var aggregator         = new FakeTraitAggregator();
    //        resolution.ProvidePropertyTraits(projectionProperty, underlyingProperty, aggregator);
    //        return aggregator.Traits;
    //    }

    //    protected static Constraint HasTraits(params object[] traits)
    //    {
    //        return Is.EqualTo(traits);
    //    }
    //}
}
