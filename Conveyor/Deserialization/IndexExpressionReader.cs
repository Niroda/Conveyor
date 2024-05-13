using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.IndexExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.IndexExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.IndexExpression"/></returns>
        private IndexExpression IndexExpression(
                ExpressionType nodeType, 
                JObject jObject
            )
        {
            Expression expression = ReadKeyValuePair(
                                                jObject, 
                                                "object", 
                                                DeserializeAndBuildExpression
                                            );
            PropertyInfo indexer = ReadKeyValuePair(
                                                jObject, 
                                                "indexer", 
                                                GetPropertyInfo
                                            );
            IEnumerable<Expression> arguments = ReadKeyValuePair(
                                                        jObject, "arguments", 
                                                        GetEnumerableBuilder(DeserializeAndBuildExpression)
                                                    );

            switch (nodeType) {
                case ExpressionType.Index:
                    return Expression.MakeIndex(expression, indexer, arguments);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
