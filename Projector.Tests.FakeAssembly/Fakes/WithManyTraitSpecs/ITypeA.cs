namespace Projector.Fakes.WithManyTraitSpecs
{
    [FakeAttribute(Tag = "TypeA")]
    public interface ITypeA
    {
        [FakeAttribute(Tag = "PropertyA")]
        string PropertyA { get; set; }

        [FakeAttribute(Tag = "PropertyB")]
        string PropertyB { get; set; }
    }
}
