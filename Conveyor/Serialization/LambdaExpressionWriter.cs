using System;
using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized LambdaExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is LambdaExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a LambdaExpression, False otherwise.</returns>
        private bool LambdaExpression(Expression expression)
        {
            // cast the expression in to a LambdaExpression
            LambdaExpression lambdaExpression = expression as LambdaExpression;
            // if the variable lambdaExpression is null, then given expression isn't a LambdaExpression
            if (lambdaExpression == null)
                return false;

            WriteKeyValuePair("typeName", "lambda");
            WriteKeyValuePair("name", lambdaExpression.Name);
            WriteKeyValuePair("parameters", Apply(lambdaExpression.Parameters, GetExpressionAction));
            WriteKeyValuePair("body", GetExpressionAction(lambdaExpression.Body));
            WriteKeyValuePair("tailCall", lambdaExpression.TailCall);

            return true;
        }
    }
}
