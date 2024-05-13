using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.ConditionalExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.ConditionalExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="type">The result type of the block</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.ConditionalExpression"/></returns>
        private ConditionalExpression ConditionalExpression(
                ExpressionType nodeType, 
                Type type, 
                JObject jObject
            )
        {
            Expression test = ReadKeyValuePair(jObject, "test", DeserializeAndBuildExpression);
            Expression ifTrue = ReadKeyValuePair(jObject, "ifTrue", DeserializeAndBuildExpression);
            Expression ifFalse = ReadKeyValuePair(jObject, "ifFalse", DeserializeAndBuildExpression);

            switch (nodeType) {
                case ExpressionType.Conditional:
                    return Expression.Condition(test, ifTrue, ifFalse, type);
                default:
                    throw new NotSupportedException($"Specified type {nodeType} is not supported.");
            }
        }
    }
}
