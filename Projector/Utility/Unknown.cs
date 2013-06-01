namespace Projector
{
    using System.Diagnostics;

    [DebuggerDisplay("Unknown.Value")]
    public sealed class Unknown
    {
        private static readonly Unknown
            value = new Unknown();

        private Unknown() { }

        public static Unknown Value
        {
            get { return value; }
        }

        public override string ToString()
        {
            return "(unknown)";
        }
    }
}
