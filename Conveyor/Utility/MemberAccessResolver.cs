using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Conveyor.Utility
{
    /// <summary>
    /// Visitor used to visit Members and resolve their expressions to get the constant value.
    /// </summary>
    public class MemberAccessResolver : ExpressionVisitor
    {
        /// <summary>
        /// A <see cref="Stack{T}"/> instance to store expressions found in the member expression on the stack
        /// </summary>
        private Stack<Expression> Nodes = new Stack<Expression>();
        
        /// <summary>
        /// Constant value from the member expression
        /// </summary>
        private object ConstantValue { get; set; }

        /// <summary>
        /// Reads the constant value from the given expression.
        /// </summary>
        /// <param name="expression">Given expression to be visited and to extract the value from</param>
        /// <returns>The value from the given expression.</returns>
        public static object GetConstantValue(Expression expression)
        {
            MemberAccessResolver visitor = new MemberAccessResolver();
            visitor.Visit(expression);
            return visitor.ConstantValue;
        }


        /// <summary>
        /// A private constructor to make sure that no instance will directly be created.
        /// </summary>
        private MemberAccessResolver() { }

        /// <summary>
        /// Pushes the member expression into the <see cref="MemberAccessResolver.Nodes"/> collection.
        /// </summary>
        /// <param name="node">The expression to be added to <see cref="MemberAccessResolver.Nodes"/></param>
        /// <returns>The same as the base method. In other words, The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            Nodes.Push(node);
            return base.VisitMember(node);
        }

        /// <summary>
        /// Reads all values from the stack, if there is any property or field, its value will be used, otherwise the constant value that has been passed as a parameter.
        /// </summary>
        /// <param name="node">Constant Expression to extract the value from</param>
        /// <returns>The same input parameter</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            MemberExpression prevNode;
            object val = node.Value;
            while (Nodes.Count != 0 && (prevNode = Nodes.Pop() as MemberExpression) != null)
            {
                FieldInfo fieldInfo = prevNode.Member as FieldInfo;
                PropertyInfo propertyInfo = prevNode.Member as PropertyInfo;

                if (fieldInfo != null)
                    val = fieldInfo.GetValue(val);
                if (propertyInfo != null)
                    val = propertyInfo.GetValue(val);
            }

            ConstantValue = val;

            return node;
        }
    }
}
