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
    /// A temporary solution for changing the type of the passed parameter into the expression. <para />
    /// It does what it suppose to do, but keep in mind that it won't support complex hierarchy expression. <para/>
    /// (x => x.City.Something == somevalue) will fail!
    /// </summary>
    /// <typeparam name="T">The new parameter type</typeparam>
    public class ParameterChanger<T> : ExpressionVisitor where T : class
    {
        /// <summary>
        /// Stores the new parameter type
        /// </summary>
        private readonly ParameterExpression _parameter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameter">The passed parameter into the lambda expression</param>
        public ParameterChanger(ParameterExpression parameter) => _parameter = parameter;

        /// <summary>
        /// Returns the parameter type that will replace the passed parameter
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression node) => _parameter;

        /// <summary>
        /// Returns an expression that represents accessed property from the (T) Entity instead of the (TDTO) ViewModel
        /// </summary>
        /// <param name="memberExpression">Accessed member</param>
        /// <returns>An instance of <see cref="MemberExpression"/> that represents accessed property from the Entity</returns>
        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            // for now we are going to support only properties.
            // if we need for any reason to support fields later, just make sure to handle it here instead of casting an exception
            if (memberExpression.Member.MemberType != MemberTypes.Property)
                throw new NotSupportedException($"{nameof(memberExpression.Member.MemberType)} is not support.");

            // get property name
            string memberName = memberExpression.Member.Name;
            //find property on type T (Model) that represents our Entity by name
            PropertyInfo otherMember = typeof(T).GetProperty(memberName);

            // make sure that otherMemeber isn't null.
            // this will be null in case of deep access such as (x => x.Prop.Prop)
            if (otherMember == null)
                throw new NotSupportedException($"Deep property access isn't supported yet.");

            //visit left side of this expression
            Expression inner = Visit(memberExpression.Expression);
            // return accessed property
            return Expression.Property(inner, otherMember);
        }
    }
}
