namespace Projector.Fakes.WithManyTraitSpecs
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property,
        AllowMultiple = false, Inherited = true)]
    public class FakeAttribute : Attribute
    {
        public string Tag { get; set; }

        public override bool Equals(object obj)
        {
            var that = obj as FakeAttribute;
            return that != null && that.Tag == this.Tag;
        }

        public override int GetHashCode()
        {
            return Tag == null ? 0 : Tag.GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(GetType().Name, ":", Tag);
        }
    }
}
