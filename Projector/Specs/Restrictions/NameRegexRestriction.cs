﻿namespace Projector.Specs
{
    using System.Text.RegularExpressions;
    using Projector.ObjectModel;

    internal sealed class NameRegexRestriction : ITypeRestriction, IPropertyRestriction
    {
        private readonly Regex regex;

        public NameRegexRestriction(string pattern, RegexOptions options)
        {
            this.regex = new Regex(pattern, options);
        }

        public bool AppliesTo(ProjectionType type)
        {
            return regex.IsMatch(type.Name);
        }

        public bool AppliesTo(ProjectionProperty property)
        {
            return regex.IsMatch(property.Name);
        }

        public override string ToString()
        {
            return string.Format
            (
                "name matches regex '{0}' with options {{{1}}}",
                regex.ToString(),
                regex.Options
            );
        }
    }

    partial class TypeCutExtensions
    {
        public static ITypeCut NamedLike(this ITypeCut cut, string pattern)
        {
            return Required(cut).Matching(new NameRegexRestriction(pattern, RegexOptions.CultureInvariant));
        }
    }

    partial class PropertyCutExtensions
    {
        public static IPropertyCut NamedLike(this IPropertyCut cut, string pattern)
        {
            return Required(cut).Matching(new NameRegexRestriction(pattern, RegexOptions.CultureInvariant));
        }
    }
}
