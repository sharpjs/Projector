namespace Projector
{
    using System.Reflection;

    public sealed class AssemblyA
    {
        // Used like a static class, but usable as a type argument too
        private AssemblyA() { }

        public static Assembly Assembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }
    }
}
