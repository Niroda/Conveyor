using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Conveyor.Utility;
using Newtonsoft.Json;

namespace Conveyor.Serialization
{
    /// <summary>
    /// Used to create a serialized Expression
    /// </summary>
    internal sealed partial class ExpressionSerializer
    {

        private ReadOnlyCollection<ParameterExpression> _parameters;

        /// <summary>
        /// Represents a writer that provides a fast, non-cached, forward-only way of generating JSON data
        /// </summary>
        private readonly JsonWriter _jsonWriter;

        /// <summary>
        /// Serializes and deserializes objects into and from the JSON format.
        /// </summary>
        private readonly JsonSerializer _jsonSerializer;

        /// <summary>
        /// Serializes the given expression
        /// </summary>
        /// <param name="jsonWriter">
        /// A <see cref="Newtonsoft.Json.JsonWriter"/> instance that represents a writer that provides a fast, non-cached, forward-only way of generating 
        /// JSON data.
        /// </param>
        /// <param name="jsonSerializer">
        /// A <see cref="Newtonsoft.Json.JsonSerializer"/> instance that serializes and deserializes objects into and from the JSON format. 
        /// </param>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be serialized</param>
        public static void Serialize(
                JsonWriter jsonWriter,
                JsonSerializer jsonSerializer,
                Expression expression
            )
        {
            new ExpressionSerializer(jsonWriter, jsonSerializer)
                    .WriteExpressionTree(expression);
        }

        /// <summary>
        /// Just a constructor :)
        /// </summary>
        /// <param name="jsonWriter">
        /// A <see cref="Newtonsoft.Json.JsonWriter"/> instance that represents a writer that provides a fast, non-cached, forward-only way of generating 
        /// JSON data.
        /// </param>
        /// <param name="jsonSerializer">
        /// A <see cref="Newtonsoft.Json.JsonSerializer"/> instance that serializes and deserializes objects into and from the JSON format. 
        /// </param>
        private ExpressionSerializer(JsonWriter jsonWriter, JsonSerializer jsonSerializer)
        {
            _jsonWriter = jsonWriter;
            _jsonSerializer = jsonSerializer;
        }


        /// <summary>
        /// Creates an <see cref="System.Action"/> that serializes the specified System.Object and writes the JSON structure using the
        /// specified <see cref="Newtonsoft.Json.JsonWriter"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Action Serialize(object value, Type type) => () => _jsonSerializer.Serialize(_jsonWriter, value, type);

        /// <summary>
        /// Gets an action that writes enum values
        /// </summary>
        /// <typeparam name="TEnum">Type of enum</typeparam>
        /// <param name="value">enumeration instance</param>
        /// <returns>An <see cref="System.Action"/> instance that calls
        /// <see cref="Conveyor.Serialization.ExpressionSerializer.WriteEnumValue{TEnum}(TEnum)"/>
        /// </returns>
        private Action GetEnumAction<TEnum>(TEnum value) => () => WriteEnumValue(value);

        /// <summary>
        /// Writes enum values from given type
        /// </summary>
        /// <typeparam name="TEnum">Type of enum</typeparam>
        /// <param name="enumObject">The enumeration that contains the desired value to be read from</param>
        private void WriteEnumValue<TEnum>(TEnum enumObject) => _jsonWriter.WriteValue(Enum.GetName(typeof(TEnum), enumObject));


        /// <summary>
        /// Gets an action that writes JSON object from the beginning to the end
        /// </summary>
        /// <param name="expression">The expression to be serialized</param>
        /// <returns>An <see cref="System.Action"/> instance that calls 
        /// <see cref="Conveyor.Serialization.ExpressionSerializer.WriteExpressionTree(System.Linq.Expressions.Expression)"/>
        /// </returns>
        private Action GetExpressionAction(Expression expression) => () => WriteExpressionTree(expression);


        /// <summary>
        /// Writes the property name and assigns its value
        /// </summary>
        /// <param name="key">Property name</param>
        /// <param name="value">Value of the given property</param>
        private void WriteKeyValuePair(string key, object value)
        {
            _jsonWriter.WritePropertyName(key);
            _jsonWriter.WriteValue(value);
        }

        /// <summary>
        /// Writes the property name and applies the given <see cref="System.Action"/>
        /// </summary>
        /// <param name="propName">Property name to be written</param>
        /// <param name="producer">The <see cref="System.Action"/> to be invoked to write the value</param>
        private void WriteKeyValuePair(string propName, Action producer)
        {
            _jsonWriter.WritePropertyName(propName);
            producer();
        }

        /// <summary>
        /// Writes JSON object from the beginning to the end
        /// </summary>
        /// <param name="expression">The expression to be serialized</param>
        private void WriteExpressionTree(Expression expression)
        {
            if(_parameters == null)
                _parameters = (expression as LambdaExpression).Parameters;

            // if the expression is null, assign null to the property in the serialized object
            if (expression == null)
            {
                _jsonWriter.WriteNull();
                return;
            }

            while (expression.CanReduce)
            {
                expression = expression.Reduce();
            }


            if ((expression.NodeType == ExpressionType.Call 
                || 
                expression.NodeType == ExpressionType.MemberAccess) 
                && 
                IsConstantExpression(expression))
            {
                object value = MemberAccessResolver.GetConstantValue(expression); //GetValue(expression);
                ConstantExpression constantExpression = Expression.Constant(value);
                expression = constantExpression;
            }


            // start writing the beginning of JSON object
            _jsonWriter.WriteStartObject();
            // write other common properties such as NodeType and Type
            WriteKeyValuePair("nodeType", GetEnumAction(expression.NodeType));
            WriteKeyValuePair("type", InvokeTypeWriter(expression.Type));
            // check if given expression uses one of the supported methods.
            if (BinaryExpression(expression)
                || BlockExpression(expression)
                || ConditionalExpression(expression)
                || ConstantExpression(expression)
                || DefaultExpression(expression)
                || IndexExpression(expression)
                || InvocationExpression(expression)
                || LambdaExpression(expression)
                || MemberExpression(expression)
                || MethodCallExpression(expression)
                || NewArrayExpression(expression)
                || NewExpression(expression)
                || ParameterExpression(expression)
                || RuntimeVariablesExpression(expression)
                || TypeBinaryExpression(expression)
                || UnaryExpression(expression))
            {
                _jsonWriter.WriteEndObject();
            }
            else
            {
                throw new NotSupportedException("Specified method is not supported right now.");
            }

        }

        /// <summary>
        /// Used in other partial classes to serialize their inner as well as outer objects/expression body.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="collection">A collection of <see cref="Collection{T}"/></param>
        /// <param name="consumer">The function to handle objects</param>
        /// <returns>An <see cref="System.Action"/> instance</returns>
        private Action Apply<T>(ICollection<T> collection, Func<T, Action> consumer) => () =>
        {
            if (collection == null)
            {
                _jsonWriter.WriteNull();
            }
            else
            {
                _jsonWriter.WriteStartArray();
                foreach (T item in collection)
                {
                    consumer(item)();
                }
                _jsonWriter.WriteEndArray();
            }
        };


        private object GetValue(Expression member)
        {
            UnaryExpression objectMember = Expression.Convert(member, typeof(object));

            Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);

            Func<object> getter = getterLambda.Compile();

            return getter();
        }

    }
}
