namespace Projector
{
    // TODO: Prefer "Fake" over "Mock"
    using System;
    using Projector.ObjectModel;

    internal interface IMockTrait : ITraitOptions
    {
        string Tag { get; set; }
    }

    internal abstract class MockTrait : IMockTrait
    {
        public AttributeTargets ValidOn       { get; set; }
        public bool             AllowMultiple { get; set; }
        public bool             Inherited     { get; set; }
        public string           Tag           { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal abstract class MockTraitAttribute : Attribute, IMockTrait
    {
        // NOTE: ITraitOptions overrides AttributeUsageAttribute

        public AttributeTargets ValidOn       { get; set; }
        public bool             AllowMultiple { get; set; }
        public bool             Inherited     { get; set; }
        public string           Tag           { get; set; }
    }

    internal interface IMockAnnotation : IMockTrait { }

    internal class MockAnnotation : MockTrait, IMockAnnotation
    {
        public override string ToString()
        {
            return MockTraitHelpers.ToString(this);
        }
    }

    internal class MockAnnotationAttribute : MockTraitAttribute, IMockAnnotation
    {
        public override string ToString()
        {
            return MockTraitHelpers.ToString(this);
        }
    }

    internal interface IMockBehavior : IMockTrait, IProjectionBehavior { }

    internal class MockBehavior : MockTrait, IMockBehavior
    {
        public int Priority { get; set; }

        public override string ToString()
        {
            return MockTraitHelpers.ToString(this);
        }
    }

    internal class MockBehaviorAttribute : MockTraitAttribute, IMockBehavior
    {
        public int Priority { get; set; }

        public override string ToString()
        {
            return MockTraitHelpers.ToString(this);
        }
    }

    internal static class MockTraitHelpers
    {
        public static string ToString(IMockAnnotation annotation)
        {
            return string.Format
            (
                "{0}(An, {1}, {2})",
                annotation.GetName(),
                annotation.DescribeAllowMultiple(),
                annotation.DescribeInherited()
            );
        }
        
        public static string ToString(IMockBehavior behavior)
        {
            return string.Format
            (
                "{0}(#{1}, {2}, {3})",
                behavior.GetName(),
                behavior.Priority,
                behavior.DescribeAllowMultiple(),
                behavior.DescribeInherited()
            );
        }

        private static string GetName(this IMockTrait trait)
        {
            return trait.Tag ?? trait.GetType().Name;
        }

        private static string DescribeAllowMultiple(this ITraitOptions options)
        {
            return options.AllowMultiple ? "Mul" : "Sin";
        }

        private static string DescribeInherited(this ITraitOptions options)
        {
            return options.Inherited ? "Inh" : "Non";
        }
    }

    internal class AnnotationA : MockAnnotation { }
    internal class AnnotationB : MockAnnotation { }
    internal class AnnotationC : MockAnnotation { }

    internal class AnnotationAAttribute : MockAnnotationAttribute { }
    internal class AnnotationBAttribute : MockAnnotationAttribute { }
    internal class AnnotationCAttribute : MockAnnotationAttribute { }

    internal class BehaviorA : MockBehavior { }
    internal class BehaviorB : MockBehavior { }
    internal class BehaviorC : MockBehavior { }

    internal class BehaviorAAttribute : MockBehaviorAttribute { }
    internal class BehaviorBAttribute : MockBehaviorAttribute { }
    internal class BehaviorCAttribute : MockBehaviorAttribute { }
}
