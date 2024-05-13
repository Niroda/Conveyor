using System;
using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized NewArrayExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is NewArrayExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a NewArrayExpression, False otherwise.</returns>
        private bool NewArrayExpression(Expression expression)
        {
            // cast the expression in to a NewArrayExpression
            NewArrayExpression newArrayExpression = expression as NewArrayExpression;
            // if the variable newArrayExpression is null, then given expression isn't a NewArrayExpression
            if (newArrayExpression == null) { return false; }

            WriteKeyValuePair("typeName", "newArray");
            WriteKeyValuePair("elementType", InvokeTypeWriter(newArrayExpression.Type.GetElementType()));
            WriteKeyValuePair("expressions", Apply(newArrayExpression.Expressions, GetExpressionAction));

            return true;
        }
    }
}
