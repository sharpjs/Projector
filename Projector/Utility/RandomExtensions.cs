namespace Projector
{
    using System;

    internal static class RandomExtensions
    {
        internal static char NextAlphanumericChar(this Random random)
        {
            const int
                LowerOffset = 'a',
                UpperOffset = 'A' - 26,
                DigitOffset = '0' - 52;

            var value = random.Next(62);
            value +=
                value < 26 ? LowerOffset : // generates [a-z]
                value < 52 ? UpperOffset : // generates [A-Z]
                /*else*/     DigitOffset ; // generates [0-9]
            return (char) value;
        }
    }
}
