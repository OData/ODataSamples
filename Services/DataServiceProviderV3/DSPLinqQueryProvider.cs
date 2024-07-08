// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>Implementation of <see cref="IQueryProvider"/> which allows running the DSP queries against a LINQ to Objects.</summary>
    internal class DSPLinqQueryProvider : IQueryProvider
    {
        /// <summary>The underlying query provider (the LINQ to Objects provider) determined from the source query.</summary>
        private IQueryProvider underlyingQueryProvider;

        /// <summary>Private constructor.</summary>
        /// <param name="underlyingQueryProvider">The underlying provider to run the translated query on.</param>
        private DSPLinqQueryProvider(IQueryProvider underlyingQueryProvider)
        {
            this.underlyingQueryProvider = underlyingQueryProvider;
        }

        /// <summary>Wraps a query in a new query which will translate the DSP query into a LINQ to Objects runnable query
        /// and run it on the provided <paramref name="underlyingQuery"/>.</summary>
        /// <param name="underlyingQuery">The underlying (LINQ to Objects) query to wrap.</param>
        /// <returns>A new query which can handle the DSP expressions and run them on top of the <pararef name="underlyingQuery"/>.</returns>
        public static IQueryable CreateQuery(IQueryable underlyingQuery)
        {
            DSPLinqQueryProvider provider = new DSPLinqQueryProvider(underlyingQuery.Provider);
            return provider.CreateQuery(underlyingQuery.Expression);
        }

        /// <summary>Executes the specified DSP expression on the underlying LINQ to Objects provider.</summary>
        /// <typeparam name="TElement">The type of the result.</typeparam>
        /// <param name="expression">The expression (the DSP version) to run.</param>
        /// <returns>Enumerator with the results of the query.</returns>
        internal IEnumerator<TElement> ExecuteQuery<TElement>(Expression expression)
        {
            expression = this.ProcessExpression(expression);
            return this.underlyingQueryProvider.CreateQuery<TElement>(expression).GetEnumerator();
        }

        #region IQueryProvider Members

        /// <summary>Creates a query for the specified <paramref name="expression"/>.</summary>
        /// <typeparam name="TElement">The type of the result of the query.</typeparam>
        /// <param name="expression">The expression to create a query for.</param>
        /// <returns>The new query using the specified <paramref name="expression"/>.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DSPLinqQuery<TElement>(this, expression);
        }

        /// <summary>Creates a query for the specified <paramref name="expression"/>.</summary>
        /// <param name="expression">The expression to create a query for.</param>
        /// <returns>The new query using the specified <paramref name="expression"/>.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            Type et = TypeSystem.GetIEnumerableElementType(expression.Type);
            Type qt = typeof(DSPLinqQuery<>).MakeGenericType(et);
            object[] args = new object[] { this, expression };

            ConstructorInfo ci = qt.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { typeof(DSPLinqQueryProvider), typeof(Expression) },
                null);

            return (IQueryable)ci.Invoke(args);
        }

        /// <summary>Executes an expression which returns a single result.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The result of the expression.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            expression = this.ProcessExpression(expression);
            return this.underlyingQueryProvider.Execute<TResult>(expression);
        }

        /// <summary>Executes an expression which returns a single result.</summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The result of the expression.</returns>
        public object Execute(Expression expression)
        {
            expression = this.ProcessExpression(expression);
            return this.underlyingQueryProvider.Execute(expression);
        }

        #endregion

        /// <summary>Method which converts expressions from the DSP "syntax" into the LINQ to Objects "syntax".</summary>
        /// <param name="expression">The expression to process.</param>
        /// <returns>A new expression which is the result of the conversion.</returns>
        private Expression ProcessExpression(Expression expression)
        {
            return DSPMethodTranslatingVisitor.TranslateExpression(expression);
        }
    }
}
