namespace Projector.ObjectModel.PropertyAccessors
{
    using System;

    internal class Int32ToInt64Converter : IConverter<long, int>
    {
        public long ToOuter(int inner)
        {
            return inner;
        }

        public int ToInner(long outer)
        {
            try
            {
                checked { return (int) outer; }
            }
            catch (OverflowException e)
            {
                throw Error.ConversionFailed<long, int>(e);
            }
        }
    }
}
