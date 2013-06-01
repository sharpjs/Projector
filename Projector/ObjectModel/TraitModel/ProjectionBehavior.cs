namespace Projector.ObjectModel
{
    using System;

    [Serializable]
    public abstract class ProjectionBehavior : IProjectionBehavior
    {
        private int priority;

        protected ProjectionBehavior() { }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public static class Priorities
        {
            public const int
                Minimum = int.MinValue,
                Default = default(int),
                Maximum = int.MaxValue;
        }
    }
}
