using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.TypeBinaryExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.TypeBinaryExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="obj">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.TypeBinaryExpression"/></returns>
        private TypeBinaryExpression TypeBinaryExpression(
                ExpressionType nodeType,
                JObject obj
            )
        {
            Expression expression = ReadKeyValuePair(
                                        obj, 
                                        "expression", 
                                        DeserializeAndBuildExpression
                                    );
            Type typeOperand = ReadKeyValuePair(
                                        obj, 
                                        "typeOperand", 
                                        GetTokenType
                                    );

            switch (nodeType)
            {
                case ExpressionType.TypeIs:
                    return Expression.TypeIs(expression, typeOperand);
                case ExpressionType.TypeEqual:
                    return Expression.TypeEqual(expression, typeOperand);
                default:
                    throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");
            }
        }
    }
}
