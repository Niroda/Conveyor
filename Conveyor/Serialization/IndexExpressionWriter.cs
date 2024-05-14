using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized IndexExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is IndexExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a IndexExpression, False otherwise.</returns>
        private bool IndexExpression(Expression expression)
        {
            // cast the expression in to a IndexExpression
            IndexExpression indexExpression = expression as IndexExpression;
            // if the variable indexExpression is null, then given expression isn't a IndexExpression
            if (indexExpression == null)
                return false;

            WriteKeyValuePair("typeName", "index");
            WriteKeyValuePair("object", GetExpressionAction(indexExpression.Object));
            WriteKeyValuePair("indexer", InvokePropertyWriter(indexExpression.Indexer));
            WriteKeyValuePair("arguments", Apply(indexExpression.Arguments, GetExpressionAction));

            return true;
        }
    }
}
