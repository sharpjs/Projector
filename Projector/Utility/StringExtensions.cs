namespace Projector
{
    using System;

    internal static class StringExtensions
    {
        public static string RemoveSuffix(this string text, string suffix)
        {
            return text.EndsWith(suffix, StringComparison.Ordinal)
                ? text.Substring(0, text.Length - suffix.Length)
                : text;
        }
    }
}
