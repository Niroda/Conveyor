using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized MemberExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is MemberExpression and writes it, if it is.
        /// </summary>
        /// <param name="expr">Given expression to be serialized</param>
        /// <returns>True if given expression is a MemberExpression, False otherwise.</returns>
        private bool MemberExpression(Expression expr)
        {
            // cast the expression in to a MemberExpression
            MemberExpression memberExpression = expr as MemberExpression;
            // if the variable memberExpression is null, then given expression isn't a MemberExpression
            if (memberExpression == null) { return false; }

            WriteKeyValuePair("typeName", "member");
            WriteKeyValuePair("expression", GetExpressionAction(memberExpression.Expression));
            WriteKeyValuePair("member", InvokeMemberWriter(memberExpression.Member));

            return true;
        }
    }
}
