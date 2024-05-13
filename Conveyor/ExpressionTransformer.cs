using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Conveyor.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Conveyor
{
    /// <summary>
    /// Transformer class used to create a modified type of <see cref="Newtonsoft.Json.JsonConvert"/>
    /// DON'T USE THIS CLASS DIRECTLY ON A WEB API APPLICATION!!!
    /// USE EXTENSIONS FROM <see cref="Conveyor.Utility.HttpClientExtensions"/> INSTEAD!
    /// </summary>
    public class ExpressionTransformer : JsonConverter
    {
        /// <summary>
        /// Types can be serialized and deserialized
        /// </summary>
        private readonly Type expressionType = typeof (Expression);

        /// <summary>
        /// A singleton instance of <see cref="Newtonsoft.Json.JsonSerializerSettings"/>
        /// </summary>
        private JsonSerializerSettings settings;

        /// <summary>
        /// The assembly that has called/used this class.
        /// </summary>
        private readonly Assembly _assembly;

        /// <summary>
        /// A private constructor to make a singleton <see cref="Newtonsoft.Json.JsonSerializerSettings"/> instance
        /// </summary>
        /// <param name="assembly">The assembly this instance will be created for.</param>
        private ExpressionTransformer(Assembly assembly) => _assembly = assembly;

        /// <summary>
        /// Creates a new instance of <see cref="Newtonsoft.Json.JsonSerializerSettings"/> after injecting wrapper class assembly
        /// </summary>
        /// <param name="type">Type of the class that will be serialized/deserialized in the given expression</param>
        /// <returns>An instance of <see cref="Newtonsoft.Json.JsonSerializerSettings"/></returns>
        private static ExpressionTransformer Create(Type type = null)
        {
            // get passed class assembly, if nothing provided, will be set to object.
            Assembly assembly = Assembly.GetAssembly(type ?? typeof(object));
            // create a new instance of ExpressionTransformer
            ExpressionTransformer expressionTransformer = new ExpressionTransformer(assembly)
            {
                // initialize a new JsonSerializerSettings instance
                settings = new JsonSerializerSettings()
            };
            // add our transformer to converters collection in json settings
            expressionTransformer.settings.Converters.Add(expressionTransformer);

            return expressionTransformer;
        }


        /// <summary>
        /// Serializes given expression
        /// DON'T USE THIS METHOD DIRECTLY ON A WEB API APPLICATION!!! <para />
        /// USE EXTENSIONS FROM <see cref="Conveyor.Utility.HttpClientExtensions"/> INSTEAD! <para />
        /// </summary>
        /// <typeparam name="T">Type of the expression</typeparam>
        /// <param name="expression">An instance of <see cref="System.Linq.Expressions.Expression"/> to be serialized</param>
        /// <param name="type">Type of the class that will be serialized in the given expression</param>
        /// <returns>Serialized <see cref="System.Linq.Expressions.Expression"/>.</returns>
        public static string SerializeExpression<T>(T expression, Type type = null) where T : Expression
        {
            // serialize given expression
            string json = JsonConvert.SerializeObject(
                                        expression,
                                        Create(type)
                                    );

            return StringCipher.Encrypt(json);
        }


        /// <summary>
        /// Deserialize given json
        /// DON'T USE THIS METHOD DIRECTLY ON A WEB API APPLICATION!!! <para />
        /// USE EXTENSIONS FROM <see cref="Conveyor.Utility.HttpClientExtensions"/> INSTEAD! <para />
        /// MAKE SURE YOU PASS THE PARAMETER "type" IN CASE YOU USE THIS FUNCTION IN A DIFFERENT PROJECT THAN THE ONE WHICH SERIALIZED THE EXPRESSION
        /// </summary>
        /// <typeparam name="TEntity">Type of the expression to be returned</typeparam>
        /// <typeparam name="TViewModel">Type of the passed expression</typeparam>
        /// <param name="encryptedJson">Encrypted/Serialized expression to be deserialized</param>
        /// <param name="type">Type of the class that will be deserialized in the given expression</param>
        /// <returns>Deserialized <see cref="System.Linq.Expressions.Expression"/>.</returns>
        public static Expression<Func<TEntity, bool>> DeserializeExpression<TEntity, TViewModel>(string encryptedJson, Type type = null)
            where TEntity : class where TViewModel : class
        {
            // It's expensive to use try/catch block, but we have to due to the place that this function will be used at.
            try
            {

                string json = StringCipher.Decrypt(encryptedJson);

                FixAssemblyInfo(ref json);

                Expression<Func<TViewModel, bool>> expression = JsonConvert.DeserializeObject<Expression<Func<TViewModel, bool>>>(
                                                                            json,
                                                                            Create(type)
                                                                        );
                // get the new parameter type
                ParameterExpression param = Expression.Parameter(typeof(TEntity));
                // visiting body of original expression that gives us body of the new expression
                Expression body = new ParameterChanger<TEntity>(param).Visit(expression.Body);
                //generating lambda expression form body and parameter 
                return Expression.Lambda<Func<TEntity, bool>>(body, param);
            }
            catch (Exception ex)
            {
                if(ex is PlatformNotSupportedException || ex is CryptographicException)
                    throw;

                return null;
            }
        }

        /// <summary>
        /// Updates the namespace for all core functions/expressions to the current.
        /// The main problem is that in core, <see cref="Expression"/> is packaged in System.Private.CoreLib assembly, and in framework is packaged in mscorelib namespace.
        /// </summary>
        /// <param name="json"></param>
        private static void FixAssemblyInfo(ref string json)
        {
            // get current assembly to determine whether is core or framework
            AssemblyName assembly = typeof(string).Assembly.GetName();

            // a double check in case the client uses core and the server uses framework, or vice versa.
            // in such scenario, we must update assembly path, other wise we will get a runtime exception.
            if (json.IndexOf(assembly.ToString()) > 0)
                return;

            // current assembly signature to be used
            string replaceWith = $"\"assemblyName\":\"{assembly.ToString()}\",";
            // look for this pattern
            string pattern = @"""assemblyName"":""(mscorlib|System.Private.CoreLib)[a-zA-Z0-9,.=\s]+"",";
            // replace all occurrences with the new signature
            json = Regex.Replace(json, pattern, replaceWith, RegexOptions.Multiline);
        }


#region Overridden functions

        /// <summary>
        /// Checks whether given type can be serialized
        /// </summary>
        /// <param name="objectType"><see cref="System.Type"/> of given object to be serialized</param>
        /// <returns>True if the type of given object to be serialized is either <see cref="Expression"/> or a derived class</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == expressionType || objectType.IsSubclassOf(expressionType);
        }

        /// <summary>
        /// Serializes given object to JSON
        /// </summary>
        /// <param name="writer"><see cref="Newtonsoft.Json.JsonWriter"/> instance</param>
        /// <param name="value">Given value to be serialized</param>
        /// <param name="serializer"><see cref="Newtonsoft.Json.JsonSerializer"/> instance</param>
        public override void WriteJson(
            JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!CanConvert(value.GetType()))
                throw new NotSupportedException($"{value.GetType()} is not supported");

            Serialization.ExpressionSerializer.Serialize(writer, serializer, (Expression) value);
        }

        /// <summary>
        /// Reads serialized JSON and parse it to its original type
        /// </summary>
        /// <param name="reader"><see cref="Newtonsoft.Json.JsonReader"/> Reader to read the serialized JSON</param>
        /// <param name="objectType"><see cref="System.Type"/> of given object to be deserialized to</param>
        /// <param name="existingValue">Given value to be parsed</param>
        /// <param name="serializer"><see cref="Newtonsoft.Json.JsonSerializer"/> instance</param>
        /// <returns>Parsed object</returns>
        public override object ReadJson(
                JsonReader reader, 
                Type objectType,
                object existingValue, 
                JsonSerializer serializer
            )
        {
            if (!CanConvert(objectType))
                throw new NotSupportedException($"{objectType} is not supported");

            return Deserialization.ExpressionDeserializer.Deserialize(this._assembly, JToken.ReadFrom(reader));
        }

#endregion

    }
}
