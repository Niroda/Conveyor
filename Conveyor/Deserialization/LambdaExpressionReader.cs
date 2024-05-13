using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.LambdaExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.LambdaExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.LambdaExpression"/></returns>
        private LambdaExpression LambdaExpression(
                ExpressionType nodeType, 
                JObject jObject
            )
        {
            var body = ReadKeyValuePair(jObject, "body", DeserializeAndBuildExpression);
            var tailCall = ReadKeyValuePair(jObject, "tailCall").Value<bool>();
            var parameters = ReadKeyValuePair(jObject, "parameters", GetEnumerableBuilder(ParameterExpression));

            switch (nodeType) {
                case ExpressionType.Lambda:
                    return Expression.Lambda(body, tailCall, parameters);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.LambdaExpression"/>
        /// </summary>
        /// <param name="jToken">An abstract JSON token</param>
        /// <returns>A <see cref="System.Linq.Expressions.LambdaExpression"/></returns>
        private LambdaExpression LambdaExpression(JToken jToken)
        {
            if (jToken == null || jToken.Type != JTokenType.Object) {
                return null;
            }

            JObject jObject = (JObject) jToken;
            ExpressionType nodeType = ReadKeyValuePair(jObject, "nodeType", ParseEnum<ExpressionType>);
            string typeName = ReadKeyValuePair(jObject, "typeName", t => t.Value<string>());

            if (!typeName.Equals("lambda"))
                return null;

            return LambdaExpression(nodeType, jObject);
        }
    }
}
