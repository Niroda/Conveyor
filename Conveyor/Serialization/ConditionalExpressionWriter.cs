using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized ConditionalExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is ConditionalExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a ConditionalExpression, False otherwise.</returns>
        private bool ConditionalExpression(Expression expression)
        {
            // cast the expression in to a ConditionalExpression
            ConditionalExpression conditionalExpression = expression as ConditionalExpression;
            // if the variable conditionalExpression is null, then given expression isn't a ConditionalExpression
            if (conditionalExpression == null)
                return false;

            #region Write ConditionalExpression body
            WriteKeyValuePair("typeName", "conditional");
            WriteKeyValuePair("test", GetExpressionAction(conditionalExpression.Test));
            WriteKeyValuePair("ifTrue", GetExpressionAction(conditionalExpression.IfTrue));
            WriteKeyValuePair("ifFalse", GetExpressionAction(conditionalExpression.IfFalse));
            #endregion

            return true;
        }
    }
}
