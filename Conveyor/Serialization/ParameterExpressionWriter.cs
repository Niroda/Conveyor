using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized ParameterExpression
    /// </summary>
    partial class ExpressionSerializer
    {
        /// <summary>
        /// Stores a collection of key value pair that represents arguments
        /// </summary>
        private readonly Dictionary<ParameterExpression, string> _params = new Dictionary<ParameterExpression, string>();


        /// <summary>
        /// Checks whether is ParameterExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a ParameterExpression, False otherwise.</returns>
        private bool ParameterExpression(Expression expression)
        {
            // cast the expression in to a ParameterExpression
            ParameterExpression parameterExpression = expression as ParameterExpression;
            // if the variable parameterExpression is null, then given expression isn't a ParameterExpression
            if (parameterExpression == null)
                return false;

            string name;
            // check whether current parameter exists in the dictionary
            if (!_params.TryGetValue(parameterExpression, out name)) {
                name = parameterExpression.Name ?? "p_" + Guid.NewGuid().ToString("N");
                _params[parameterExpression] = name;
            }

            WriteKeyValuePair("typeName", "parameter");
            WriteKeyValuePair("name", name);

            return true;
        }
    }
}
