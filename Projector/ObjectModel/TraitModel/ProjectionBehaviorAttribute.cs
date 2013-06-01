namespace Projector.ObjectModel
{
    using System;

    [Serializable]
    public abstract class ProjectionBehaviorAttribute : Attribute, IProjectionBehavior
    {
        private int priority;

        protected ProjectionBehaviorAttribute() { }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}
