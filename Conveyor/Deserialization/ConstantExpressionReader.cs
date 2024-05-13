using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.ConstantExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.ConstantExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="type">The result type of the block</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.ConstantExpression"/></returns>
        private ConstantExpression ConstantExpression(
                ExpressionType nodeType,
                Type type,
                JObject jObject
            )
        {
            object value;

            JToken valueTok = ReadKeyValuePair(jObject, "value");

            if (valueTok == null || valueTok.Type == JTokenType.Null)
            {
                value = null;
            }
            else
            {
                JObject valueObj = (JObject)valueTok;
                Type valueType = ReadKeyValuePair(valueObj, "type", GetTokenType);
                value = ToObject(ReadKeyValuePair(valueObj, "value"), valueType);
            }

            switch (nodeType)
            {
                case ExpressionType.Constant:
                    return Expression.Constant(value, type);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
