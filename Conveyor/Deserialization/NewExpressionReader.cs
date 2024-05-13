using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.NewExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.NewExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.NewExpression"/></returns>
        private NewExpression NewExpression(
                ExpressionType nodeType, 
                JObject jObject
            )
        {
            ConstructorInfo constructor = ReadKeyValuePair(
                                                    jObject, 
                                                    "constructor", 
                                                    GetConstructorInfo
                                                );

            IEnumerable<Expression> arguments = ReadKeyValuePair(
                                                        jObject, 
                                                        "arguments", 
                                                        GetEnumerableBuilder(DeserializeAndBuildExpression)
                                                    );

            IEnumerable<MemberInfo> members = ReadKeyValuePair(
                                                        jObject, 
                                                        "members", 
                                                        GetEnumerableBuilder(GetMemberInfo)
                                                    );

            switch (nodeType) {
                case ExpressionType.New:
                    if (arguments == null) {
                        if (members == null)
                            return Expression.New(constructor);

                        return Expression.New(constructor, new Expression[0], members);
                    }

                    if (members == null)
                        return Expression.New(constructor, arguments);

                    return Expression.New(constructor, arguments, members);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
