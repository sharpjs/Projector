namespace Projector
{
    using System;
    using Projector.ObjectModel;

    [Serializable]
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class SuppressAttribute : Attribute
    {
        private readonly Type type;

        public SuppressAttribute(Type type)
        {
            if (type == null)
                throw Error.ArgumentNull("type");

            this.type = type;
        }

        public Type Type
        {
            get { return type; }
        }
    }
}
