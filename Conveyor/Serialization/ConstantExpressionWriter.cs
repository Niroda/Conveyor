using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized ConstantExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is ConstantExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a ConstantExpression, False otherwise.</returns>
        private bool ConstantExpression(Expression expression)
        {
            // cast the expression in to a ConstantExpression
            ConstantExpression constantExpression = expression as ConstantExpression;
            // if the variable constantExpression is null, then given expression isn't a ConstantExpression
            if (constantExpression == null)
                return false;

            #region Write ConstantExpression body
            WriteKeyValuePair("typeName", "constant");
            if (constantExpression.Value == null)
            {
                WriteKeyValuePair("value", () => _jsonWriter.WriteNull());
            }
            else
            {
                var value = constantExpression.Value;
                var type = value.GetType();
                WriteKeyValuePair("value", () =>
                {
                    _jsonWriter.WriteStartObject();
                    WriteKeyValuePair("type", InvokeTypeWriter(type));
                    WriteKeyValuePair("value", Serialize(value, type));
                    _jsonWriter.WriteEndObject();
                });
            }
            #endregion

            return true;
        }


        /// <summary>
        /// Checks whether an <see cref="System.Linq.Expressions.Expression"/> ends with a constant value regardless the way.
        /// It might be a call to a function that returns a value, variable, property or whatever.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be checked</param>
        /// <returns>True if the give expression results in a constant value, false otherwise.</returns>
        private bool IsConstantExpression(Expression expression)
        {
            while (expression != null && expression.NodeType != ExpressionType.Parameter)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Constant:
                        return true;
                    case ExpressionType.Call:
                        MethodCallExpression methodCallExpression = expression as MethodCallExpression;
                        // check whether there is any parameter passed to this function and lambda
                        if(methodCallExpression.Arguments.Count > 0 && _parameters != null)
                        {
                            // check if any of the called function parameter is the same as the lambda's
                            bool checkParam = methodCallExpression.Arguments
                                                .Any(
                                                    x =>
                                                        x is ParameterExpression parameter
                                                        &&
                                                        _parameters.Any(
                                                            a =>
                                                                a.Name.Equals(parameter.Name)
                                                                &&
                                                                a.Type.Equals(parameter.Type)
                                                        )
                                                );
                            // if so, throw an exception because the call to this method won't be evaluated.
                            // additionally, this method might not be available during deserialization process.
                            if (checkParam)
                                throw new MethodAccessException(
                                        $"{methodCallExpression.Method.Name} is not allowed to take any of lambda's argument as a parameter!"
                                    );
                        }

                        if (methodCallExpression.Object != null)
                            expression = methodCallExpression.Object;
                        else
                        {
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

                            if (memberAccess)
                                return false;

                            for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
                                if (IsConstantExpression(methodCallExpression.Arguments[i]))
                                    return true;

                            return false;
                        }
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpression = expression as MemberExpression;
                        if (memberExpression != null)
                        {
                            if (typeof(ICollection).IsAssignableFrom(memberExpression.Type))
                                return false;
                            
                            expression = memberExpression.Expression;
                            if (expression == null)
                                return false;
                        }
                        break;
                    case ExpressionType.Block:
                        expression = (expression as BlockExpression).Result;
                        break;
                    case ExpressionType.Lambda:
                        expression = (expression as LambdaExpression).Body;
                        break;
                    case ExpressionType.Invoke:
                        expression = (expression as InvocationExpression).Expression;
                        break;
                    case ExpressionType.Index:
                        expression = (expression as IndexExpression).Object;
                        break;
                    case ExpressionType.Conditional:
                        expression = (expression as ConditionalExpression).Test;
                        break;
                    case ExpressionType.RuntimeVariables:
                    case ExpressionType.NewArrayInit:
                    case ExpressionType.Default:
                    case ExpressionType.Parameter:
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}
