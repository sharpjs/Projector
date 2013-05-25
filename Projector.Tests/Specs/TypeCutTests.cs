namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class TypeCutTests : ProjectionTestsBase
    {
        private TypeCut Cut { get; set; }

        [SetUp]
        public void SetUp()
        {
            Cut = new TypeCut();
        }

        [Test]
        public void Initially()
        {
            Assert.That(Cut.AppliesTo(TypeOf<IAny>()), Is.True);
        }

        [Test]
        public void OfKind_Single()
        {
            Cut.OfKind(TypeKind.Array);

            Assert.That(Cut.AppliesTo(TypeOf<IAny[]>()), Is.True );
            Assert.That(Cut.AppliesTo(TypeOf<IAny  >()), Is.False);
        }

        [Test]
        public void OfKind_Multiple()
        {
            Cut.OfKind(TypeKind.Array, TypeKind.List);

            Assert.That(Cut.AppliesTo(TypeOf<      IAny[]>()), Is.True );
            Assert.That(Cut.AppliesTo(TypeOf<IList<IAny> >()), Is.True );
            Assert.That(Cut.AppliesTo(TypeOf<      IAny  >()), Is.False);
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
            Cut.Matching(t => t.UnderlyingType == typeof(TestType));

            Assert.That(Cut.AppliesTo(TypeOf<TestType>()), Is.True );
            Assert.That(Cut.AppliesTo(TypeOf<IAny    >()), Is.False);
        }

        [Test]
        public void Matching_Predicate_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Cut.Matching(null as Func<ProjectionType, bool>)
            )
            .ForParameter("predicate");
        }

        [Test]
        public void Matching_Restriction()
        {
            Cut.Matching(new IsTestTypeRestriction());

            Assert.That(Cut.AppliesTo(TypeOf<TestType>()), Is.True );
            Assert.That(Cut.AppliesTo(TypeOf<IAny    >()), Is.False);
        }

        [Test]
        public void Matching_Restriction_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Cut.Matching(null as ITypeRestriction)
            )
            .ForParameter("restriction");
        }

        [Test]
        public void WithMultipleRestrictions()
        {
            Cut
                .OfKind(TypeKind.Structure)
                .Matching(t => t.Name.Contains("Test"));

            Assert.That(Cut.AppliesTo(TypeOf<ITestType>()), Is.True );
            Assert.That(Cut.AppliesTo(TypeOf<TestType >()), Is.False);
            Assert.That(Cut.AppliesTo(TypeOf<IAny     >()), Is.False);
        }

        private interface ITestType { }
        private class      TestType { }

        private class IsTestTypeRestriction : ITypeRestriction
        {
            public bool AppliesTo(ProjectionType type)
            {
                return type.UnderlyingType == typeof(TestType);
            }
        }
    }
}
