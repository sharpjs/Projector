namespace Projector.Utility
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TypePrettyNameTests
    {
        [Test]
        public void GetPrettyName_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as Type).GetPrettyName(false)
            );
        }

        [Test]
        public void GetPrettyName_NonGeneric()
        {
            var name = typeof(object).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("Object"));
        }

        [Test]
        public void GetPrettyName_NonGeneric_Qualified()
        {
            var name = typeof(object).GetPrettyName(true);

            Assert.That(name, Is.EqualTo("System.Object"));
        }

        [Test]
        public void GetPrettyName_Array()
        {
            var name = typeof(int[,]).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("Int32[,]"));
        }

        [Test]
        public void GetPrettyName_Array_Qualified()
        {
            var name = typeof(int[,]).GetPrettyName(true);

            Assert.That(name, Is.EqualTo("System.Int32[,]"));
        }

        [Test]
        public void GetPrettyName_Pointer()
        {
            var name = typeof(int*).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("Int32*"));
        }

        [Test]
        public void GetPrettyName_Pointer_Qualified()
        {
            var name = typeof(int*).GetPrettyName(true);

            Assert.That(name, Is.EqualTo("System.Int32*"));
        }

        [Test]
        public void GetPrettyName_ByRef()
        {
            var name = typeof(int).MakeByRefType().GetPrettyName(false);

            Assert.That(name, Is.EqualTo("Int32&"));
        }

        [Test]
        public void GetPrettyName_ByRef_Qualified()
        {
            var name = typeof(int).MakeByRefType().GetPrettyName(true);

            Assert.That(name, Is.EqualTo("System.Int32&"));
        }

        [Test]
        public void GetPrettyName_ClosedGeneric_OfNonGeneric()
        {
            var name = typeof(Func<string, object>).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("Func<String, Object>"));
        }

        [Test]
        public void GetPrettyName_ClosedGeneric_OfNonGeneric_Qualified()
        {
            var name = typeof(Func<string, object>).GetPrettyName(true);

            Assert.That(name, Is.EqualTo("System.Func<System.String, System.Object>"));
        }

        [Test]
        public void GetPrettyName_ClosedGeneric_OfArray()
        {
            var name = typeof(Func<string[,], int[,]>).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("Func<String[,], Int32[,]>"));
        }

        [Test]
        public void GetPrettyName_ClosedGeneric_OfArray_Qualified()
        {
            var name = typeof(Func<string[,], int[,]>).GetPrettyName(true);

            Assert.That(name, Is.EqualTo("System.Func<System.String[,], System.Int32[,]>"));
        }

        // NOT IMPLEMENTED: Pointer types cannot be used as type arguments
        // - public void GetPrettyName_ClosedGeneric_OfPointer()
        // - public void GetPrettyName_ClosedGeneric_OfPointer_Qualified()

        // NOT IMPLEMENTED: ByRef types cannot be used as type arguments
        // - public void GetPrettyName_ClosedGeneric_OfByRef()
        // - public void GetPrettyName_ClosedGeneric_OfByRef_Qualified()

        [Test]
        public void GetPrettyName_OpenGeneric()
        {
            var name = typeof(Func<,>).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("Func<,>"));
        }

        [Test]
        public void GetPrettyName_OpenGeneric_Qualified()
        {
            var name = typeof(Func<,>).GetPrettyName(true);

            Assert.That(name, Is.EqualTo("System.Func<,>"));
        }

        [Test]
        public void GetPrettyName_GenericParameter()
        {
            var name = typeof(MockClass<,>).GetGenericArguments()[0].GetPrettyName(false);

            Assert.That(name, Is.EqualTo("TA"));
        }

        [Test]
        public void GetPrettyName_GenericParameter_Qualified()
        {
            var name = typeof(MockClass<,>).GetGenericArguments()[0].GetPrettyName(true);

            // Type.FullName returns null in this case
            Assert.That(name, Is.EqualTo("TA"));
        }

        [Test]
        public void GetPrettyName_PartialGeneric()
        {
            var name = typeof(MockSubclass<>).BaseType.GetPrettyName(false);

            Assert.That(name, Is.EqualTo("MockClass<String,>"));
        }

        [Test]
        public void GetPrettyName_PartialGeneric_Qualified()
        {
            var name = typeof(MockSubclass<>).BaseType.GetPrettyName(true);

            Assert.That(name, Is.EqualTo("Projector.Utility.MockClass<System.String,>"));
        }

        [Test]
        public void GetPrettyName_Nested_NonGeneric_NonGeneric()
        {
            var name = typeof(MockClass.NestedClass).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("MockClass.NestedClass"));
        }

        [Test]
        public void GetPrettyName_Nested_NonGeneric_NonGeneric_Qualified()
        {
            var name = typeof(MockClass.NestedClass).GetPrettyName(true);

            Assert.That(name, Is.EqualTo("Projector.Utility.MockClass.NestedClass"));
        }

        [Test]
        public void GetPrettyName_Nested_NonGeneric_ClosedGeneric()
        {
            var name = typeof(MockClass.NestedClass<int>).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("MockClass.NestedClass<Int32>"));
        }

        [Test]
        public void GetPrettyName_Nested_NonGeneric_ClosedGeneric_Qualified()
        {
            var name = typeof(MockClass.NestedClass<int>).GetPrettyName(true);

            Assert.That(name, Is.EqualTo("Projector.Utility.MockClass.NestedClass<System.Int32>"));
        }

        [Test]
        public void GetPrettyName_Nested_ClosedGeneric_NonGeneric()
        {
            var name = typeof(MockClass<string, object>.NestedClass).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("MockClass<String, Object>.NestedClass"));
        }

        [Test]
        public void GetPrettyName_Nested_ClosedGeneric_NonGeneric_Qualified()
        {
            var name = typeof(MockClass<string, object>.NestedClass).GetPrettyName(true);

            Assert.That(name, Is.EqualTo(
                "Projector.Utility.MockClass<System.String, System.Object>" +
                ".NestedClass"
            ));
        }

        [Test]
        public void GetPrettyName_Nested_ClosedGeneric_ClosedGeneric()
        {
            var name = typeof(MockClass<string, object>.NestedClass<int>).GetPrettyName(false);

            Assert.That(name, Is.EqualTo("MockClass<String, Object>.NestedClass<Int32>"));
        }

        [Test]
        public void GetPrettyName_Nested_ClosedGeneric_ClosedGeneric_Qualified()
        {
            var name = typeof(MockClass<string, object>.NestedClass<int>).GetPrettyName(true);

            Assert.That(name, Is.EqualTo(
                "Projector.Utility.MockClass<System.String, System.Object>" +
                ".NestedClass<System.Int32>"
            ));
        }

        [Test]
        public void GetPrettyName_ByRefArrayOfNestedPointers()
        {
            var name = typeof(MockClass.NestedStruct*[,]).MakeByRefType().GetPrettyName(false);

            Assert.That(name, Is.EqualTo("MockClass.NestedStruct*[,]&"));
        }

        [Test]
        public void GetPrettyName_ByRefArrayOfNestedPointers_Qualified()
        {
            var name = typeof(MockClass.NestedStruct*[,]).MakeByRefType().GetPrettyName(true);

            Assert.That(name, Is.EqualTo("Projector.Utility.MockClass.NestedStruct*[,]&"));
        }

        [Test]
        public void GetPrettyName_ByRefArrayOfNestedGenerics()
        {
            var name = typeof(MockClass<string, object>.NestedClass<int>[,])
                .MakeByRefType().GetPrettyName(false);

            Assert.That(name, Is.EqualTo("MockClass<String, Object>.NestedClass<Int32>[,]&"));
        }

        [Test]
        public void GetPrettyName_ByRefArrayOfNestedGenerics_Qualified()
        {
            var name = typeof(MockClass<string, object>.NestedClass<int>[,])
                .MakeByRefType().GetPrettyName(true);

            Assert.That(name, Is.EqualTo(
                "Projector.Utility.MockClass<System.String, System.Object>" +
                ".NestedClass<System.Int32>[,]&"
            ));
        }
    }

    internal class MockClass
    {
        internal struct NestedStruct    { }
        internal class  NestedClass     { }
        internal class  NestedClass<TC> { }
    }

    internal class MockClass<TA, TB>
    {
        internal class NestedClass     { }
        internal class NestedClass<TC> { }
    }

    internal class MockSubclass<TB> : MockClass<string, TB> { }
}
