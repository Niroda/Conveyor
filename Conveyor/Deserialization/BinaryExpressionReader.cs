using System;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.BinaryExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.BinaryExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="obj">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.BinaryExpression"/></returns>
        private BinaryExpression BinaryExpression(
                ExpressionType nodeType,
                JObject obj
            )
        {

            Expression left = ReadKeyValuePair(obj, "left", DeserializeAndBuildExpression);
            Expression right = ReadKeyValuePair(obj, "right", DeserializeAndBuildExpression);
            MethodInfo methodInfo = ReadKeyValuePair(obj, "method", GetMethodInfo);
            LambdaExpression lambdaExpression = ReadKeyValuePair(obj, "conversion", LambdaExpression);
            bool isLiftedToNull = ReadKeyValuePair(obj, "liftToNull").Value<bool>();

            switch (nodeType)
            {
                case ExpressionType.Add: return Expression.Add(left, right, methodInfo);
                case ExpressionType.AddAssign: return Expression.AddAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.AddAssignChecked: return Expression.AddAssignChecked(left, right, methodInfo, lambdaExpression);
                case ExpressionType.AddChecked: return Expression.AddChecked(left, right, methodInfo);
                case ExpressionType.And: return Expression.And(left, right, methodInfo);
                case ExpressionType.AndAlso: return Expression.AndAlso(left, right, methodInfo);
                case ExpressionType.AndAssign: return Expression.AndAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.ArrayIndex: return Expression.ArrayIndex(left, right);
                case ExpressionType.Assign: return Expression.Assign(left, right);
                case ExpressionType.Coalesce: return Expression.Coalesce(left, right, lambdaExpression);
                case ExpressionType.Divide: return Expression.Divide(left, right, methodInfo);
                case ExpressionType.DivideAssign: return Expression.DivideAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.Equal: return Expression.Equal(left, right, isLiftedToNull, methodInfo);
                case ExpressionType.ExclusiveOr: return Expression.ExclusiveOr(left, right, methodInfo);
                case ExpressionType.ExclusiveOrAssign: return Expression.ExclusiveOrAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.GreaterThan: return Expression.GreaterThan(left, right, isLiftedToNull, methodInfo);
                case ExpressionType.GreaterThanOrEqual: return Expression.GreaterThanOrEqual(left, right, isLiftedToNull, methodInfo);
                case ExpressionType.LeftShift: return Expression.LeftShift(left, right, methodInfo);
                case ExpressionType.LeftShiftAssign: return Expression.LeftShiftAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.LessThan: return Expression.LessThan(left, right, isLiftedToNull, methodInfo);
                case ExpressionType.LessThanOrEqual: return Expression.LessThanOrEqual(left, right, isLiftedToNull, methodInfo);
                case ExpressionType.Modulo: return Expression.Modulo(left, right, methodInfo);
                case ExpressionType.ModuloAssign: return Expression.ModuloAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.Multiply: return Expression.Multiply(left, right, methodInfo);
                case ExpressionType.MultiplyAssign: return Expression.MultiplyAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.MultiplyAssignChecked: return Expression.MultiplyAssignChecked(left, right, methodInfo, lambdaExpression);
                case ExpressionType.MultiplyChecked: return Expression.MultiplyChecked(left, right, methodInfo);
                case ExpressionType.NotEqual: return Expression.NotEqual(left, right, isLiftedToNull, methodInfo);
                case ExpressionType.Or: return Expression.Or(left, right, methodInfo);
                case ExpressionType.OrAssign: return Expression.OrAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.OrElse: return Expression.OrElse(left, right, methodInfo);
                case ExpressionType.Power: return Expression.Power(left, right, methodInfo);
                case ExpressionType.PowerAssign: return Expression.PowerAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.RightShift: return Expression.RightShift(left, right, methodInfo);
                case ExpressionType.RightShiftAssign: return Expression.RightShiftAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.Subtract: return Expression.Subtract(left, right, methodInfo);
                case ExpressionType.SubtractAssign: return Expression.SubtractAssign(left, right, methodInfo, lambdaExpression);
                case ExpressionType.SubtractAssignChecked: return Expression.SubtractAssignChecked(left, right, methodInfo, lambdaExpression);
                case ExpressionType.SubtractChecked: return Expression.SubtractChecked(left, right, methodInfo);
                default: throw new NotSupportedException($"Specified type '{nodeType}' is not support right now.");
            }
        }
    }
}
