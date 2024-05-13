using Conveyor.Utility;
using System.Linq;
using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized BlockExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is MethodCallExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a MethodCallExpression, False otherwise.</returns>
        private bool MethodCallExpression(Expression expression)
        {
            // cast the expression in to a BlockExpression
            MethodCallExpression methodCallExpression = expression as MethodCallExpression;
            // if the variable methodCallExpression is null, then given expression isn't a MethodCallExpression
            if (methodCallExpression == null)
                return false;

            // check if any parameter of the called method is a member.
            bool memberAccess = methodCallExpression
                                        .Arguments
                                        .Any(
                                           x =>
                                           x.NodeType == ExpressionType.MemberAccess
                                           &&
                                           !_parameters.Any(a => a.Type.Equals((x as MemberExpression)?.Expression?.Type))
                                        ) 
                                        ||
                                        (
                                        methodCallExpression.Object != null
                                        &&
                                        !_parameters.Any(a => a.Type.Equals((methodCallExpression.Object as MemberExpression)?.Expression?.Type))
                                        );

            Expression[] arguments = memberAccess ? new Expression[methodCallExpression.Arguments.Count] : null;

            if (memberAccess)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (
                   (methodCallExpression.Arguments[i].NodeType == ExpressionType.Constant
                   ||
                   methodCallExpression.Arguments[i].NodeType == ExpressionType.MemberAccess)
                   &&
                   !_parameters.Any(a => a.Type.Equals((methodCallExpression.Arguments[i] as MemberExpression)?.Expression?.Type))
                   )
                        arguments[i] = Expression.Constant(MemberAccessResolver.GetConstantValue(methodCallExpression.Arguments[i]));
                    else
                        arguments[i] = methodCallExpression.Arguments[i];
                }

                if (methodCallExpression.Object != null)
                {
                    object memberAccessValue = MemberAccessResolver.GetConstantValue(methodCallExpression.Object);
                    Expression instance = null;
                    if (memberAccessValue != null)
                        instance = Expression.Constant(memberAccessValue);
                    else
                        instance = methodCallExpression.Object;

                    methodCallExpression = Expression.Call(
                                                      instance,
                                                      methodCallExpression.Method,
                                                      arguments
                                                 );
                } else
                {
                    methodCallExpression = Expression.Call(
                                                          methodCallExpression.Method,
                                                          arguments
                                                     );
                }

            }

            WriteKeyValuePair("typeName", "methodCall");
            WriteKeyValuePair("object", GetExpressionAction(methodCallExpression.Object));
            WriteKeyValuePair("method", InvokeMethodWriter(methodCallExpression.Method));
            WriteKeyValuePair("arguments", Apply(methodCallExpression.Arguments, GetExpressionAction));

            return true;
        }
    }
}
