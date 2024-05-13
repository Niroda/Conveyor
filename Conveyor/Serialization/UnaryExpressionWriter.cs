using System;
using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized UnaryExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is UnaryExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a UnaryExpression, False otherwise.</returns>
        private bool UnaryExpression(Expression expression)
        {
            // cast the expression in to a UnaryExpression
            UnaryExpression unaryExpression = expression as UnaryExpression;
            // if the variable unaryExpression is null, then given expression isn't a UnaryExpression
            if (unaryExpression == null) { return false; }

            WriteKeyValuePair("typeName", "unary");
            WriteKeyValuePair("operand", GetExpressionAction(unaryExpression.Operand));
            WriteKeyValuePair("method", InvokeMethodWriter(unaryExpression.Method));

            return true;
        }
    }
}
