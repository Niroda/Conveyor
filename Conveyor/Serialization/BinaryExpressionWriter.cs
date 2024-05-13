using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized BinaryExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is BinaryExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a BinaryExpression, False otherwise.</returns>
        private bool BinaryExpression(Expression expression)
        {
            // cast the expression in to a BinaryExpression
            BinaryExpression binaryExpression = expression as BinaryExpression;
            // if the variable binaryExpression is null, then given expression isn't a BinaryExpression
            if (binaryExpression == null)
                return false;
            #region Write BinaryExpression body
            WriteKeyValuePair("typeName", "binary");
            WriteKeyValuePair("left", GetExpressionAction(binaryExpression.Left));
            WriteKeyValuePair("right", GetExpressionAction(binaryExpression.Right));
            WriteKeyValuePair("method", InvokeMethodWriter(binaryExpression.Method));
            WriteKeyValuePair("conversion", GetExpressionAction(binaryExpression.Conversion));
            WriteKeyValuePair("liftToNull", binaryExpression.IsLiftedToNull);
            #endregion
            return true;
        }
    }
}
