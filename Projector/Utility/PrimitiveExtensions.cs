using System;
using System.Globalization;

namespace Projector
{
    internal static class PrimitiveExtensions
    {
        public static string ToStringInvariant(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
