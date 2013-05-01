namespace Projector
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.Constraints;
    using Projector.ObjectModel;

	internal static class NUnitExtensions
	{
		public static EqualConstraint Tag(this ConstraintExpression e, string tag)
		{
			return e.Property("Tag").EqualTo(tag);
		}

		public static EqualConstraint Name(this ConstraintExpression e, string name)
		{
			return e.Property("Name").EqualTo(name);
		}

		public static EqualConstraint DeclaringType(this ConstraintExpression e, ProjectionType type)
		{
			return e.Property("DeclaringType").EqualTo(type);
		}

		public static TException WithMessageMatching<TException>(this TException e, string pattern)
			where TException : Exception
		{
			Assert.That(e.Message, Is.StringMatching(pattern));
			return e;
		}

		public static ArgumentNullException ForParameter(this ArgumentNullException e, string pattern)
		{
			return e.WithMessageMatching(@"Value cannot be null\.(.|\n)*Parameter name: " + pattern);
		}

		public static ArgumentOutOfRangeException ForParameter(this ArgumentOutOfRangeException e, string pattern)
		{
			return e.WithMessageMatching(@"out of the range of valid values\.(.|\n)*Parameter name: " + pattern);
		}
	}
}
