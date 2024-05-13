using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    partial class ExpressionDeserializer
    {
        private static readonly Dictionary<Assembly, Dictionary<string, Dictionary<string, Type>>>
            _cachedTypes = new Dictionary<Assembly, Dictionary<string, Dictionary<string, Type>>>();

        private static readonly Dictionary<Type, Dictionary<string, Dictionary<string, ConstructorInfo>>>
            _cachedConstructors = new Dictionary<Type, Dictionary<string, Dictionary<string, ConstructorInfo>>>();

        /// <summary>
        /// Gets the type of the given <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// </summary>
        /// <param name="jToken">JSON token to look for its contain type</param>
        /// <returns>Type of the contain type</returns>
        private Type GetTokenType(JToken jToken)
        {
            // make sure that provided token is not null and is of type json token
            if (jToken == null || jToken.Type != JTokenType.Object)
                return null;

            JObject jObject = jToken as JObject;

            string assemblyName = ReadKeyValuePair(
                                            jObject,
                                            "assemblyName",
                                            x => x.Value<string>()
                                        );

            string typeName = ReadKeyValuePair(
                                        jObject,
                                        "typeName",
                                        x => x.Value<string>()
                                    );

            IEnumerable<Type> generic = ReadKeyValuePair(
                                                jObject,
                                                "genericArguments",
                                                GetEnumerableBuilder(GetTokenType)
                                            );

            Dictionary<string, Dictionary<string, Type>> assemblies;

            if (!_cachedTypes.TryGetValue(_assembly, out assemblies))
            {
                assemblies = new Dictionary<string, Dictionary<string, Type>>();
                _cachedTypes[_assembly] = assemblies;
            }

            Dictionary<string, Type> types;

            if (!assemblies.TryGetValue(assemblyName, out types))
            {
                types = new Dictionary<string, Type>();
                assemblies[assemblyName] = types;
            }

            Type type;
            // if the try to get the type from the local dictionary failed
            if (!types.TryGetValue(typeName, out type))
            {
                // try to get the type from the provided assembly when the current object created.
                type = _assembly.GetType(typeName);
                if (type == null)
                {
                    try
                    {
                        // try to load the assembly if all previous attempts failed
                        Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
                        type = assembly.GetType(typeName);
                    }
                    catch (Exception)
                    {
                        // at this point, we got only on chance.
                        // the crux of the matter here is that the reader runs on a different project.
                        // we have to to find the desired class from the given assembly in deserialize method.
                        // if nothing provided there, the default value will be used (object) which means it will fail.
                        type =  _assembly.GetTypes()
                                        .FirstOrDefault(x => typeName.EndsWith(x.Name));
                        if (type == null)
                            throw new MissingMemberException(
                                $"{typeName} couldn't be found. " +
                                $"It seems to be you are using Deserializer on a different project that serialized this JSON. " +
                                $"Pass the second parameter to be able to deserialize this object!");
                    }
                }

                types[typeName] = type ?? throw new Exception($"Type could not be found: {assemblyName} {typeName}");
            }
            
            if (generic != null && type.IsGenericTypeDefinition)
            {
                type = type.MakeGenericType(generic.ToArray());
            }

            return type;
        }

        /// <summary>
        /// Gets <see cref="System.Reflection.ConstructorInfo"/> for the given <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// </summary>
        /// <param name="jToken">JSON token to look for its constructor</param>
        /// <returns>A <see cref="System.Reflection.ConstructorInfo"/> instance</returns>
        private ConstructorInfo GetConstructorInfo(JToken jToken)
        {
            // make sure that provided token is not null and is of type json token
            if (jToken == null || jToken.Type != JTokenType.Object)
                return null;

            JObject jObject = jToken as JObject;

            Type type = ReadKeyValuePair(
                                jObject,
                                "type",
                                GetTokenType
                            );

            string name = ReadKeyValuePair(
                                    jObject,
                                    "name"
                                ).Value<string>();

            string signature = ReadKeyValuePair(
                                        jObject,
                                        "signature"
                                    ).Value<string>();

            ConstructorInfo constructor;
            Dictionary<string, Dictionary<string, ConstructorInfo>> firstCache;
            Dictionary<string, ConstructorInfo> secondCache;

            if (!_cachedConstructors.TryGetValue(type, out firstCache))
            {
                constructor = GetConstructorInfo(type, name, signature);

                secondCache = new Dictionary<
                    string, ConstructorInfo>(1) {
                        {signature, constructor}
                    };

                firstCache = new Dictionary<
                    string, Dictionary<
                        string, ConstructorInfo>>(1) {
                            {name, secondCache}
                        };

                _cachedConstructors[type] = firstCache;
            }
            else if (!firstCache.TryGetValue(name, out secondCache))
            {
                constructor = GetConstructorInfo(type, name, signature);

                secondCache = new Dictionary<string, ConstructorInfo>(1)
                {
                    { signature, constructor }
                };

                firstCache[name] = secondCache;
            }
            else if (!secondCache.TryGetValue(signature, out constructor))
            {
                constructor = GetConstructorInfo(type, name, signature);
                secondCache[signature] = constructor;
            }

            return constructor;
        }

        /// <summary>
        /// Gets <see cref="System.Reflection.ConstructorInfo"/> for the given <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// </summary>
        /// <param name="type">The type to find its constructor</param>
        /// <param name="name">Member name</param>
        /// <param name="signature">string representation for the constructor</param>
        /// <returns>A <see cref="System.Reflection.ConstructorInfo"/> instance</returns>
        private ConstructorInfo GetConstructorInfo(
                Type type,
                string name,
                string signature
            )
        {
            ConstructorInfo constructor = type.GetConstructors(
                                                    BindingFlags.Public | BindingFlags.Instance
                                                )
                                                .FirstOrDefault(
                                                    x =>
                                                        x.Name.Equals(name)
                                                        &&
                                                        x.ToString().Equals(signature)
                                                );

            if (constructor == null)
            {
                constructor = type.GetConstructors(
                                            BindingFlags.NonPublic | BindingFlags.Instance
                                        )
                                        .FirstOrDefault(
                                            x =>
                                                x.Name.Equals(name)
                                                &&
                                                x.ToString().Equals(signature)
                                        );

                if (constructor == null)
                {
                    throw new Exception(
                        $"Constructor for type \"{type.FullName}\" with signature \"{signature}\" could not be found"
                    );
                }
            }

            return constructor;
        }

        /// <summary>
        /// Gets <see cref="System.Reflection.MethodInfo"/> for the given <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// </summary>
        /// <param name="jToken">JSON token to look for its constructor</param>
        /// <returns>A <see cref="System.Reflection.MethodInfo"/> instance</returns>
        private MethodInfo GetMethodInfo(JToken jToken)
        {
            if (jToken == null || jToken.Type != JTokenType.Object)
                return null;

            JObject jObject = jToken as JObject;
            Type type = ReadKeyValuePair(
                                jObject, 
                                "type", 
                                GetTokenType
                            );
            string name = ReadKeyValuePair(
                                    jObject, 
                                    "name"
                                ).Value<string>();
            string signature = ReadKeyValuePair(
                                        jObject, 
                                        "signature"
                                    ).Value<string>();
            IEnumerable<Type> generic = ReadKeyValuePair(
                                                jObject, 
                                                "generic", 
                                                GetEnumerableBuilder(GetTokenType)
                                            );

            MethodInfo[] methods = type.GetMethods(
                                        BindingFlags.Public | BindingFlags.NonPublic |
                                        BindingFlags.Instance | BindingFlags.Static
                                    );
            MethodInfo method = methods.First(x => x.Name.Equals(name) && signature.Equals(x.ToString()));

            if (generic != null && method.IsGenericMethodDefinition)
                method = method.MakeGenericMethod(generic.ToArray());

            return method;
        }

        /// <summary>
        /// Gets <see cref="System.Reflection.PropertyInfo"/> for the given <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// </summary>
        /// <param name="jToken">JSON token to look for its constructor</param>
        /// <returns>A <see cref="System.Reflection.PropertyInfo"/> instance</returns>
        private PropertyInfo GetPropertyInfo(JToken jToken)
        {
            if (jToken == null || jToken.Type != JTokenType.Object)
                return null;

            JObject jObject = jToken as JObject;
            Type type = ReadKeyValuePair(
                                jObject, 
                                "type", 
                                GetTokenType
                            );

            string name = ReadKeyValuePair(
                                    jObject, 
                                    "name"
                                ).Value<string>();

            string signature = ReadKeyValuePair(
                                        jObject, 
                                        "signature"
                                    ).Value<string>();

            PropertyInfo[] propertyInfos = type.GetProperties(
                                                    BindingFlags.Public | BindingFlags.NonPublic |
                                                    BindingFlags.Instance | BindingFlags.Static
                                                );
            return propertyInfos.First(x => x.Name.Equals(name) && signature.Equals(x.ToString()));
        }


        /// <summary>
        /// Gets <see cref="System.Reflection.MemberInfo"/> for the given <see cref="Newtonsoft.Json.Linq.JToken"/>
        /// </summary>
        /// <param name="jToken">JSON token to look for its constructor</param>
        /// <returns>A <see cref="System.Reflection.MemberInfo"/> instance</returns>
        private MemberInfo GetMemberInfo(JToken jToken)
        {
            if (jToken == null || jToken.Type != JTokenType.Object)
                return null;

            JObject jObject = jToken as JObject;

            Type type = ReadKeyValuePair(
                                jObject,
                                "type",
                                GetTokenType
                            );

            string name = ReadKeyValuePair(
                                    jObject,
                                    "name"
                                ).Value<string>();

            string signature = ReadKeyValuePair(
                                            jObject,
                                            "signature"
                                    ).Value<string>();

            MemberTypes memberType = (MemberTypes)ReadKeyValuePair(
                                                            jObject,
                                                            "memberType"
                                                        ).Value<int>();

            MemberInfo[] membersInfo = type.GetMembers(
                                                BindingFlags.Public | BindingFlags.NonPublic |
                                                BindingFlags.Instance | BindingFlags.Static
                                            );
            try
            {
                return membersInfo.First(
                            x =>
                                x.MemberType == memberType
                                &&
                                name.Equals(x.Name)
                                &&
                                signature.Equals(x.ToString())
                        );
            }
            catch (Exception)
            {
                throw new MissingMemberException(
                        $"Make sure that existing properties in your view model are the same as existing properties in your model"
                    );
            }
        }
    }
}
