using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized DefaultExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is DefaultExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a DefaultExpression, False otherwise.</returns>
        private bool DefaultExpression(Expression expression)
        {
            // cast the expression in to a DefaultExpression
            DefaultExpression defaultExpression = expression as DefaultExpression;
            // if the variable defaultExpression is null, then given expression isn't a DefaultExpression
            if (defaultExpression == null)
                return false;

            WriteKeyValuePair("typeName", "default");

            return true;
        }
    }
}
