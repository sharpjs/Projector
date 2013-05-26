namespace Projector.Specs
{
    public static partial class PropertyCutExtensions
    {
        // Methods are colocated with restriction types

        private static IPropertyCut Required(IPropertyCut cut)
        {
            if (cut == null)
                throw Error.ArgumentNull("cut");

            return cut;
        }
    }
}
