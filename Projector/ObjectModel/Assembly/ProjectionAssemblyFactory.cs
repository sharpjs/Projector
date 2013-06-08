namespace Projector.ObjectModel
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    // In some versions of .NET, performance of TypeBuilder.CreateType degrades at O(n^2) as more
    // types are added to the dynamic assembly.  Workaround is to create one assembly per N types.
    //
    // See: http://support.microsoft.com/kb/970924

    internal abstract class ProjectionAssemblyFactory
    {
        private const string
            NamePrefix    = "Projector.";
        
        private const char
            NameSeparator = '.';

        private const int
            NameSuffixLength = 8;

        private readonly ProjectionOptions            options;
        private readonly CellList<ProjectionAssembly> unsaved;

        protected ProjectionAssemblyFactory(ProjectionOptions options)
        {
            this.options = options;
            this.unsaved = new CellList<ProjectionAssembly>();
        }

        public ProjectionOptions Options
        {
            get { return options; }
        }

        public static ProjectionAssemblyFactory Single(ProjectionOptions options)
        {
            return new SingleFactory(options);
        }

        public static ProjectionAssemblyFactory PerType(ProjectionOptions options)
        {
            return new PerTypeFactory(options);
        }

        public static ProjectionAssemblyFactory Batched(int limit, ProjectionOptions options)
        {
            return new BatchedFactory(limit, options);
        }

        public abstract ProjectionAssembly GetAssembly(Type type);

        public void SaveAll()
        {
            ProjectionAssembly assembly;
            while (unsaved.TryTake(out assembly))
                assembly.Save();
        }

        protected ProjectionAssembly CreateProjectionAssembly(string name)
        {
            var assembly = new ProjectionAssembly(name, options);

            if (0 != (options & ProjectionOptions.SaveAssemblies))
                unsaved.Enqueue(assembly);

            return assembly;
        }

        protected static string GenerateAssemblyName(string prefix, int suffixLength)
        {
            var index = prefix.Length;
            var name  = new char[index + suffixLength];
            prefix.CopyTo(0, name, 0, index);

            var random = new Random();
            do name[index] = random.NextAlphanumericChar();
            while (++index < name.Length);

            return new string(name);
        }

        protected static string GenerateAssemblyName(string prefix, string infix, int suffixLength)
        {
            var index  = prefix.Length;
            var length = infix .Length;
            var name   = new char[index + length + 1 + suffixLength];
            prefix .CopyTo(0, name, 0,     index );
            infix  .CopyTo(0, name, index, length);

            index += length;
            name[index++] = NameSeparator;

            var random = new Random();
            do name[index] = random.NextAlphanumericChar();
            while (++index < name.Length);

            return new string(name);
        }

        private sealed class SingleFactory : ProjectionAssemblyFactory
        {
            private readonly ProjectionAssembly assembly;

            public SingleFactory(ProjectionOptions options)
                : base(options)
            {
                var name = GenerateAssemblyName(NamePrefix, NameSuffixLength);
                assembly = CreateProjectionAssembly(name);
            }

            public override ProjectionAssembly GetAssembly(Type type)
            {
                return assembly;
            }
        }

        private sealed class PerTypeFactory : ProjectionAssemblyFactory
        {
            public PerTypeFactory(ProjectionOptions options)
                : base(options) { }

            public override ProjectionAssembly GetAssembly(Type type)
            {
                var name = GenerateAssemblyName(NamePrefix, type.Name, NameSuffixLength);
                return CreateProjectionAssembly(name);
            }
        }

        private sealed class BatchedFactory : ProjectionAssemblyFactory
        {
            private readonly int limit;
            private int count;
            private ProjectionAssembly module;

            public BatchedFactory(int limit, ProjectionOptions options)
                : base(options)
            {
                if (limit < 1)
                    throw new ArgumentOutOfRangeException("limit");

                this.limit = limit;
            }

            public override ProjectionAssembly GetAssembly(Type type)
            {
                if (count == 0 || count >= limit)
                {
                    var name = GenerateAssemblyName(NamePrefix, NameSuffixLength);
                    module   = CreateProjectionAssembly(name);
                    count    = 1;
                }
                return module;
            }
        }
    }
}
