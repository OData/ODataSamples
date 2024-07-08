// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataServiceProviderV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;

    /// <summary>Expression visitor which translates calls to methods on the <see cref="DataServiceProviderMethods"/> class
    /// into expressions which can be evaluated by LINQ to Objects.</summary>
    internal class DSPMethodTranslatingVisitor : ExpressionVisitor
    {
        /// <summary>MethodInfo for object DataServiceProviderMethods.GetValue(this object value, ResourceProperty property).</summary>
        internal static readonly MethodInfo GetValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "GetValue",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(ResourceProperty) },
            null);

        /// <summary>MethodInfo for IEnumerable&lt;T&gt; DataServiceProviderMethods.GetSequenceValue(this object value, ResourceProperty property).</summary>
        internal static readonly MethodInfo GetSequenceValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "GetSequenceValue",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(ResourceProperty) },
            null);

        /// <summary>MethodInfo for Convert.</summary>
        internal static readonly MethodInfo ConvertMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "Convert",
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for TypeIs.</summary>
        internal static readonly MethodInfo TypeIsMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "TypeIs",
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for OfType(IQueryable&lt;&gt;, ResourceType)</summary>
        internal static readonly MethodInfo OfTypeQueryableMethodInfo = 
            ExpressionUtils.GetMethodInfoFromLambdaBody(() => DataServiceProviderMethods.OfType<object, object>((IQueryable<object>)null, (ResourceType)null)).GetGenericMethodDefinition();

        /// <summary>MethodInfo for OfType(IEnumerable&lt;&gt;, ResourceType)</summary>
        internal static readonly MethodInfo OfTypeEnumerableMethodInfo = 
            ExpressionUtils.GetMethodInfoFromLambdaBody(() => DataServiceProviderMethods.OfType<object, object>((IEnumerable<object>)null, (ResourceType)null)).GetGenericMethodDefinition();

        /// <summary>MethodInfo for object OpenTypeMethods.GetValue(this object value, string propertyName).</summary>
        internal static readonly MethodInfo OpenTypesGetValueMethodInfo = typeof(OpenTypeMethods).GetMethod("GetValue", BindingFlags.Static | BindingFlags.Public, null, new Type[] {typeof(object), typeof(string)}, null);

        /// <summary>Method which translates an expression using the <see cref="DataServiceProviderMethods"/> methods
        /// into a new expression which can be evaluated by LINQ to Objects.</summary>
        /// <param name="expression">The expression to translate.</param>
        /// <returns>The translated expression.</returns>
        public static Expression TranslateExpression(Expression expression)
        {
            DSPMethodTranslatingVisitor visitor = new DSPMethodTranslatingVisitor();
            return visitor.Visit(expression);
        }

        /// <summary>
        /// MethodCallExpression visit method
        /// </summary>
        /// <param name="m">The MethodCallExpression expression to visit</param>
        /// <returns>The visited MethodCallExpression expression </returns>
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method == GetValueMethodInfo)
            {
                // Arguments[0] - the resource to get property value of - we assume it's a DSPEntity
                // Arguments[1] - the ResourceProperty to get value of

                // Just call the targetResource.GetValue(resourceProperty.Name)
                return Expression.Call(
                    this.Visit(m.Arguments[0]),
                    typeof(DSPResource).GetMethod("GetValue"),
                    Expression.Property(m.Arguments[1], "Name"));
            }
            else if (m.Method == OpenTypesGetValueMethodInfo)
            {
                // Arguments[0] - the resource to get property value of - we assume it's a DSPEntity
                // Arguments[1] - the property name to get value of

                // Just call the targetResource.GetValue(propertyName)
                return Expression.Call(this.Visit(m.Arguments[0]), typeof(DSPResource).GetMethod("GetValue"), m.Arguments[1]);
            }
            else if (m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == GetSequenceValueMethodInfo)
            {
                // Arguments[0] - the resource to get property value of - we assume it's a DSPEntity
                // Arguments[1] - the ResourceProperty to get value of

                // Just call the targetResource.GetValue(resourceProperty.Name) and cast it to the right IEnumerable<T> (which is the return type of the GetSequenceMethod
                return Expression.Convert(
                    Expression.Call(
                        this.Visit(m.Arguments[0]),
                        typeof(DSPResource).GetMethod("GetValue"),
                        Expression.Property(m.Arguments[1], "Name")),
                    m.Method.ReturnType);
            }
            else if (m.Method == ConvertMethodInfo)
            {
                // All our resources are of the same underlying CLR type, so no need for conversion of the CLR type
                //   and we don't have any specific action to take to convert the Resource Types either (as we access properties from a property bag)
                // So get rid of the conversions as we don't need them
                return this.Visit(m.Arguments[0]);
            }
            else if (m.Method == TypeIsMethodInfo)
            {
                // Arguments[0] - the resource to determine the type of - we assume it's a DSPEntity
                // Arguments[1] - the ResourceType to test for

                // We don't support type inheritance yet, so simple comparison is enough
                return Expression.Equal(Expression.Property(this.Visit(m.Arguments[0]), typeof(DSPResource).GetProperty("ResourceType")), m.Arguments[1]);
            }
            else if (m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == OfTypeQueryableMethodInfo)
            {
                // Arguments[0] - the source queryable to filter
                // Arguments[1] - the ResourceType to filter by

                Expression sourceQueryable = this.Visit(m.Arguments[0]);
                Expression resourceTypeToCompareTo = this.Visit(m.Arguments[1]);
                if (sourceQueryable.ElementType() != typeof(DSPResource))
                {
                    throw new NotImplementedException("Currently we can only handle resources backed by DSPResource instances.");
                }

                // We don't support type inheritance yet, so just compare the type names.
                // sourceQueryable.Where(resource => resource.ResourceType == resourceTypeToCompareTo)
                ParameterExpression parameter = Expression.Parameter(typeof(DSPResource), "resource");
                return sourceQueryable.QueryableWhere(
                    Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, typeof(DSPResource).GetProperty("ResourceType")),
                            resourceTypeToCompareTo),
                        parameter));
            }
            else if (m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == OfTypeEnumerableMethodInfo)
            {
                // Arguments[0] - the source enumerable to filter
                // Arguments[1] - the ResourceType to filter by

                Expression sourceEnumerable = this.Visit(m.Arguments[0]);
                Expression resourceTypeToCompareTo = this.Visit(m.Arguments[1]);
                if (sourceEnumerable.ElementType() != typeof(DSPResource))
                {
                    throw new NotImplementedException("Currently we can only handle resources backed by DSPResource instances.");
                }

                // We don't support type inheritance yet, so just compare the type names.
                // sourceEnumerable.Where(resource => resource.ResourceType == resourceTypeToCompareTo)
                ParameterExpression parameter = Expression.Parameter(typeof(DSPResource), "resource");
                return sourceEnumerable.EnumerableWhere(
                    Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, typeof(DSPResource).GetProperty("ResourceType")),
                            resourceTypeToCompareTo),
                        parameter));
            }

            return base.VisitMethodCall(m);
        }

        internal override Expression VisitBinary(BinaryExpression b)
        {
            if (b.Method != null && b.Method.DeclaringType == typeof(OpenTypeMethods))
            {
                var replacement = typeof(OpenTypesMethodReplacement).GetMethod(b.Method.Name, BindingFlags.Public | BindingFlags.Static);
                return Expression.MakeBinary(b.NodeType, this.Visit(b.Left), this.Visit(b.Right), b.IsLiftedToNull, replacement, (LambdaExpression)this.VisitLambda(b.Conversion));
            }

            return base.VisitBinary(b);
        }

        internal override Expression VisitUnary(UnaryExpression u)
        {
            if (u.Method != null && u.Method.DeclaringType == typeof(OpenTypeMethods))
            {
                var replacement = typeof(OpenTypesMethodReplacement).GetMethod(u.Method.Name, BindingFlags.Public | BindingFlags.Static);
                return Expression.MakeUnary(u.NodeType, this.Visit(u.Operand), u.Type, replacement);
            }

            return base.VisitUnary(u);
        }
    }
}
