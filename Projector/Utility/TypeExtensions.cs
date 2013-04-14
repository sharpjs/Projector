namespace Projector
{
	using System;
	using System.Text;

	internal static class TypeExtensions
	{
        public static string RemoveInterfacePrefix(this string name)
        {
			if (name == null)
				throw Error.ArgumentNull("name");

            return (name.Length > 1 && name[0] == 'I' && char.IsUpper(name, 1))
                ? name.Substring(1)
                : name;
        }
	}
}
