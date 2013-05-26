namespace Projector.Specs
{
    public static partial class TypeCutExtensions
    {
        // Methods are colocated with restriction types

        private static ITypeCut Required(ITypeCut cut)
        {
            if (cut == null)
                throw Error.ArgumentNull("cut");

            return cut;
        }
    }
}
