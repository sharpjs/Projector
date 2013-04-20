namespace Projector
{
    using System;
    using System.Text;

    internal static class TypePrettyName
    {
        public static string GetPrettyName(this Type type, bool qualified)
        {
            if (type == null)
                throw Error.ArgumentNull("type");

            if (!type.IsGenericType && !type.IsArray && !type.IsNested && !type.IsByRef)
                return qualified ? type.FullName : type.Name;

            if (type.IsGenericParameter)
                return type.Name;

            return new StringBuilder(64)
                .AppendPrettyNameCore(type, new PrettyNameContext(type, qualified))
                .ToString();
        }

        private static StringBuilder AppendPrettyName
            (this StringBuilder name, Type type, PrettyNameContext context)
        {
            return name.AppendPrettyNameCore(type, new PrettyNameContext(type, context.IsQualified));
        }

        private static StringBuilder AppendPrettyNameCore
            (this StringBuilder name, Type type, PrettyNameContext context)
        {
            // Suffixes (array, ref, pointer)
            if (type.IsArray)
                return name
                    .AppendPrettyName(type.GetElementType(), context)
                    .Append('[')
                    .Append(',', type.GetArrayRank() - 1)
                    .Append(']');
            else if (type.IsByRef)
                return name
                    .AppendPrettyName(type.GetElementType(), context)
                    .Append('&');
            else if (type.IsPointer)
                return name
                    .AppendPrettyName(type.GetElementType(), context)
                    .Append('*');

            // Prefixes (nesting, namespace)
            if (type.IsNested)
                name.AppendPrettyNameCore(type.DeclaringType, context)
                    .Append('.');
            else if (context.IsQualified)
                name.Append(type.Namespace)
                    .Append('.');

            // Name and arguments
            if (type.IsGenericType)
                return name
                    .Append(type.Name.WithoutGenericSuffix())
                    .AppendPrettyArguments(type, context);
            else
                return name
                    .Append(type.Name);
        }

        private static StringBuilder AppendPrettyArguments
            (this StringBuilder name, Type type, PrettyNameContext context)
        {
            // No args remain (ex: non-generic nested in generic)
            var count = context.GetGenericArgumentCount(type);
            if (count == 0)
                return name;

            // Completely open type
            if (context.IsGenericTypeDefinition)
                return name
                    .Append('<')
                    .Append(',', count - 1)
                    .Append('>');

            // Partially or fully closed types
            name.Append('<');

            for (var i = 0; i < count; i++)
            {
                var arg = context.GetNextGenericArgument();
                if (arg.IsGenericParameter)
                {
                    if (i != 0) name.Append(',');
                }
                else
                {
                    if (i != 0) name.Append(',').Append(' ');
                    name.AppendPrettyName(arg, context);
                }
            }

            return name.Append('>');
        }

        private static string WithoutGenericSuffix(this string name)
        {
            var index = name.IndexOf('`');

            return index == -1
                ? name
                : name.Substring(0, index);
        }

        private sealed class PrettyNameContext
        {
            private readonly Type subjectType;
            private readonly bool isQualified;

            private bool   isDefinition;
            private Type[] arguments;
            private int    argumentIndex;

            public PrettyNameContext(Type type, bool qualified)
            {
                this.subjectType = type;
                this.isQualified = qualified;
            }

            public bool IsQualified
            {
                get { return isQualified; }
            }

            public bool IsGenericTypeDefinition
            {
                get { return isDefinition; }
            }

            public int GetGenericArgumentCount(Type type)
            {
                if (arguments == null)
                {
                    isDefinition = subjectType.IsGenericTypeDefinition;
                    arguments    = subjectType.GetGenericArguments();
                }

                return type == subjectType
                    ? arguments.Length - argumentIndex
                    : type.GetGenericArguments().Length;
            }

            public Type GetNextGenericArgument()
            {
                return arguments[argumentIndex++];
            }
        }
    }
}
