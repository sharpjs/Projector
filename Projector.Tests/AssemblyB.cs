namespace Projector
{
    using System.Reflection;

    public sealed partial  class AssemblyB
    {
        // Used like a static class, but usable as a type argument too
        private AssemblyB() { }

        public static Assembly Assembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        private const string
            TraitPath = "B";

        public static partial class Fakes
        {
            private const string
                TraitPath = AssemblyB.TraitPath + ".Fakes";
        }
    }
}
