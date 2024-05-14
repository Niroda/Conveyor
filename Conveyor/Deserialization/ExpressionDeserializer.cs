using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{
    /// <summary>
    /// Used to create a deserialized Expression
    /// </summary>
    internal sealed partial class ExpressionDeserializer
    {
        /// <summary>
        /// The assembly that loaded this object
        /// </summary>
        private readonly Assembly _assembly;

        /// <summary>
        /// Deserializes the given expression
        /// </summary>
        /// <param name="assembly">The assembly that loaded this object</param>
        /// <param name="token">The abstract JSON token</param>
        /// <returns>Deserialized <see cref="System.Linq.Expressions.Expression"/></returns>
        public static Expression Deserialize(
                    Assembly assembly,
                    JToken token
            )
        {
            ExpressionDeserializer expressionDeserializer = new ExpressionDeserializer(assembly);
            return expressionDeserializer.DeserializeAndBuildExpression(token);
        }

        /// <summary>
        /// Just a constructor :)
        /// </summary>
        /// <param name="assembly">The assembly that loaded this object.</param>
        private ExpressionDeserializer(Assembly assembly) => _assembly = assembly;

        /// <summary>
        /// Creates an instance of the specified .NET type from the <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// </summary>
        /// <param name="jToken">Represents an abstract JSON token</param>
        /// <param name="type">The object type that the token will be deserialized to.</param>
        /// <returns>The new object created from the JSON value.</returns>
        private object ToObject(JToken jToken, Type type) => jToken.ToObject(type);

        /// <summary>
        /// Reads the property value
        /// </summary>
        /// <param name="jObject">JSON object to read values from</param>
        /// <param name="propName">Property name to be read</param>
        /// <returns>An instance of <see cref="Newtonsoft.Json.Linq.JToken"/></returns>
        private JToken ReadKeyValuePair(JObject jObject, string propName) => jObject.Property(propName)?.Value;

        /// <summary>
        /// Reads the property value and applies the given <see cref="System.Func{JToken, T}"/>
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="jObject">JSON object to read values from</param>
        /// <param name="propName">Property name to be read</param>
        /// <param name="producer">A <see cref="System.Func{T, TResult}"/> to tell what to do with the value</param>
        /// <returns>An object that represent property value of the selected type T</returns>
        private T ReadKeyValuePair<T>(JObject jObject, string propName, Func<JToken, T> producer)
        {
            JProperty jProperty = jObject.Property(propName);
            if (jProperty == null)
                return producer(null);

            return producer(jProperty.Value);
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <typeparam name="T">The type of enum</typeparam>
        /// <param name="jToken">An abstract JSON token</param>
        /// <returns>An object of type enumType whose value is represented by value.</returns>
        private T ParseEnum<T>(JToken jToken) => (T)Enum.Parse(typeof(T), jToken.Value<string>());

        /// <summary>
        /// Builds a <see cref="System.Func{T, TResult}"/> that reads all properties of given <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// </summary>
        /// <typeparam name="T">The return type of the <see cref="System.Func{T, TResult}"/></typeparam>
        /// <param name="result">A <see cref="System.Func{T, TResult}"/> that takes <see cref="Newtonsoft.Json.Linq.JToken"/> and returns desired value(s)</param>
        /// <returns>A <see cref="System.Func{JToken, IEnumerable}"/></returns>
        private Func<JToken, IEnumerable<T>> GetEnumerableBuilder<T>(Func<JToken, T> result) => (JToken jToken) =>
        {
            // make sure that given token is not null and is of type JSON array
            if (jToken == null || jToken.Type != JTokenType.Array) return null;

            JArray jArray = (JArray)jToken;
            return jArray.Select(result);
        };

        /// <summary>
        /// Deserializes the JSON token and builds up the <see cref="System.Linq.Expressions.Expression"/> instructions
        /// </summary>
        /// <param name="jToken">An abstract JSON token</param>
        /// <returns>Deserialized <see cref="System.Linq.Expressions.Expression"/></returns>
        private Expression DeserializeAndBuildExpression(JToken jToken)
        {
            // make sure that given token is not null and is of the type JSON object
            if (jToken == null || jToken.Type != JTokenType.Object)
                return null;
            // cast the json token into json object
            JObject jObject = (JObject)jToken;
            // read the type of node expression
            ExpressionType nodeType = ReadKeyValuePair(jObject, "nodeType", ParseEnum<ExpressionType>);
            Type type = ReadKeyValuePair(jObject, "type", GetTokenType);
            string typeName = ReadKeyValuePair(jObject, "typeName", t => t.Value<string>());

            switch (typeName)
            {
                case "binary": return BinaryExpression(nodeType, jObject);
                case "block": return BlockExpression(nodeType, type, jObject);
                case "conditional": return ConditionalExpression(nodeType, type, jObject);
                case "constant": return ConstantExpression(nodeType, type, jObject);
                case "default": return DefaultExpression(nodeType, type, jObject);
                case "index": return IndexExpression(nodeType, jObject);
                case "invocation": return InvocationExpression(nodeType, jObject);
                case "lambda": return LambdaExpression(nodeType, jObject);
                case "member": return MemberExpression(nodeType, jObject);
                case "methodCall": return MethodCallExpression(nodeType, jObject);
                case "newArray": return NewArrayExpression(nodeType, jObject);
                case "new": return NewExpression(nodeType, jObject);
                case "parameter": return ParameterExpression(nodeType, type, jObject);
                case "runtimeVariables": return RuntimeVariablesExpression(nodeType, jObject);
                case "typeBinary": return TypeBinaryExpression(nodeType, jObject);
                case "unary": return UnaryExpression(nodeType, type, jObject);
            }
            throw new NotSupportedException($"Specified method {typeName} is not supported right now.");
        }
    }
}
