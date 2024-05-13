using System;
using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized InvocationExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is InvocationExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a InvocationExpression, False otherwise.</returns>
        private bool InvocationExpression(Expression expression)
        {
            // cast the expression in to a InvocationExpression
            InvocationExpression invocationExpression = expression as InvocationExpression;
            // if the variable invocationExpression is null, then given expression isn't a InvocationExpression
            if (invocationExpression == null)
                return false;

            WriteKeyValuePair("typeName", "invocation");
            WriteKeyValuePair("expression", GetExpressionAction(invocationExpression.Expression));
            WriteKeyValuePair("arguments", Apply(invocationExpression.Arguments, GetExpressionAction));

            return true;
        }
    }
}
