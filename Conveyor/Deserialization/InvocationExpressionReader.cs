using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.InvocationExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.InvocationExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.InvocationExpression"/></returns>
        private InvocationExpression InvocationExpression(
                ExpressionType nodeType, 
                JObject jObject
            )
        {
            Expression expression = ReadKeyValuePair(
                                            jObject, 
                                            "expression", 
                                            DeserializeAndBuildExpression
                                        );
            IEnumerable<Expression> arguments = ReadKeyValuePair(
                                                        jObject, 
                                                        "arguments", 
                                                        GetEnumerableBuilder(DeserializeAndBuildExpression)
                                                    );
            
            switch (nodeType) {
                case ExpressionType.Invoke:
                    if (arguments == null)
                        return Expression.Invoke(expression);

                    return Expression.Invoke(expression, arguments);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
