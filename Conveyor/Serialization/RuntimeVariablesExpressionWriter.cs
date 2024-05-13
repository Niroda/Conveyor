using System;
using System.Linq.Expressions;

namespace Conveyor.Serialization
{

    /// <summary>
    /// Used to create a serialized RuntimeVariablesExpression
    /// </summary>
    partial class ExpressionSerializer
    {

        /// <summary>
        /// Checks whether is RuntimeVariablesExpression and writes it, if it is.
        /// </summary>
        /// <param name="expression">Given expression to be serialized</param>
        /// <returns>True if given expression is a RuntimeVariablesExpression, False otherwise.</returns>
        private bool RuntimeVariablesExpression(Expression expression)
        {
            // cast the expression in to a BlockExpression
            RuntimeVariablesExpression runtimeVariablesExpression = expression as RuntimeVariablesExpression;
            // if the variable runtimeVariablesExpression is null, then given expression isn't a RuntimeVariablesExpression
            if (runtimeVariablesExpression == null) { return false; }

            WriteKeyValuePair("typeName", "runtimeVariables");
            WriteKeyValuePair("variables", Apply(runtimeVariablesExpression.Variables, GetExpressionAction));

            return true;
        }
    }
}
