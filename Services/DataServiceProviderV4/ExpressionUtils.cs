// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV4
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>Utility methods to work with the Expression type.</summary>
    internal static class ExpressionUtils
    {
        /// <summary>
        /// MethodInfo for Queryable.Where
        /// </summary>
        private static MethodInfo queryableWhereMethodInfo;

        /// <summary>
        /// MethodInfo for Enumerable.Where
        /// </summary>
        private static MethodInfo enumerableWhereMethodInfo;

        /// <summary>
        /// MethodInfo for Queryable.Where
        /// </summary>
        private static MethodInfo QueryableWhereMethodInfo
        {
            get { return ExpressionUtils.queryableWhereMethodInfo ?? (ExpressionUtils.queryableWhereMethodInfo = ExpressionUtils.GetMethodInfoFromLambdaBody(() => (new object[0]).AsQueryable().Where(o => true))); }
        }

        /// <summary>
        /// MethodInfo for Enumerable.Where
        /// </summary>
        private static MethodInfo EnumerableWhereMethodInfo
        {
            get { return ExpressionUtils.enumerableWhereMethodInfo ?? (ExpressionUtils.enumerableWhereMethodInfo = ExpressionUtils.GetMethodInfoFromLambdaBody(() => (new object[0]).AsEnumerable().Where(o => true))); }
        }

        /// <summary>
        /// Returns the element type of the expression.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <returns>Returns the element type of the expression.</returns>
        internal static Type ElementType(this Expression source)
        {
            Debug.Assert(source != null, "source != null");
            return TypeSystem.GetIEnumerableElementType(source.Type) ?? source.Type;
        }

        /// <summary>
        /// Applies Queryable.Where() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="predicate">Predicate to pass to Queryable.Where().</param>
        /// <returns>New expression with Queryable.Where() applied.</returns>
        internal static Expression QueryableWhere(this Expression source, LambdaExpression predicate)
        {
            return ExpressionUtils.Where(ExpressionUtils.QueryableWhereMethodInfo, source, predicate);
        }

        /// <summary>
        /// Applies Enumerable.Where() to <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Source expression.</param>
        /// <param name="predicate">Predicate to pass to Enumerable.Where().</param>
        /// <returns>New expression with Enumerable.Where() applied.</returns>
        internal static Expression EnumerableWhere(this Expression source, LambdaExpression predicate)
        {
            return ExpressionUtils.Where(ExpressionUtils.EnumerableWhereMethodInfo, source, predicate);
        }

        /// <summary>
        /// Helper method to get the MethodInfo from the body of the given lambda expression.
        /// </summary>
        /// <typeparam name="TResult">Result type of <paramref name="lambda"/>.</typeparam>
        /// <param name="lambda">Lambda expression.</param>
        /// <returns>Returns the MethodInfo from the body of the given lambda expression.</returns>
        internal static MethodInfo GetMethodInfoFromLambdaBody<TResult>(Expression<Func<TResult>> lambda)
        {
            Debug.Assert(lambda != null, "lambda != null");
            return ((MethodCallExpression)lambda.Body).Method.GetGenericMethodDefinition();
        }

        /// <summary>
        /// Compose Where() to expression
        /// </summary>
        /// <param name="genericMethodInfo">Where MethodInfo</param>
        /// <param name="source">Source expression</param>
        /// <param name="predicate">Predicate expression</param>
        /// <returns>Expression with Where()</returns>
        private static Expression Where(MethodInfo genericMethodInfo, Expression source, LambdaExpression predicate)
        {
            Debug.Assert(genericMethodInfo != null && genericMethodInfo.IsGenericMethod, "genericMethodInfo != null && genericMethodInfo.IsGenericMethod");
            Debug.Assert(source != null, "source != null");
            Debug.Assert(predicate != null, "predicate != null");

            Type elementType = source.ElementType();
            Debug.Assert(elementType != null, "elementType != null");

            MethodInfo whereMethod = genericMethodInfo.MakeGenericMethod(elementType);

            // TODO: If the type of predicate is not sourceType, we need to replace the parameter with
            // a downcasted parameter type if predicate's input is a base class of sourceType.

            // Note the ParameterType on an IQueryable mehtod is Expression<Func<>> where as the ParameterType
            // on an IEnumerable method is Func<>.
            Debug.Assert(
                whereMethod.GetParameters()[1].ParameterType == predicate.GetType() ||
                whereMethod.GetParameters()[1].ParameterType == predicate.Type,
                "predicate should be of type Expression<Func<TSource, bool>>");
            return Expression.Call(null, whereMethod, source, predicate);
        }
    }
}
