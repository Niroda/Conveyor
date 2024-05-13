using System.Linq.Expressions;

namespace Conveyor.Serialization
{
    /// <summary>
    /// Used to create a serialized TypeBinaryExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is TypeBinaryExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a TypeBinaryExpression, False otherwise.</returns>
        private bool TypeBinaryExpression(Expression expression)
        {
            // cast the expression in to a type TypeBinaryExpression
            TypeBinaryExpression typeBinaryExpression = expression as TypeBinaryExpression;
            // if the variable typeBinaryExpression is null, then given expression isn't a TypeBinaryExpression
            if (typeBinaryExpression == null)
                return false;

            #region Write TypeBinaryExpression body
            WriteKeyValuePair("typeName", "typeBinary");
            WriteKeyValuePair("expression", GetExpressionAction(typeBinaryExpression.Expression));
            WriteKeyValuePair("typeOperand", InvokeTypeWriter(typeBinaryExpression.TypeOperand));
            #endregion

            return true;
        }
    }
}
