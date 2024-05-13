using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.NewArrayExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.NewArrayExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.NewArrayExpression"/></returns>
        private NewArrayExpression NewArrayExpression(
                ExpressionType nodeType, 
                JObject jObject
            )
        {
            Type elementType = ReadKeyValuePair(
                                        jObject, 
                                        "elementType", 
                                        GetTokenType
                                    );
            IEnumerable<Expression> expressions = ReadKeyValuePair(
                                                            jObject, 
                                                            "expressions", 
                                                            GetEnumerableBuilder(DeserializeAndBuildExpression)
                                                        );

            switch (nodeType) {
                case ExpressionType.NewArrayInit:
                    return Expression.NewArrayInit(elementType, expressions);
                case ExpressionType.NewArrayBounds:
                    return Expression.NewArrayBounds(elementType, expressions);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
