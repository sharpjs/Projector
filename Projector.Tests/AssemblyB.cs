namespace Projector
{
    using System.Reflection;

    public sealed class AssemblyB
    {
        // Used like a static class, but usable as a type argument too
        private AssemblyB() { }

        public static Assembly Assembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }
    }
}
