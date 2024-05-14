using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized NewExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is NewExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a NewExpression, False otherwise.</returns>
        private bool NewExpression(Expression expression)
        {
            // cast the expression in to a NewExpression
            NewExpression newExpression = expression as NewExpression;
            // if the variable newExpression is null, then given expression isn't a NewExpression
            if (newExpression == null)
                return false;

            WriteKeyValuePair("typeName", "new");
            WriteKeyValuePair("constructor", InvokeConstructorWriter(newExpression.Constructor));
            WriteKeyValuePair("arguments", Apply(newExpression.Arguments, GetExpressionAction));
            WriteKeyValuePair("members", Apply(newExpression.Members, InvokeMemberWriter));

            return true;
        }
    }
}
