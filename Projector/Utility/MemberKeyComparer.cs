namespace Projector.ObjectModel
{
    using System.Collections.Generic;

    internal sealed class MemberKeyComparer : IEqualityComparer<MemberKey>
    {
        public static MemberKeyComparer Instance = new MemberKeyComparer();

        private MemberKeyComparer() { }

        public bool Equals(MemberKey keyA, MemberKey keyB)
        {
            return keyA.DeclaringType == keyB.DeclaringType
                && keyA.MemberName    == keyB.MemberName;
        }

        public int GetHashCode(MemberKey key)
        {
            unchecked
            {
                return key.DeclaringType.GetHashCode() * 17
                    +  key.MemberName   .GetHashCode();
            }
        }
    }
}
