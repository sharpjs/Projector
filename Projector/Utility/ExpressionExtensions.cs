namespace Projector
{
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class ExpressionExtensions
    {
        internal static PropertyInfo ToProperty(this LambdaExpression expression)
        {
            if (expression == null)
                throw Error.ArgumentNull("expression");

            var access = expression.Body as MemberExpression;
            if (access == null)
                throw Error.NotPropertyExpression("expression");

            var property = access.Member as PropertyInfo;
            if (property == null)
                throw Error.NotPropertyExpression("expression");

            return property;
        }
    }
}
