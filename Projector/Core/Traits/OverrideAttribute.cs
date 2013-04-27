namespace Projector
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class OverrideAttribute : Attribute
    {
        private readonly Type sourceType;
        private string        memberName;

        public OverrideAttribute(Type sourceType)
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
    }
}
