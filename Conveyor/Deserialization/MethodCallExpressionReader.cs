using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.MethodCallExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.MethodCallExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.MethodCallExpression"/></returns>
        private MethodCallExpression MethodCallExpression(
                ExpressionType nodeType, 
                JObject jObject
            )
        {
            Expression instance = ReadKeyValuePair(
                                            jObject, 
                                            "object", 
                                            DeserializeAndBuildExpression
                                        );
            MethodInfo methodInfo = ReadKeyValuePair(
                                                jObject, 
                                                "method", 
                                                GetMethodInfo
                                            );
            IEnumerable<Expression> arguments = ReadKeyValuePair(
                                                        jObject, 
                                                        "arguments", 
                                                        GetEnumerableBuilder(DeserializeAndBuildExpression)
                                                    );

            switch (nodeType) {
                case ExpressionType.ArrayIndex:
                    return Expression.ArrayIndex(instance, arguments);
                case ExpressionType.Call:
                    return Expression.Call(instance, methodInfo, arguments);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
