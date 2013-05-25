namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class PropertyCutTests : ProjectionTestsBase
    {
        private interface IStructure
        {
            IAny        AnyProperty   { get; set; }
            IAny[]      ArrayProperty { get; set; }
            IList<IAny> ListProperty  { get; set; }
            IAny        OtherProperty { get; set; }

            IStructure  TestPropertyA { get; set; }
            string      TestPropertyB { get; set; }
        }

        private PropertyCut Cut { get; set; }

        [SetUp]
        public void SetUp()
        {
            Cut = new PropertyCut();
        }

        [Test]
        public void Initially()
        {
            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.AnyProperty)), Is.True);
        }

        [Test]
        public void OfKind_Single()
        {
            Cut.OfKind(TypeKind.Array);

            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.ArrayProperty)), Is.True );
            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.OtherProperty)), Is.False);
        }

        [Test]
        public void OfKind_Multiple()
        {
            Cut.OfKind(TypeKind.Array, TypeKind.List);

            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.ArrayProperty)), Is.True );
            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.ListProperty )), Is.True );
            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.OtherProperty)), Is.False);
        }

        [Test]
        public void OfKind_Multiple_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Cut.OfKind(null as TypeKind[])
            )
            .ForParameter("kinds");
        }

        [Test]
        public void Matching_Predicate()
        {
            Cut.Matching(t => t.Name.StartsWith("Test"));

            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.TestPropertyA)), Is.True );
            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.OtherProperty)), Is.False);
        }

        [Test]
        public void Matching_Predicate_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Cut.Matching(null as Func<ProjectionProperty, bool>)
            )
            .ForParameter("predicate");
        }

        [Test]
        public void Matching_Restriction()
        {
            Cut.Matching(new IsTestPropertyRestriction());

            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.TestPropertyA)), Is.True );
            Assert.That(Cut.AppliesTo(PropertyOf<IStructure>(o => o.OtherProperty)), Is.False);
        }

        [Test]
        public void Matching_Restriction_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Cut.Matching(null as IPropertyRestriction)
            )
            .ForParameter("restriction");
        }

        private class IsTestPropertyRestriction : IPropertyRestriction
        {
            public bool AppliesTo(ProjectionProperty property)
            {
                return property.Name.StartsWith("Test");
            }
        }
    }
}
