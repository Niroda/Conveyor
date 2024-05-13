using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.BlockExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.BlockExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="type">The result type of the block</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.BlockExpression"/></returns>
        private BlockExpression BlockExpression(
                ExpressionType nodeType,
                Type type,
                JObject jObject
            )
        {
            IEnumerable<Expression> expressions = ReadKeyValuePair(
                                                            jObject,
                                                            "expressions",
                                                            GetEnumerableBuilder(DeserializeAndBuildExpression)
                                                        );

            IEnumerable<ParameterExpression> arguments = ReadKeyValuePair(
                                                                jObject,
                                                                "variables",
                                                                GetEnumerableBuilder(ParameterExpression)
                                                            );

            switch (nodeType)
            {
                case ExpressionType.Block:
                    return Expression.Block(type, arguments, expressions);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
