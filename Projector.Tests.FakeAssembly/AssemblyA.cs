namespace Projector
{
    using System.Reflection;

    public sealed partial class AssemblyA
    {
        // Used like a static class, but usable as a type argument too
        private AssemblyA() { }

        public static Assembly Assembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        private const string
            TraitPath = "A";

        public static partial class Fakes
        {
            private const string
                TraitPath = AssemblyA.TraitPath + ".Fakes";
        }
    }
}
