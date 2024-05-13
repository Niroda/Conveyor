using System;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.UnaryExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.UnaryExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="type">The result type of the block</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.UnaryExpression"/></returns>
        private UnaryExpression UnaryExpression(
                ExpressionType nodeType,
                Type type,
                JObject jObject
            )
        {
            Expression operand = ReadKeyValuePair(
                                            jObject,
                                            "operand",
                                            DeserializeAndBuildExpression
                                        );

            MethodInfo method = ReadKeyValuePair(
                                            jObject,
                                            "method",
                                            GetMethodInfo
                                        );

            switch (nodeType)
            {
                case ExpressionType.ArrayLength: return Expression.ArrayLength(operand);
                case ExpressionType.Convert: return Expression.Convert(operand, type, method);
                case ExpressionType.ConvertChecked: return Expression.ConvertChecked(operand, type, method);
                case ExpressionType.Decrement: return Expression.Decrement(operand, method);
                case ExpressionType.Increment: return Expression.Increment(operand, method);
                case ExpressionType.IsFalse: return Expression.IsFalse(operand, method);
                case ExpressionType.IsTrue: return Expression.IsTrue(operand, method);
                case ExpressionType.Negate: return Expression.Negate(operand, method);
                case ExpressionType.NegateChecked: return Expression.NegateChecked(operand, method);
                case ExpressionType.Not: return Expression.Not(operand, method);
                case ExpressionType.OnesComplement: return Expression.OnesComplement(operand, method);
                case ExpressionType.PostDecrementAssign: return Expression.PostDecrementAssign(operand, method);
                case ExpressionType.PostIncrementAssign: return Expression.PostIncrementAssign(operand, method);
                case ExpressionType.PreDecrementAssign: return Expression.PreDecrementAssign(operand, method);
                case ExpressionType.PreIncrementAssign: return Expression.PreIncrementAssign(operand, method);
                case ExpressionType.Quote: return Expression.Quote(operand);
                case ExpressionType.Throw: return Expression.Throw(operand, type);
                case ExpressionType.TypeAs: return Expression.TypeAs(operand, type);
                case ExpressionType.UnaryPlus: return Expression.UnaryPlus(operand, method);
                case ExpressionType.Unbox: return Expression.Unbox(operand, type);
                default: throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
