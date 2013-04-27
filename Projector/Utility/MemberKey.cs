namespace Projector.ObjectModel
{
    using System;

    internal struct MemberKey
    {
        public readonly Type   DeclaringType;
        public readonly string MemberName;

        public MemberKey(string name, Type declaringType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");

            MemberName    = name;
            DeclaringType = declaringType;
        }

        public MemberKey(ProjectionProperty property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            MemberName    = property.Name;
            DeclaringType = property.DeclaringType.UnderlyingType;
        }
    }
}
