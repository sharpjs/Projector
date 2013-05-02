namespace Projector
{
    using System;

    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class InheritFromAttribute : Attribute
    {
        private readonly Type sourceType;
        private string        memberName;
        private Type          attributeType;

        public InheritFromAttribute(Type sourceType)
        {
            if (sourceType == null)
                throw Error.ArgumentNull("sourceType");

            this.sourceType = sourceType;
        }

        public Type SourceType
        {
            get { return sourceType; }
        }

        public string MemberName
        {
            get { return memberName; }
            set { memberName = value; }
        }

        public Type AttributeType
        {
            get { return attributeType; }
            set { attributeType = value; }
        }
    }
}
