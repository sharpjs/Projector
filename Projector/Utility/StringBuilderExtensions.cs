namespace Projector
{
    using System.Text;

    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendList(this StringBuilder text, params string[] items)
        {
            text.Append('[');

            if (items.Length != 0)
            {
                text.Append(items[0]);

                for (var i = 1; i < items.Length; i++)
                    text.Append(", ").Append(items[i]);
            }

            return text.Append(']');
        }

        public static StringBuilder AppendList<T>(this StringBuilder text, params T[] items)
        {
            text.Append('[');

            if (items.Length != 0)
            {
                text.Append(items[0].ToString());

                for (var i = 1; i < items.Length; i++)
                    text.Append(", ").Append(items[i].ToString());
            }

            return text.Append(']');
        }
    }
}
