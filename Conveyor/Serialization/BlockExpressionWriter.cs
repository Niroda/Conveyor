using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized BlockExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is BlockExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a BlockExpression, False otherwise.</returns>
        private bool BlockExpression(Expression expression)
        {
            // cast the expression in to a BlockExpression
            BlockExpression blockExpression = expression as BlockExpression;
            // if the variable blockExpression is null, then given expression isn't a BlockExpression
            if (blockExpression == null)
                return false;

            #region Write BlockExpression body
            WriteKeyValuePair("typeName", "block");
            WriteKeyValuePair("expressions", Apply(blockExpression.Expressions, GetExpressionAction));
            WriteKeyValuePair("variables", Apply(blockExpression.Variables, GetExpressionAction));
            #endregion

            return true;
        }
    }
}
