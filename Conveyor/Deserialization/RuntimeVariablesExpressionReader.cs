using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace Conveyor.Deserialization
{

    /// <summary>
    /// Used to create a deserialized <see cref="System.Linq.Expressions.RuntimeVariablesExpression"/>
    /// </summary>
    partial class ExpressionDeserializer
    {

        /// <summary>
        /// Reads required properties and builds a <see cref="System.Linq.Expressions.RuntimeVariablesExpression"/>
        /// </summary>
        /// <param name="nodeType">Describes the node types for the nodes of an expression tree</param>
        /// <param name="jObject">JSON object to read from</param>
        /// <returns>A <see cref="System.Linq.Expressions.RuntimeVariablesExpression"/></returns>
        private RuntimeVariablesExpression RuntimeVariablesExpression(
                ExpressionType nodeType,
                JObject jObject
            )
        {
            if (nodeType != ExpressionType.RuntimeVariables)
                throw new NotSupportedException($"Specified type '{nodeType}' is not supported.");

            IEnumerable<ParameterExpression> variables = ReadKeyValuePair(
                                                                    jObject,
                                                                    "variables",
                                                                    GetEnumerableBuilder(ParameterExpression)
                                                                );

            return Expression.RuntimeVariables(variables);
        }
    }
}
