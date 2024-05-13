using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.DefaultExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.DefaultExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="type">The result type of the block</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.DefaultExpression"/></returns>
        private DefaultExpression DefaultExpression(
                ExpressionType nodeType, 
                Type type, 
                JObject jObject
            )
        {
            switch (nodeType) {
                case ExpressionType.Default:
                    return Expression.Default(type);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
