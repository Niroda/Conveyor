using System;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.MemberExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.MemberExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.MemberExpression"/></returns>
        private MemberExpression MemberExpression(
                ExpressionType nodeType, 
                JObject jObject
            )
        {
            Expression expression = ReadKeyValuePair(
                                        jObject, 
                                        "expression", 
                                        DeserializeAndBuildExpression
                                    );
            MemberInfo memeberInfo = ReadKeyValuePair(
                                    jObject, 
                                    "member", 
                                    GetMemberInfo
                                );

            switch (nodeType) {
                case ExpressionType.MemberAccess:
                    return Expression.MakeMemberAccess(expression, memeberInfo);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
