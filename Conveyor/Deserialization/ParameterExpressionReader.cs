using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.ParameterExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        private readonly Dictionary<string, ParameterExpression> _parameters = new Dictionary<string, ParameterExpression>();


        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.ParameterExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="type">The result type of the block</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.ParameterExpression"/></returns>
        private ParameterExpression ParameterExpression(
                ExpressionType nodeType,
                Type type,
                JObject jObject
            )
        {
            if (nodeType != ExpressionType.Parameter)
                throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");

            string key = ReadKeyValuePair(jObject, "name", t => t.Value<string>());

            ParameterExpression result;
            // check if there is any row in the dictionary with the same key
            if (_parameters.TryGetValue(key, out result))
                return result;

            result = Expression.Parameter(type, key);
            _parameters[key] = result;
            return result;
        }
        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.ParameterExpression"/>
        /// </summary>
        /// <param name="jToken">An abstract json object</param>
        /// <returns>A <see cref="System.Linq.Expressions.ParameterExpression"/></returns>
        private ParameterExpression ParameterExpression(JToken jToken)
        {
            if (jToken == null || jToken.Type != JTokenType.Object)
                return null;

            JObject jObject = jToken as JObject;
            ExpressionType nodeType = ReadKeyValuePair(jObject, "nodeType", ParseEnum<ExpressionType>);
            Type type = ReadKeyValuePair(jObject, "type", GetTokenType);
            string typeName = ReadKeyValuePair(jObject, "typeName", t => t.Value<string>());

            if (!typeName.Equals("parameter"))
                return null;

            return ParameterExpression(nodeType, type, jObject);
        }
    }
}
