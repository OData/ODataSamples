﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

#if !INTERNAL_DROP
namespace Microsoft.OData.Core.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Validation;
#if ASTORIA_SERVER
    using Microsoft.OData.Service;
    using ErrorStrings = Microsoft.OData.Service.Strings;
#endif
#if ASTORIA_CLIENT
    using ErrorStrings = Microsoft.OData.Client.Strings;
#endif
#if !ASTORIA_SERVER && !ASTORIA_CLIENT
    using Microsoft.OData.Core.JsonLight;
    using ErrorStrings = Microsoft.OData.Core.Strings;
    using PlatformHelper = Microsoft.OData.Core.PlatformHelper;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class with code that will eventually live in EdmLib.
    /// </summary>
    /// <remarks>This class should go away completely when the EdmLib integration is fully done.</remarks>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The class coupling is due to mapping primitive types, lot of different types there.")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Edm", Justification = "Following EdmLib standards.")]
    internal static class EdmLibraryExtensions
    {
        /// <summary>
        /// Map of CLR primitive type to EDM primitive type reference. Doesn't include spatial types since they need assignability and not equality.
        /// </summary>
        private static readonly Dictionary<Type, IEdmPrimitiveTypeReference> PrimitiveTypeReferenceMap = new Dictionary<Type, IEdmPrimitiveTypeReference>(EqualityComparer<Type>.Default);

        /// <summary>Type reference for Edm.Boolean.</summary>
        private static readonly EdmPrimitiveTypeReference BooleanTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), false);

        /// <summary>Type reference for Edm.Byte.</summary>
        private static readonly EdmPrimitiveTypeReference ByteTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), false);

        /// <summary>Type reference for Edm.Decimal.</summary>
        private static readonly EdmPrimitiveTypeReference DecimalTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);

        /// <summary>Type reference for Edm.Double.</summary>
        private static readonly EdmPrimitiveTypeReference DoubleTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), false);

        /// <summary>Type reference for Edm.Int16.</summary>
        private static readonly EdmPrimitiveTypeReference Int16TypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), false);

        /// <summary>Type reference for Edm.Int32.</summary>
        private static readonly EdmPrimitiveTypeReference Int32TypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false);

        /// <summary>Type reference for Edm.Int64.</summary>
        private static readonly EdmPrimitiveTypeReference Int64TypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), false);

        /// <summary>Type reference for Edm.SByte.</summary>
        private static readonly EdmPrimitiveTypeReference SByteTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), false);

        /// <summary>Type reference for Edm.String.</summary>
        private static readonly EdmPrimitiveTypeReference StringTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);

        /// <summary>Type reference for Edm.Float.</summary>
        private static readonly EdmPrimitiveTypeReference SingleTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), false);

        #region Edm Collection constants

        /// <summary>The qualifier to turn a type name into a Collection type name.</summary>
        private const string CollectionTypeQualifier = "Collection";

        /// <summary>Format string to describe a Collection of a given type.</summary>
        private const string CollectionTypeFormat = CollectionTypeQualifier + "({0})";

        #endregion Edm Collection constants

        /// <summary>
        /// Constructor.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Need to use the static constructor for the phone platform.")]
        static EdmLibraryExtensions()
        {
            PrimitiveTypeReferenceMap.Add(typeof(Boolean), BooleanTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Byte), ByteTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Decimal), DecimalTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Double), DoubleTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Int16), Int16TypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Int32), Int32TypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Int64), Int64TypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(SByte), SByteTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(String), StringTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Single), SingleTypeReference);

#if ASTORIA_SERVER
            PrimitiveTypeReferenceMap.Add(typeof(DateTime), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false));
#endif
            PrimitiveTypeReferenceMap.Add(typeof(DateTimeOffset), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false));
            PrimitiveTypeReferenceMap.Add(typeof(Guid), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), false));
            PrimitiveTypeReferenceMap.Add(typeof(TimeSpan), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), false));
            PrimitiveTypeReferenceMap.Add(typeof(byte[]), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true));
            PrimitiveTypeReferenceMap.Add(typeof(Stream), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), false));

            PrimitiveTypeReferenceMap.Add(typeof(Boolean?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), true));
            PrimitiveTypeReferenceMap.Add(typeof(Byte?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), true));
#if ASTORIA_SERVER
            PrimitiveTypeReferenceMap.Add(typeof(DateTime?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true));
#endif
            PrimitiveTypeReferenceMap.Add(typeof(DateTimeOffset?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true));

            PrimitiveTypeReferenceMap.Add(typeof(Decimal?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true));
            PrimitiveTypeReferenceMap.Add(typeof(Double?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), true));
            PrimitiveTypeReferenceMap.Add(typeof(Int16?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), true));
            PrimitiveTypeReferenceMap.Add(typeof(Int32?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true));
            PrimitiveTypeReferenceMap.Add(typeof(Int64?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), true));
            PrimitiveTypeReferenceMap.Add(typeof(SByte?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), true));
            PrimitiveTypeReferenceMap.Add(typeof(Single?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), true));
            PrimitiveTypeReferenceMap.Add(typeof(Guid?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), true));
            PrimitiveTypeReferenceMap.Add(typeof(TimeSpan?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), true));
        }

        #region Internal methods
        #region ODataLib only
#if !ODATALIB_QUERY && !ASTORIA_SERVER && !ASTORIA_CLIENT

        /// <summary>
        /// Returns the fully qualified name of a navigation source.
        /// </summary>
        /// <param name="navigationSource">The navigation source to get the full name for.</param>
        /// <returns>The full qualified name of the navigation source.</returns>
        internal static string FullNavigationSourceName(this IEdmNavigationSource navigationSource)
        {
            Debug.Assert(navigationSource != null, "navigationSource != null");

            return string.Join(".", navigationSource.Path.Path.ToArray());
        }

        /// <summary>
        /// Filters functions by the parameter names.
        /// </summary>
        /// <param name="functionImports">The operation imports.</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <returns>Return the operation imports that match the parameter names.</returns>
        internal static IEnumerable<IEdmFunctionImport> FilterFunctionsByParameterNames(this IEnumerable<IEdmFunctionImport> functionImports, IEnumerable<string> parameterNames)
        {
            Debug.Assert(functionImports != null, "functionImports");
            Debug.Assert(parameterNames != null, "parameterNames");

            IList<string> parameterNameList = parameterNames.ToList();

            // TODO: update code that is duplicate between operation and operation import, add more tests.
            foreach (IEdmFunctionImport functionImport in functionImports)
            {
                IEnumerable<IEdmOperationParameter> parametersToMatch = functionImport.Operation.Parameters;

                // bindable functions don't require the first parameter be specified, since its already implied in the path.
                if (functionImport.Function.IsBound)
                {
                    parametersToMatch = parametersToMatch.Skip(1);
                }

                // if any parameter count is different, don't consider it a match.
                List<IEdmOperationParameter> operationImportParameters = parametersToMatch.ToList();
                if (operationImportParameters.Count != parameterNameList.Count)
                {
                    continue;
                }

                // if any parameter was missing, don't consider it a match.
                if (operationImportParameters.Any(p => parameterNameList.All(k => k != p.Name)))
                {
                    continue;
                }

                yield return functionImport;
            }
        }

        /// <summary>
        /// Filters the type of the bound operations in by the bindingtype inheritance hierarchy to type closest to bindingtype. 
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>The closest Bound Operations to the specified binding type.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
            Justification = "Parameter type is needed to get binding type.")]
        internal static IEnumerable<IEdmOperation> FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(this IEnumerable<IEdmOperation> operations, IEdmType bindingType)
        {
            Debug.Assert(operations != null, "operations");
            Debug.Assert(bindingType != null, "bindingType");

            IEdmStructuredType nonCollectionBindingType = bindingType as IEdmStructuredType;
            if (bindingType.TypeKind == EdmTypeKind.Collection)
            {
                nonCollectionBindingType = ((IEdmCollectionType)bindingType).ElementType.Definition as IEdmStructuredType;
            }

            // If its a type that can't have inheritance then no filtering is necessary
            if (nonCollectionBindingType == null)
            {
                return operations;
            }

            Dictionary<IEdmType, List<IEdmOperation>> sortedOperations = new Dictionary<IEdmType, List<IEdmOperation>>(new EdmTypeEqualityComparer());
            IEdmType currentClosestType = null;
            int currentInheritanceLevelsFromBase = int.MaxValue;
            foreach (IEdmOperation operation in operations)
            {
                if (!operation.IsBound || !operation.Parameters.Any())
                {
                    continue;
                }

                IEdmType operationBindingType = operation.Parameters.First().Type.Definition;
                IEdmStructuredType operationBindingStructuralType = operationBindingType as IEdmStructuredType;

                if (operationBindingType.TypeKind == EdmTypeKind.Collection)
                {
                    IEdmCollectionType operationBindingCollectionType = operationBindingType as IEdmCollectionType;
                    operationBindingStructuralType = operationBindingCollectionType.ElementType.Definition as IEdmStructuredType;
                }
                
                if (operationBindingStructuralType == null || !nonCollectionBindingType.IsOrInheritsFrom(operationBindingStructuralType))
                {
                    continue;
                }

                int inheritanceLevelsFromBase = nonCollectionBindingType.InheritanceLevelFromSpecifiedInheritedType(operationBindingStructuralType);

                if (currentInheritanceLevelsFromBase > inheritanceLevelsFromBase)
                {
                    currentInheritanceLevelsFromBase = inheritanceLevelsFromBase;
                    currentClosestType = operationBindingType;
                }

                if (!sortedOperations.ContainsKey(operationBindingType))
                {
                    sortedOperations[operationBindingType] = new List<IEdmOperation>();
                }

                sortedOperations[operationBindingType].Add(operation);
            }

            if (currentClosestType != null)
            {
                return sortedOperations[currentClosestType];
            }

            return Enumerable.Empty<IEdmOperation>();
        }

        /// <summary>
        /// Given a list of possible operations and a list of parameter names, filter operations that exactly matches
        /// the parameter names. If more than one function matches, throw.
        /// </summary>
        /// <remarks>
        /// Binding parameters will be ignored in this method. Only non-binding parameters are matched.
        /// </remarks>
        /// <param name="functions">The list of functions to search.</param>
        /// <param name="parameters">The list of non-binding parameter names to match.</param>
        /// <returns>All operation imports matches the parameter names exactly or an empty enumerable.</returns>
        internal static IEnumerable<IEdmFunction> FilterFunctionsByParameterNames(this IEnumerable<IEdmFunction> functions, IEnumerable<string> parameters)
        {
            return functions.Cast<IEdmOperation>().FilterOperationsByParameterNames(parameters).Cast<IEdmFunction>();
        }

        /// <summary>
        /// Filters the operations by parameter names.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Operations filtered by parameter.</returns>
        internal static IEnumerable<IEdmOperation> FilterOperationsByParameterNames(this IEnumerable<IEdmOperation> operations, IEnumerable<string> parameters)
        {
            Debug.Assert(operations != null, "operations");
            Debug.Assert(parameters != null, "parameters");

            IList<string> parametersList = parameters.ToList();

            // TODO: update code that is duplicate between operation and operation import, add more tests.
            foreach (IEdmOperation operation in operations)
            {
                IEnumerable<IEdmOperationParameter> parametersToMatch = operation.Parameters;

                // bindable functions don't require the first parameter be specified, since its already implied in the path.
                if (operation.IsBound)
                {
                    parametersToMatch = parametersToMatch.Skip(1);
                }

                // if any parameter count is different, don't consider it a match.
                List<IEdmOperationParameter> functionImportParameters = parametersToMatch.ToList();
                if (functionImportParameters.Count != parametersList.Count)
                {
                    continue;
                }

                // if any parameter was missing, don't consider it a match.
                if (functionImportParameters.Any(p => parametersList.All(k => k != p.Name)))
                {
                    continue;
                }

                yield return operation;
            }
        }

        /// <summary>
        /// Ensures that operations are bound and have a binding parameter, other wise throws an exception.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <returns>Bound Operations with binding parameters.</returns>
        internal static IEnumerable<IEdmOperation> EnsureOperationsBoundWithBindingParameter(this IEnumerable<IEdmOperation> operations)
        {
            foreach (IEdmOperation operation in operations)
            {
                if (!operation.IsBound)
                {
                    throw new ODataException(ErrorStrings.EdmLibraryExtensions_UnBoundOperationsFoundFromIEdmModelFindMethodIsInvalid(operation.Name));
                }

                if (!operation.Parameters.Any())
                {
                    throw new ODataException(ErrorStrings.EdmLibraryExtensions_NoParameterBoundOperationsFoundFromIEdmModelFindMethodIsInvalid(operation.Name));
                }

                yield return operation;
            }
        }

        /// <summary>
        /// Finds the operation group with the specified namespace and name. If the name contains the function parameters, this
        /// method will return the operation with matching parameters.
        /// </summary>
        /// <param name="model">The model to find the operation in.</param>
        /// <param name="namespaceQualifiedOperationName">The namespace qualified name of the operation.</param>
        /// <returns>The <see cref="IEdmOperation"/> Operation group with the specified name or null if no such operation exists.</returns>
        internal static IEnumerable<IEdmOperation> ResolveOperations(this IEdmModel model, string namespaceQualifiedOperationName)
        {
            return model.ResolveOperations(namespaceQualifiedOperationName, true /*allowParameterNames*/);
        }

        /// <summary>
        /// Resolves an operation or operation group.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="operationName">The operation name to resolve. The name may be namespace qualified and it may contain parameter type names, e.g. Function1(P1Type,P2Type)</param>
        /// <param name="allowParameterTypeNames">Whether parameter type names are allowed to appear in the operation name to resolve.</param>
        /// <returns>The resolved operation or operation group.</returns>
        internal static IEnumerable<IEdmOperation> ResolveOperations(this IEdmModel model, string operationName, bool allowParameterTypeNames)
        {
            // TODO: Resolve duplication of operationImport and operation
            if (string.IsNullOrEmpty(operationName))
            {
                return Enumerable.Empty<IEdmOperation>();
            }

            int indexOfParameterStart = operationName.IndexOf(JsonLightConstants.FunctionParameterStart);
            string operationNameWithoutParameterTypes;
            if (indexOfParameterStart > 0)
            {
                if (!allowParameterTypeNames)
                {
                    return Enumerable.Empty<IEdmOperation>();
                }

                operationNameWithoutParameterTypes = operationName.Substring(0, indexOfParameterStart);
            }
            else
            {
                operationNameWithoutParameterTypes = operationName;
            }

            IEnumerable<IEdmOperation> operations = model.FindDeclaredOperations(operationNameWithoutParameterTypes);

            if (operations == null)
            {
                return Enumerable.Empty<IEdmOperation>();
            }

            if (indexOfParameterStart > 0)
            {
                return operations.Where(f => f.FullNameWithParameters().Equals(operationName, StringComparison.Ordinal));
            }

            return ValidateOperationGroupReturnsOnlyOnKind(operations, operationNameWithoutParameterTypes);
        }

        /// <summary>
        /// Checks whether all operation imports have the same return type 
        /// </summary>
        /// <param name="operationImports">the list to check</param>
        /// <returns>true if the list of operation imports all have the same return type</returns>
        internal static bool AllHaveEqualReturnTypeAndAttributes(this IList<IEdmOperationImport> operationImports)
        {
            // TODO: Resolve duplication of operationImport and operation
            Debug.Assert(operationImports != null, "operationImports != null");

            if (!operationImports.Any())
            {
                return true;
            }

            IEdmType firstReturnType = operationImports[0].Operation.ReturnType == null ? null : operationImports[0].Operation.ReturnType.Definition;
            bool firstIsFunction = operationImports[0].IsFunctionImport();
            bool firstIsAction = operationImports[0].IsActionImport();
            foreach (IEdmOperationImport f in operationImports)
            {
                if (f.IsFunctionImport() != firstIsFunction)
                {
                    return false;
                }

                if (f.IsActionImport() != firstIsAction)
                {
                    return false;
                }

                if (firstReturnType != null)
                {
                    if (f.Operation.ReturnType.Definition.FullTypeName() != firstReturnType.FullTypeName())
                    {
                        return false;
                    }
                }
                else
                {
                    if (f.Operation.ReturnType != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether all operations have the same return type 
        /// </summary>
        /// <param name="operations">the list to check</param>
        /// <returns>true if the list of operation imports all have the same return type</returns>
        internal static bool AllHaveEqualReturnTypeAndAttributes(this IList<IEdmOperation> operations)
        {
            // TODO: Resolve duplication of operationImport and operation
            Debug.Assert(operations != null, "operations != null");

            if (!operations.Any())
            {
                return true;
            }

            IEdmType firstReturnType = operations[0].ReturnType == null ? null : operations[0].ReturnType.Definition;
            bool firstIsFunction = operations[0].IsFunction();
            bool firstIsAction = operations[0].IsAction();
            foreach (IEdmOperation f in operations)
            {
                if (f.IsFunction() != firstIsFunction)
                {
                    return false;
                }

                if (f.IsAction() != firstIsAction)
                {
                    return false;
                }

                if (firstReturnType != null)
                {
                    if (f.ReturnType.Definition.FullTypeName() != firstReturnType.FullTypeName())
                    {
                        return false;
                    }
                }
                else
                {
                    if (f.ReturnType != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Name of the operation with parameters.
        /// </summary>
        /// <param name="operation">Operation in question.</param>
        /// <returns>Name of the operation import with parameters.</returns>
        internal static string NameWithParameters(this IEdmOperation operation)
        {
            Debug.Assert(operation != null, "operation != null");

            return operation.Name + operation.ParameterTypesToString();
        }

        /// <summary>
        /// Full name of the operation with parameters.
        /// </summary>
        /// <param name="operation">Operation in question.</param>
        /// <returns>Full name of the operation with parameters.</returns>
        internal static string FullNameWithParameters(this IEdmOperation operation)
        {
            Debug.Assert(operation != null, "operation != null");

            return operation.FullName() + operation.ParameterTypesToString();
        }

        /// <summary>
        /// Name of the operation import with parameters.
        /// </summary>
        /// <param name="operationImport">Operation import in question.</param>
        /// <returns>Name of the operation import with parameters.</returns>
        internal static string NameWithParameters(this IEdmOperationImport operationImport)
        {
            Debug.Assert(operationImport != null, "operationImport != null");

            return operationImport.Name + operationImport.ParameterTypesToString();
        }

        /// <summary>
        /// Full name of the operation import with parameters.
        /// </summary>
        /// <param name="operationImport">Operation import in question.</param>
        /// <returns>Full name of the operation import with parameters.</returns>
        internal static string FullNameWithParameters(this IEdmOperationImport operationImport)
        {
            Debug.Assert(operationImport != null, "operationImport != null");

            return operationImport.FullName() + operationImport.ParameterTypesToString();
        }

        /// <summary>
        /// Removes the actions.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="actionItems">The action items.</param>
        /// <returns>Only the functions from the operation sequence.</returns>
        internal static IEnumerable<IEdmFunction> RemoveActions(this IEnumerable<IEdmOperation> source, out IList<IEdmAction> actionItems)
        {
            List<IEdmFunction> functions = new List<IEdmFunction>();

            actionItems = new List<IEdmAction>();
            foreach (var item in source)
            {
                if (item.IsAction())
                {
                    actionItems.Add((IEdmAction)item);
                }
                else
                {
                    functions.Add((IEdmFunction)item);
                }
            }

            return functions;
        }

        /// <summary>
        /// Removes the action imports.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="actionImportItems">The action import items.</param>
        /// <returns>Only the function imports from the operation Import sequence.</returns>
        internal static IEnumerable<IEdmFunctionImport> RemoveActionImports(this IEnumerable<IEdmOperationImport> source, out IList<IEdmActionImport> actionImportItems)
        {
            List<IEdmFunctionImport> functions = new List<IEdmFunctionImport>();

            actionImportItems = new List<IEdmActionImport>();
            foreach (var item in source)
            {
                if (item.IsActionImport())
                {
                    actionImportItems.Add((IEdmActionImport)item);
                }
                else
                {
                    functions.Add((IEdmFunctionImport)item);
                }
            }

            return functions;
        }

        /// <summary>
        /// A method that determines whether a given model is a user model or one of the built-in core models
        /// that can only used for primitive type resolution.
        /// </summary>
        /// <param name="model">The model to check.</param>
        /// <returns>true if the <paramref name="model"/> is a user model; otherwise false.</returns>
        internal static bool IsUserModel(this IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");

            return !(model is EdmCoreModel);
        }

        /// <summary>
        /// Checks whether the provided <paramref name="clrType"/> is a supported primitive type.
        /// </summary>
        /// <param name="clrType">The CLR type to check.</param>
        /// <returns>true if the <paramref name="clrType"/> is a supported primitive type; otherwise false.</returns>
        internal static bool IsPrimitiveType(Type clrType)
        {
            Debug.Assert(clrType != null, "clrType != null");

            // PERF
            switch (Type.GetTypeCode(clrType))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.String:
                case TypeCode.Single:
                    return true;

                default:
                    return PrimitiveTypeReferenceMap.ContainsKey(clrType) || typeof(ISpatial).IsAssignableFrom(clrType);
            }
        }

        /// <summary>
        /// Creates a collection value type for the specified <paramref name="itemTypeReference"/>.
        /// </summary>
        /// <param name="itemTypeReference">The <see cref="IEdmPrimitiveTypeReference"/> for the item type.</param>
        /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for primitive type references only.")]
        internal static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmPrimitiveTypeReference itemTypeReference)
        {
            IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference);
            return (IEdmCollectionTypeReference)ToTypeReference(collectionType);
        }

        /// <summary>
        /// Creates a collection type for the specified <paramref name="itemTypeReference"/>.
        /// </summary>
        /// <param name="itemTypeReference">The <see cref="IEdmComplexTypeReference"/> for the item type.</param>
        /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for complex type references only.")]
        internal static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmComplexTypeReference itemTypeReference)
        {
            IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference);
            return (IEdmCollectionTypeReference)ToTypeReference(collectionType);
        }

        /// <summary>
        /// Checks if the <paramref name="baseType"/> type reference is assignable from the <paramref name="subtype"/> type reference.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        /// <remarks>Note that this method only checks the type definition for assignability; it does not consider nullability
        /// or any other facets of the type reference.</remarks>
        internal static bool IsAssignableFrom(this IEdmTypeReference baseType, IEdmTypeReference subtype)
        {
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(subtype != null, "subtype != null");

            // We only consider the type definition but no facets (incl. nullability) here.
            return baseType.Definition.IsAssignableFrom(subtype.Definition);
        }

        /// <summary>
        /// Checks if the <paramref name="baseType"/> type is assignable from the <paramref name="subtype"/> type.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        internal static bool IsAssignableFrom(this IEdmType baseType, IEdmType subtype)
        {
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(subtype != null, "subtype != null");

            EdmTypeKind baseTypeKind = baseType.TypeKind;
            EdmTypeKind subTypeKind = subtype.TypeKind;

            // If the type kinds don't match, the types are not assignable.
            if (baseTypeKind != subTypeKind)
            {
                return false;
            }

            switch (baseTypeKind)
            {
                case EdmTypeKind.Primitive:
                    return ((IEdmPrimitiveType)baseType).IsAssignableFrom((IEdmPrimitiveType)subtype);

                case EdmTypeKind.Entity:    // fall through
                case EdmTypeKind.Complex:
                    return ((IEdmStructuredType)baseType).IsAssignableFrom((IEdmStructuredType)subtype);

                case EdmTypeKind.Collection:
                    // NOTE: we do allow inheritance (or co-/contra-variance) in collection types.
                    return ((IEdmCollectionType)baseType).ElementType.Definition.IsAssignableFrom(((IEdmCollectionType)subtype).ElementType.Definition);

                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_IsAssignableFrom_Type));
            }
        }

        /// <summary>
        /// Checks if the <paramref name="firstType"/> structured type and the <paramref name="secondType"/> structured type
        /// have a common base type.
        /// In other words, if <paramref name="secondType"/> is a subtype of <paramref name="firstType"/> or not.
        /// </summary>
        /// <param name="firstType">Type of the base type.</param>
        /// <param name="secondType">Type of the sub type.</param>
        /// <returns>The common base type or null if no common base type exists.</returns>
        internal static IEdmStructuredType GetCommonBaseType(this IEdmStructuredType firstType, IEdmStructuredType secondType)
        {
            Debug.Assert(firstType != null, "firstType != null");
            Debug.Assert(secondType != null, "secondType != null");

            if (firstType.IsEquivalentTo(secondType))
            {
                return firstType;
            }

            IEdmStructuredType commonBaseType = firstType;
            while (commonBaseType != null)
            {
                if (commonBaseType.IsAssignableFrom(secondType))
                {
                    return commonBaseType;
                }

                commonBaseType = commonBaseType.BaseType;
            }

            commonBaseType = secondType;
            while (commonBaseType != null)
            {
                if (commonBaseType.IsAssignableFrom(firstType))
                {
                    return commonBaseType;
                }

                commonBaseType = commonBaseType.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Checks if the <paramref name="firstType"/> primitive type and the <paramref name="secondType"/> primitive type
        /// have a common base type.
        /// In other words, if <paramref name="secondType"/> is a subtype of <paramref name="firstType"/> or not.
        /// </summary>
        /// <param name="firstType">Type of the base type.</param>
        /// <param name="secondType">Type of the sub type.</param>
        /// <returns>The common base type or null if no common base type exists.</returns>
        internal static IEdmPrimitiveType GetCommonBaseType(this IEdmPrimitiveType firstType, IEdmPrimitiveType secondType)
        {
            Debug.Assert(firstType != null, "firstType != null");
            Debug.Assert(secondType != null, "secondType != null");

            if (firstType.IsEquivalentTo(secondType))
            {
                return firstType;
            }

            IEdmPrimitiveType commonBaseType = firstType;
            while (commonBaseType != null)
            {
                if (commonBaseType.IsAssignableFrom(secondType))
                {
                    return commonBaseType;
                }

                commonBaseType = commonBaseType.BaseType();
            }

            commonBaseType = secondType;
            while (commonBaseType != null)
            {
                if (commonBaseType.IsAssignableFrom(firstType))
                {
                    return commonBaseType;
                }

                commonBaseType = commonBaseType.BaseType();
            }

            return null;
        }

        /// <summary>
        /// Returns the base type of a primitive type.
        /// </summary>
        /// <param name="type">The <see cref="IEdmPrimitiveType"/> to get the base type for.</param>
        /// <returns>The base type of the <paramref name="type"/> or null if no base type exists.</returns>
        internal static IEdmPrimitiveType BaseType(this IEdmPrimitiveType type)
        {
            Debug.Assert(type != null, "type != null");

            switch (type.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.None:
                case EdmPrimitiveTypeKind.Binary:
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Decimal:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.String:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Geometry:
                    return null;

                case EdmPrimitiveTypeKind.GeographyPoint:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);

                case EdmPrimitiveTypeKind.GeographyLineString:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);

                case EdmPrimitiveTypeKind.GeographyPolygon:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);

                case EdmPrimitiveTypeKind.GeographyCollection:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);

                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);

                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);

                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);

                case EdmPrimitiveTypeKind.GeometryPoint:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);

                case EdmPrimitiveTypeKind.GeometryLineString:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);

                case EdmPrimitiveTypeKind.GeometryPolygon:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);

                case EdmPrimitiveTypeKind.GeometryCollection:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);

                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);

                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);

                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);

                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_BaseType));
            }
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmComplexTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmComplexTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmComplexTypeReference AsComplexOrNull(this IEdmTypeReference typeReference)
        {
            if (typeReference == null)
            {
                return null;
            }

            return typeReference.TypeKind() == EdmTypeKind.Complex ? typeReference.AsComplex() : null;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmCollectionTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmCollectionTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmCollectionTypeReference AsCollectionOrNull(this IEdmTypeReference typeReference)
        {
            if (typeReference == null)
            {
                return null;
            }

            if (typeReference.TypeKind() != EdmTypeKind.Collection)
            {
                return null;
            }

            IEdmCollectionTypeReference collectionTypeReference = typeReference.AsCollection();
            if (!collectionTypeReference.IsNonEntityCollectionType())
            {
                return null;
            }

            return collectionTypeReference;
        }

        /// <summary>
        /// Resolves the name of a primitive type.
        /// </summary>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        internal static IEdmSchemaType ResolvePrimitiveTypeName(string typeName)
        {
            return EdmCoreModel.Instance.FindDeclaredType(typeName);
        }

        /// <summary>
        /// Get the <see cref="IEdmEntityTypeReference"/> of the item type of the <paramref name="typeReference"/>.
        /// </summary>
        /// <param name="typeReference">The collection type to get the item type for.</param>
        /// <returns>The item type of the <paramref name="typeReference"/>.</returns>
        internal static IEdmTypeReference GetCollectionItemType(this IEdmTypeReference typeReference)
        {
            IEdmCollectionTypeReference collectionType = typeReference.AsCollectionOrNull();
            return collectionType == null ? null : collectionType.ElementType();
        }

        /// <summary>
        /// Returns the IEdmCollectionType implementation with the given IEdmType as element type.
        /// </summary>
        /// <param name="itemType">IEdmType instance which is the element type.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance using the <paramref name="itemType"/> as Collection item type.</returns>
        internal static IEdmCollectionType GetCollectionType(IEdmType itemType)
        {
            Debug.Assert(itemType != null, "itemType != null");

            IEdmTypeReference itemTypeReference = itemType.ToTypeReference();
            return GetCollectionType(itemTypeReference);
        }

        /// <summary>
        /// Returns the IEdmCollectionType implementation with the given IEdmTypeReference as element type.
        /// </summary>
        /// <param name="itemTypeReference">IEdmTypeReference instance which is the element type.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance using the <paramref name="itemTypeReference"/> as Collection item type.</returns>
        internal static IEdmCollectionType GetCollectionType(IEdmTypeReference itemTypeReference)
        {
            Debug.Assert(itemTypeReference != null, "itemTypeReference != null");

            if (!itemTypeReference.IsODataPrimitiveTypeKind() && !itemTypeReference.IsODataComplexTypeKind() && !itemTypeReference.IsODataEnumTypeKind())
            {
                throw new ODataException(ErrorStrings.EdmLibraryExtensions_CollectionItemCanBeOnlyPrimitiveEnumComplex);
            }

            return new EdmCollectionType(itemTypeReference);
        }

        /// <summary>
        /// Checks whether a type reference is a Geography type.
        /// </summary>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is a Geography type; otherwise false.</returns>
        internal static bool IsGeographyType(this IEdmTypeReference typeReference)
        {
            IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitiveOrNull();
            if (primitiveTypeReference == null)
            {
                return false;
            }

            switch (primitiveTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether a type reference is a Geometry type.
        /// </summary>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is a Geometry type; otherwise false.</returns>
        internal static bool IsGeometryType(this IEdmTypeReference typeReference)
        {
            IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitiveOrNull();
            if (primitiveTypeReference == null)
            {
                return false;
            }

            switch (primitiveTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns CollectionValue item type name or null if the provided type name is not a collectionValue.
        /// </summary>
        /// <param name="typeName">CollectionValue type name read from payload.</param>
        /// <returns>CollectionValue element type name or null if not a collectionValue.</returns>
        internal static string GetCollectionItemTypeName(string typeName)
        {
            return GetCollectionItemTypeName(typeName, false);
        }

        /// <summary>
        /// Gets the collection full type name from the given type string.
        /// </summary>
        /// <param name="typeName">The original collection type string.</param>
        /// <returns>The full type name for the given origin type name/>.</returns>
        internal static string GetCollectionTypeFullName(string typeName)
        {
            if (typeName != null)
            {
                string innerTypeName = GetCollectionItemTypeName(typeName);
                if (innerTypeName != null)
                {
                    IEdmSchemaType primitiveType = EdmCoreModel.Instance.FindDeclaredType(innerTypeName);
                    if (primitiveType != null)
                    {
                        return GetCollectionTypeName(primitiveType.FullName());
                    }
                }
            }

            return typeName;
        }

        /// <summary>
        /// Determines whether operations bound to this type must be qualified with the operation they belong to when appearing in a $select clause.
        /// </summary>
        /// <param name="entityType">The entity type the operations are bound to.</param>
        /// <returns>True if the operations must be container qualified, otherwise false.</returns>
        internal static bool OperationsBoundToEntityTypeMustBeContainerQualified(this IEdmEntityType entityType)
        {
            Debug.Assert(entityType != null, "entityType != null");
            return entityType.IsOpen;
        }
#endif
        #endregion

        #region ODataLib and WCF DS Server
#if !ODATALIB_QUERY && !ASTORIA_CLIENT
        /// <summary>
        /// Gets the full name of the definition referred to by the type reference.
        /// </summary>
        /// <param name="typeReference">The type reference to get the full name for.</param>
        /// <returns>The full name of this <paramref name="typeReference"/>.</returns>
        /// <remarks>
        /// Note that this method is different from the EdmLib FullName extension method in that it also returns
        /// names for collection types. For EdmLib, collection types are functions and thus don't have a full name.
        /// The name/string they use in CSDL is just shorthand for them.
        /// </remarks>
        internal static string ODataFullName(this IEdmTypeReference typeReference)
        {
#if !ASTORIA_SERVER
#endif
            Debug.Assert(typeReference != null, "typeReference != null");
            Debug.Assert(typeReference.Definition != null, "typeReference.Definition != null");
            return typeReference.Definition.ODataFullName();
        }

        /// <summary>
        /// Gets the full name of the type.
        /// </summary>
        /// <param name="type">The type to get the full name for.</param>
        /// <returns>The full name of the <paramref name="type"/>.</returns>
        /// <remarks>
        /// Note that this method is different from the EdmLib FullName extension method in that it also returns
        /// names for collection types. For EdmLib, collection types are functions and thus don't have a full name.
        /// The name/string they use in CSDL is just shorthand for them.
        /// </remarks>
        internal static string ODataFullName(this IEdmType type)
        {
#if !ASTORIA_SERVER
#endif
            Debug.Assert(type != null, "type != null");

            // Handle collection type names here since for EdmLib collection values are functions
            // that do not have a full name
            IEdmCollectionType collectionType = type as IEdmCollectionType;
            if (collectionType != null)
            {
                string elementTypeName = collectionType.ElementType.ODataFullName();
                if (elementTypeName == null)
                {
                    return null;
                }

                return GetCollectionTypeName(elementTypeName);
            }

            var namedDefinition = type as IEdmSchemaElement;
            return namedDefinition != null ? namedDefinition.FullName() : null;
        }

        /// <summary>
        /// Gets the Partail name of the definition referred to by the type reference.
        /// </summary>
        /// <param name="typeReference">The type reference to get the partial name for.</param>
        /// <returns>The partial name of this <paramref name="typeReference"/>.</returns>
        /// <remarks>
        /// Note that this method is different from the EdmLib PartialName extension method in that it also returns
        /// names for collection types. For EdmLib, collection types are functions and thus don't have a Partial name.
        /// The name/string they use in CSDL is just shorthand for them.
        /// </remarks>
        internal static string ODataShortQualifiedName(this IEdmTypeReference typeReference)
        {
#if !ASTORIA_SERVER
#endif
            Debug.Assert(typeReference != null, "typeReference != null");
            Debug.Assert(typeReference.Definition != null, "typeReference.Definition != null");
            return typeReference.Definition.ODataShortQualifiedName();
        }

        /// <summary>
        /// Gets the Partial name of the type.
        /// </summary>
        /// <param name="type">The type to get the partial name for.</param>
        /// <returns>The partial name of the <paramref name="type"/>.</returns>
        /// <remarks>
        /// Note that this method is different from the EdmLib PartialName extension method in that it also returns
        /// names for collection types. For EdmLib, collection types are functions and thus don't have a full name.
        /// The name/string they use in CSDL is just shorthand for them.
        /// </remarks>
        internal static string ODataShortQualifiedName(this IEdmType type)
        {
#if !ASTORIA_SERVER
#endif
            Debug.Assert(type != null, "type != null");

            // Handle collection type names here since for EdmLib collection values are functions
            // that do not have a full name
            IEdmCollectionType collectionType = type as IEdmCollectionType;
            if (collectionType != null)
            {
                string elementTypeName = collectionType.ElementType.ODataShortQualifiedName();
                if (elementTypeName == null)
                {
                    return null;
                }

                return GetCollectionTypeName(elementTypeName);
            }

            var namedDefinition = type as IEdmSchemaElement;
            return namedDefinition != null ? namedDefinition.ShortQualifiedName() : null;
        }

        /// <summary>
        /// Clones the specified type reference.
        /// </summary>
        /// <param name="typeReference">The type reference to clone.</param>
        /// <param name="nullable">true to make the cloned type reference nullable; false to make it non-nullable.</param>
        /// <returns>The cloned <see cref="IEdmTypeReference"/> instance.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The clone logic should stay in one place.")]
        internal static IEdmTypeReference Clone(this IEdmTypeReference typeReference, bool nullable)
        {
            if (typeReference == null)
            {
                return null;
            }

            EdmTypeKind typeKind = typeReference.TypeKind();
            switch (typeKind)
            {
                case EdmTypeKind.Primitive:
                    EdmPrimitiveTypeKind kind = typeReference.PrimitiveKind();
                    IEdmPrimitiveType primitiveType = (IEdmPrimitiveType)typeReference.Definition;
                    switch (kind)
                    {
                        case EdmPrimitiveTypeKind.Boolean:
                        case EdmPrimitiveTypeKind.Byte:
                        case EdmPrimitiveTypeKind.Double:
                        case EdmPrimitiveTypeKind.Guid:
                        case EdmPrimitiveTypeKind.Int16:
                        case EdmPrimitiveTypeKind.Int32:
                        case EdmPrimitiveTypeKind.Int64:
                        case EdmPrimitiveTypeKind.SByte:
                        case EdmPrimitiveTypeKind.Single:
                        case EdmPrimitiveTypeKind.Stream:
                            return new EdmPrimitiveTypeReference(primitiveType, nullable);
                        case EdmPrimitiveTypeKind.Binary:
                            IEdmBinaryTypeReference binaryTypeReference = (IEdmBinaryTypeReference)typeReference;
                            return new EdmBinaryTypeReference(
                                primitiveType,
                                nullable,
                                binaryTypeReference.IsUnbounded,
                                binaryTypeReference.MaxLength);

                        case EdmPrimitiveTypeKind.String:
                            IEdmStringTypeReference stringTypeReference = (IEdmStringTypeReference)typeReference;
                            return new EdmStringTypeReference(
                                primitiveType,
                                nullable,
                                stringTypeReference.IsUnbounded,
                                stringTypeReference.MaxLength,
                                stringTypeReference.IsUnicode);

                        case EdmPrimitiveTypeKind.Decimal:
                            IEdmDecimalTypeReference decimalTypeReference = (IEdmDecimalTypeReference)typeReference;
                            return new EdmDecimalTypeReference(primitiveType, nullable, decimalTypeReference.Precision, decimalTypeReference.Scale);

                        case EdmPrimitiveTypeKind.DateTimeOffset:
                        case EdmPrimitiveTypeKind.Duration:
                            IEdmTemporalTypeReference temporalTypeReference = (IEdmTemporalTypeReference)typeReference;
                            return new EdmTemporalTypeReference(primitiveType, nullable, temporalTypeReference.Precision);

                        case EdmPrimitiveTypeKind.Geography:
                        case EdmPrimitiveTypeKind.GeographyPoint:
                        case EdmPrimitiveTypeKind.GeographyLineString:
                        case EdmPrimitiveTypeKind.GeographyPolygon:
                        case EdmPrimitiveTypeKind.GeographyCollection:
                        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                        case EdmPrimitiveTypeKind.GeographyMultiLineString:
                        case EdmPrimitiveTypeKind.GeographyMultiPoint:
                        case EdmPrimitiveTypeKind.Geometry:
                        case EdmPrimitiveTypeKind.GeometryCollection:
                        case EdmPrimitiveTypeKind.GeometryPoint:
                        case EdmPrimitiveTypeKind.GeometryLineString:
                        case EdmPrimitiveTypeKind.GeometryPolygon:
                        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                        case EdmPrimitiveTypeKind.GeometryMultiLineString:
                        case EdmPrimitiveTypeKind.GeometryMultiPoint:
                            IEdmSpatialTypeReference spatialTypeReference = (IEdmSpatialTypeReference)typeReference;
                            return new EdmSpatialTypeReference(primitiveType, nullable, spatialTypeReference.SpatialReferenceIdentifier);

                        default:
                            throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_Clone_PrimitiveTypeKind));
                    }

                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference((IEdmEntityType)typeReference.Definition, nullable);

                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference((IEdmComplexType)typeReference.Definition, nullable);

                case EdmTypeKind.Collection:
                    return new EdmCollectionTypeReference((IEdmCollectionType)typeReference.Definition);

                case EdmTypeKind.EntityReference:
                    return new EdmEntityReferenceTypeReference((IEdmEntityReferenceType)typeReference.Definition, nullable);

                case EdmTypeKind.Enum:
                    return new EdmEnumTypeReference((IEdmEnumType)typeReference.Definition, nullable);

                case EdmTypeKind.None:  // fall through
                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_Clone_TypeKind));
            }
        }

        /// <summary>
        /// Gets the name of a operation import group.
        /// </summary>
        /// <param name="functionImportGroup">The operation import group in question.</param>
        /// <returns>The name of the operation import group.</returns>
        internal static string FunctionImportGroupName(this IEnumerable<IEdmOperationImport> functionImportGroup)
        {
            Debug.Assert(functionImportGroup != null && functionImportGroup.Any(), "functionImportGroup != null && functionImportGroup.Any()");

            string name = functionImportGroup.First().Name;
            Debug.Assert(functionImportGroup.All(f => f.Name == name), "functionImportGroup.All(f => f.Name == name)");
            return name;
        }

        /// <summary>
        /// Gets the full name of a operation group.
        /// </summary>
        /// <param name="operationGroup">The operation group in question.</param>
        /// <returns>The full name of the operation group.</returns>
        internal static string OperationGroupFullName(this IEnumerable<IEdmOperation> operationGroup)
        {
            // TODO: Resolve duplication of operationImport and operation
            Debug.Assert(operationGroup != null && operationGroup.Any(), "operationGroup != null && operationGroup.Any()");

            string fullName = operationGroup.First().FullName();
            Debug.Assert(operationGroup.All(f => f.FullName() == fullName), "operationGroup.All(f => f.FullName() == fullName)");
            return fullName;
        }

        /// <summary>
        /// Gets the full name of a operation import  group.
        /// </summary>
        /// <param name="operationImportGroup">The operation import group in question.</param>
        /// <returns>The full name of the operation import group.</returns>
        internal static string OperationImportGroupFullName(this IEnumerable<IEdmOperationImport> operationImportGroup)
        {
            // TODO: Resolve duplication of operationImport and operation
            Debug.Assert(operationImportGroup != null && operationImportGroup.Any(), "operationImportGroup != null && operationImportGroup.Any()");

            string fullName = operationImportGroup.First().FullName();
            Debug.Assert(operationImportGroup.All(f => f.FullName() == fullName), "operationImportGroup.All(f => f.FullName() == fullName)");
            return fullName;
        }
#endif
        #endregion

        #region ODataLib Contrib only (stuff that is not yet released)
#if !ASTORIA_SERVER && !ASTORIA_CLIENT

        /// <summary>
        /// Gets the multiplicity of a navigation property.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>The multiplicity of the navigation property in question.</returns>
        /// <remarks>This has been added to EdmLib, but EdmLib won't be released for a while.
        /// If you need to use this functionality before we release EdmLib, then use this method. Change your calls
        /// to use the real method whenever we release EdmLib again.</remarks>
        internal static EdmMultiplicity TargetMultiplicityTemporary(this IEdmNavigationProperty property)
        {
            Debug.Assert(property != null, "property != null");

            IEdmTypeReference type = property.Type;
            if (type.IsCollection())
            {
                return EdmMultiplicity.Many;
            }

            return type.IsNullable ? EdmMultiplicity.ZeroOrOne : EdmMultiplicity.One;
        }

#endif
        #endregion

        #region ODataLib and Query project
#if !ASTORIA_SERVER && !ASTORIA_CLIENT
        /// <summary>
        /// Checks if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for structured types only.")]
        internal static bool IsAssignableFrom(this IEdmStructuredType baseType, IEdmStructuredType subtype)
        {
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(subtype != null, "subtype != null");

            if (baseType.TypeKind != subtype.TypeKind)
            {
                return false;
            }

            if (!baseType.IsODataEntityTypeKind() && !baseType.IsODataComplexTypeKind())
            {
                // we only support complex and entity type inheritance.
                return false;
            }

            IEdmStructuredType structuredSubType = subtype;
            while (structuredSubType != null)
            {
                if (structuredSubType.IsEquivalentTo(baseType))
                {
                    return true;
                }

                structuredSubType = structuredSubType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Checks if the primitive type is a geography or geometry type.
        /// </summary>
        /// <param name="primitiveType">The type to check.</param>
        /// <returns>true, if the <paramref name="primitiveType"/> is a geography or geometry type.</returns>
        internal static bool IsSpatialType(this IEdmPrimitiveType primitiveType)
        {
            Debug.Assert(primitiveType != null, "primitiveType != null");

            switch (primitiveType.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return true;

                case EdmPrimitiveTypeKind.None:
                case EdmPrimitiveTypeKind.Binary:
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Decimal:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.String:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Duration:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the <paramref name="baseType"/> primitive type is assignable to <paramref name="subtype"/> primitive type.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Need to keep code together.")]
        internal static bool IsAssignableFrom(this IEdmPrimitiveType baseType, IEdmPrimitiveType subtype)
        {
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(subtype != null, "subtype != null");

            if (baseType.IsEquivalentTo(subtype))
            {
                return true;
            }

            // Only spatial types are assignable
            if (!baseType.IsSpatialType() || !subtype.IsSpatialType())
            {
                return false;
            }

            // For two spatial types, test for assignability
            EdmPrimitiveTypeKind baseTypeKind = baseType.PrimitiveKind;
            EdmPrimitiveTypeKind subTypeKind = subtype.PrimitiveKind;

            switch (baseTypeKind)
            {
                case EdmPrimitiveTypeKind.Geography:
                    return subTypeKind == EdmPrimitiveTypeKind.Geography ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyCollection ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPolygon ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyPolygon;

                case EdmPrimitiveTypeKind.GeographyPoint:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyPoint;

                case EdmPrimitiveTypeKind.GeographyLineString:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyLineString;

                case EdmPrimitiveTypeKind.GeographyPolygon:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyPolygon;

                case EdmPrimitiveTypeKind.GeographyCollection:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyCollection ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPolygon;

                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPolygon;

                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyMultiLineString;

                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPoint;

                case EdmPrimitiveTypeKind.Geometry:
                    return subTypeKind == EdmPrimitiveTypeKind.Geometry ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryCollection ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPolygon ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryPolygon;

                case EdmPrimitiveTypeKind.GeometryPoint:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryPoint;

                case EdmPrimitiveTypeKind.GeometryLineString:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryLineString;

                case EdmPrimitiveTypeKind.GeometryPolygon:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryPolygon;

                case EdmPrimitiveTypeKind.GeometryCollection:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryCollection ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPolygon;

                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPolygon;

                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryMultiLineString;

                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPoint;

                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_IsAssignableFrom_Primitive));
            }
        }

        /// <summary>
        /// Returns the primitive CLR type for the specified primitive type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The primitive type to resolve.</param>
        /// <returns>The CLR type for the primitive type reference.</returns>
        internal static Type GetPrimitiveClrType(IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

            return GetPrimitiveClrType(primitiveTypeReference.PrimitiveDefinition(), primitiveTypeReference.IsNullable);
        }

        /// <summary>
        /// Turns a <see cref="IEdmType"/> into the corresponding non-nullable <see cref="IEdmTypeReference"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>A non-nullable type reference for the <paramref name="type"/>.</returns>
        internal static IEdmTypeReference ToTypeReference(this IEdmType type)
        {
            return ToTypeReference(type, false /*nullable*/);
        }

        /// <summary>
        /// Determines whether the provided <paramref name="type"/> is an open type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the <paramref name="type"/> is an open type; otherwise false.</returns>
        internal static bool IsOpenType(this IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            IEdmStructuredType structuredType = type as IEdmStructuredType;
            if (structuredType != null)
            {
                return structuredType.IsOpen;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the provided <paramref name="type"/> is a stream.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the <paramref name="type"/> represents a stream; otherwise false.</returns>
        internal static bool IsStream(this IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                Debug.Assert(type.TypeKind != EdmTypeKind.Primitive, "Invalid type kind.");
                return false;
            }

            Debug.Assert(primitiveType.TypeKind == EdmTypeKind.Primitive, "Expected primitive type kind.");
            return primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Stream;
        }

#if !ODATALIB_QUERY || DEBUG
        /// <summary>
        /// Checks whether the specified <paramref name="property"/> is defined for the type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to check the properties on.</param>
        /// <param name="property">The property to check for.</param>
        /// <returns>true if the <paramref name="property"/> is defined for the <paramref name="type"/>; otherwise false.</returns>
        internal static bool ContainsProperty(this IEdmType type, IEdmProperty property)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(property != null, "property != null");

            IEdmComplexType complexType = type as IEdmComplexType;
            if (complexType != null)
            {
                // NOTE: using Any() instead of Contains() since Contains() does not exist on all platforms
                return complexType.Properties().Any(p => p == property);
            }

            IEdmEntityType entityType = type as IEdmEntityType;
            if (entityType != null)
            {
                // NOTE: using Any() instead of Contains() since Contains() does not exist on all platforms
                return entityType.Properties().Any(p => p == property) ||
                       entityType.NavigationProperties().Any(p => p == property);
            }

            // we only support complex and entity types with properties so far
            return false;
        }

        /// <summary>
        /// Checks whether the specified <paramref name="property"/> is defined for the type <paramref name="typeReference"/>.
        /// </summary>
        /// <param name="typeReference">The type to check the properties on.</param>
        /// <param name="property">The property to check for.</param>
        /// <returns>true if the <paramref name="property"/> is defined for the <paramref name="typeReference"/>; otherwise false.</returns>
        internal static bool ContainsProperty(this IEdmTypeReference typeReference, IEdmProperty property)
        {
            Debug.Assert(typeReference != null, "typeReference != null");
            Debug.Assert(property != null, "property != null");

            IEdmStructuredTypeReference structuredTypeReference = typeReference.AsStructuredOrNull();
            if (structuredTypeReference == null)
            {
                return false;
            }

            return ContainsProperty(structuredTypeReference.Definition, property);
        }
#endif
#endif
        #endregion

        #region Everyone
        /// <summary>
        /// Returns the fully qualified name of an entity container element.
        /// </summary>
        /// <param name="containerElement">The container element to get the full name for.</param>
        /// <returns>The full name of the owning entity container, slash, name of the container element.</returns>
#if ASTORIA_CLIENT || ODATALIB_QUERY
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Will be used in a later change")]
#endif
        internal static string FullName(this IEdmEntityContainerElement containerElement)
        {
#if !ASTORIA_SERVER && !ASTORIA_CLIENT
#endif
            Debug.Assert(containerElement != null, "containerElement != null");

            return containerElement.Container.Name + "." + containerElement.Name;
        }

#if !ASTORIA_CLIENT
        /// <summary>
        /// Returns the primitive type reference for the given Clr type.
        /// </summary>
        /// <param name="clrType">The Clr type to resolve.</param>
        /// <returns>The primitive type reference for the given Clr type.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "cyclomatic complexity")]
        internal static IEdmPrimitiveTypeReference GetPrimitiveTypeReference(Type clrType)
        {
            Debug.Assert(clrType != null, "clrType != null");

            TypeCode typeCode = Type.GetTypeCode(clrType);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return BooleanTypeReference;

                case TypeCode.Byte:
                    return ByteTypeReference;

                case TypeCode.Decimal:
                    return DecimalTypeReference;

                case TypeCode.Double:
                    return DoubleTypeReference;

                case TypeCode.Int16:
                    return Int16TypeReference;

                case TypeCode.Int32:
                    return Int32TypeReference;

                case TypeCode.Int64:
                    return Int64TypeReference;

                case TypeCode.SByte:
                    return SByteTypeReference;

                case TypeCode.String:
                    return StringTypeReference;

                case TypeCode.Single:
                    return SingleTypeReference;
            }

            // Try to lookup the type in our map.
            IEdmPrimitiveTypeReference primitiveTypeReference;
            if (PrimitiveTypeReferenceMap.TryGetValue(clrType, out primitiveTypeReference))
            {
                return primitiveTypeReference;
            }

            // If it didn't work, try spatial types which need assignability.
            IEdmPrimitiveType primitiveType = null;
            if (typeof(GeographyPoint).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint);
            }
            else if (typeof(GeographyLineString).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString);
            }
            else if (typeof(GeographyPolygon).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon);
            }
            else if (typeof(GeographyMultiPoint).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint);
            }
            else if (typeof(GeographyMultiLineString).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString);
            }
            else if (typeof(GeographyMultiPolygon).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon);
            }
            else if (typeof(GeographyCollection).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
            }
            else if (typeof(Geography).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
            }
            else if (typeof(GeometryPoint).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint);
            }
            else if (typeof(GeometryLineString).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString);
            }
            else if (typeof(GeometryPolygon).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon);
            }
            else if (typeof(GeometryMultiPoint).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint);
            }
            else if (typeof(GeometryMultiLineString).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString);
            }
            else if (typeof(GeometryMultiPolygon).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon);
            }
            else if (typeof(GeometryCollection).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
            }
            else if (typeof(Geometry).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
            }

            if (primitiveType == null)
            {
                return null;
            }

            // All spatial CLR types are inherently nullable
            return ToTypeReference(primitiveType, true);
        }
#endif

        /// <summary>
        /// Turns a <see cref="IEdmType"/> into the corresponding <see cref="IEdmTypeReference"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <param name="nullable">true if the returned type reference should be nullable; otherwise false.</param>
        /// <returns>A type reference for the <paramref name="type"/>.</returns>
        internal static IEdmTypeReference ToTypeReference(this IEdmType type, bool nullable)
        {
#if !ASTORIA_CLIENT
#endif

            if (type == null)
            {
                return null;
            }

            switch (type.TypeKind)
            {
                case EdmTypeKind.Primitive:
                    return ToTypeReference((IEdmPrimitiveType)type, nullable);
                case EdmTypeKind.Enum:
                    return new EdmEnumTypeReference((IEdmEnumType)type, nullable);
                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference((IEdmComplexType)type, nullable);
                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference((IEdmEntityType)type, nullable);
                case EdmTypeKind.Collection:
                    return new EdmCollectionTypeReference((IEdmCollectionType)type);
                case EdmTypeKind.EntityReference:
                    return new EdmEntityReferenceTypeReference((IEdmEntityReferenceType)type, nullable);
                case EdmTypeKind.None:
                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_ToTypeReference));
            }
        }

        /// <summary>
        /// Creates the EDM type name for a collection of the specified item type name. E.g. Collection(Edm.String)
        /// </summary>
        /// <param name="itemTypeName">Type name of the items in the collection.</param>
        /// <returns>Type name for a collection of the specified item type name.</returns>
        internal static string GetCollectionTypeName(string itemTypeName)
        {
#if !ASTORIA_SERVER && !ASTORIA_CLIENT
#endif
            return string.Format(CultureInfo.InvariantCulture, CollectionTypeFormat, itemTypeName);
        }

#if !ASTORIA_CLIENT
        /// <summary>
        /// Resolves a operation import or operation import group.
        /// </summary>
        /// <param name="container">The entity container.</param>
        /// <param name="operationImportName">The operation import name to resolve. May contain parameter type names, e.g. Function1(P1Type,P2Type)</param>
        /// <returns>The resolved operation import or operation import group.</returns>
        internal static IEnumerable<IEdmOperationImport> ResolveOperationImports(this IEdmEntityContainer container, string operationImportName)
        {
            return ResolveOperationImports(container, operationImportName, true);
        }

        /// <summary>
        /// Resolves an operation import or operation import group.
        /// </summary>
        /// <param name="container">The entity container.</param>
        /// <param name="operationImportName">The operation import name to resolve. May contain parameter type names, e.g. Function1(P1Type,P2Type)</param>
        /// <param name="allowParameterTypeNames">Whether parameter type names are allowed to appear in the operation import name to resolve.</param>
        /// <returns>The resolved operation import or operation import group.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "allowParameterTypeNames", Justification = "Used in the ODL version of the method.")]
        internal static IEnumerable<IEdmOperationImport> ResolveOperationImports(this IEdmEntityContainer container, string operationImportName, bool allowParameterTypeNames)
        {
            // TODO: Resolve duplication of operationImport and operation
            Debug.Assert(container != null, "container != null");

            if (string.IsNullOrEmpty(operationImportName))
            {
                return Enumerable.Empty<IEdmOperationImport>();
            }

#if !ASTORIA_SERVER && !ASTORIA_CLIENT && !ODATALIB_QUERY
            int indexOfParameterStart = operationImportName.IndexOf(JsonLightConstants.FunctionParameterStart);
            string functionImportNameWithoutParameterTypes = operationImportName;
            if (indexOfParameterStart > 0)
            {
                if (!allowParameterTypeNames)
                {
                    return Enumerable.Empty<IEdmOperationImport>();
                }

                functionImportNameWithoutParameterTypes = operationImportName.Substring(0, indexOfParameterStart);
            }
#else
            Debug.Assert(!allowParameterTypeNames, "Parameter type names not supported in the server version of this method.");
            string functionImportNameWithoutParameterTypes = operationImportName;
#endif
            string containerName = null;
            string operationNameWithoutContainerOrNamespace = functionImportNameWithoutParameterTypes;
            int lastPeriodPos = functionImportNameWithoutParameterTypes.LastIndexOf('.');
            if (lastPeriodPos > -1)
            {
                operationNameWithoutContainerOrNamespace = functionImportNameWithoutParameterTypes.Substring(lastPeriodPos, functionImportNameWithoutParameterTypes.Length - lastPeriodPos).TrimStart('.');
                containerName = functionImportNameWithoutParameterTypes.Substring(0, lastPeriodPos);
            }

            // if the container name of the operationImport doesn't equal the current container, don't search just return empty as there are no matches.
            if (containerName != null && !(container.Name.Equals(containerName) || container.FullName().Equals(containerName)))
            {
                return Enumerable.Empty<IEdmOperationImport>();
            }

            IEnumerable<IEdmOperationImport> operationImports = container.FindOperationImports(operationNameWithoutContainerOrNamespace);
            Debug.Assert(operationImports != null, "operationImports != null");
#if !ASTORIA_SERVER && !ASTORIA_CLIENT && !ODATALIB_QUERY
            if (indexOfParameterStart > 0)
            {
                return FilterByOperationParameterTypes(operationImports, functionImportNameWithoutParameterTypes, operationImportName);
            }
#endif
            return operationImports;
        }

        /// <summary>
        /// Returns the primitive CLR type for the specified primitive type reference.
        /// </summary>
        /// <param name="primitiveType">The primitive type to resolve.</param>
        /// <param name="isNullable">Whether the returned type should be a nullable variant or not.</param>
        /// <returns>The CLR type for the primitive type reference.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Not too complex for what this method does.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Class coupling is with all the primitive Clr types only.")]
        internal static Type GetPrimitiveClrType(IEdmPrimitiveType primitiveType, bool isNullable)
        {
            Debug.Assert(primitiveType != null, "primitiveType != null");

            switch (primitiveType.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    return typeof(byte[]);
                case EdmPrimitiveTypeKind.Boolean:
                    return isNullable ? typeof(Boolean?) : typeof(Boolean);
                case EdmPrimitiveTypeKind.Byte:
                    return isNullable ? typeof(Byte?) : typeof(Byte);
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
                case EdmPrimitiveTypeKind.Decimal:
                    return isNullable ? typeof(Decimal?) : typeof(Decimal);
                case EdmPrimitiveTypeKind.Double:
                    return isNullable ? typeof(Double?) : typeof(Double);
                case EdmPrimitiveTypeKind.Geography:
                    return typeof(Geography);
                case EdmPrimitiveTypeKind.GeographyCollection:
                    return typeof(GeographyCollection);
                case EdmPrimitiveTypeKind.GeographyLineString:
                    return typeof(GeographyLineString);
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    return typeof(GeographyMultiLineString);
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return typeof(GeographyMultiPoint);
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    return typeof(GeographyMultiPolygon);
                case EdmPrimitiveTypeKind.GeographyPoint:
                    return typeof(GeographyPoint);
                case EdmPrimitiveTypeKind.GeographyPolygon:
                    return typeof(GeographyPolygon);
                case EdmPrimitiveTypeKind.Geometry:
                    return typeof(Geometry);
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return typeof(GeometryCollection);
                case EdmPrimitiveTypeKind.GeometryLineString:
                    return typeof(GeometryLineString);
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    return typeof(GeometryMultiLineString);
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return typeof(GeometryMultiPoint);
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    return typeof(GeometryMultiPolygon);
                case EdmPrimitiveTypeKind.GeometryPoint:
                    return typeof(GeometryPoint);
                case EdmPrimitiveTypeKind.GeometryPolygon:
                    return typeof(GeometryPolygon);
                case EdmPrimitiveTypeKind.Guid:
                    return isNullable ? typeof(Guid?) : typeof(Guid);
                case EdmPrimitiveTypeKind.Int16:
                    return isNullable ? typeof(Int16?) : typeof(Int16);
                case EdmPrimitiveTypeKind.Int32:
                    return isNullable ? typeof(Int32?) : typeof(Int32);
                case EdmPrimitiveTypeKind.Int64:
                    return isNullable ? typeof(Int64?) : typeof(Int64);
                case EdmPrimitiveTypeKind.SByte:
                    return isNullable ? typeof(SByte?) : typeof(SByte);
                case EdmPrimitiveTypeKind.Single:
                    return isNullable ? typeof(Single?) : typeof(Single);
                case EdmPrimitiveTypeKind.Stream:
                    return typeof(Stream);
                case EdmPrimitiveTypeKind.String:
                    return typeof(String);
                case EdmPrimitiveTypeKind.Duration:
                    return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
                default:
                    return null;
            }
        }

#endif
        #endregion
        #endregion

        #region Private methods
        #region ODataLib only
#if ODATALIB

        /// <summary>
        /// Validates the kind of the operation group returns only on.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="operationNameWithoutParameterTypes">The operation name without parameter types.</param>
        /// <returns>Enumerable list of operations.</returns>
        /// <exception cref="ODataException">If there is an action and function in the sequence.</exception>
        private static IEnumerable<IEdmOperation> ValidateOperationGroupReturnsOnlyOnKind(IEnumerable<IEdmOperation> operations, string operationNameWithoutParameterTypes)
        {
            EdmSchemaElementKind? operationKind = null;
            foreach (IEdmOperation operation in operations)
            {
                if (operationKind == null)
                {
                    operationKind = operation.SchemaElementKind;
                }
                else
                {
                    if (operation.SchemaElementKind != operationKind)
                    {
                        throw new ODataException(ErrorStrings.EdmLibraryExtensions_OperationGroupReturningActionsAndFunctionsModelInvalid(operationNameWithoutParameterTypes));
                    }
                }

                yield return operation;
            }
        }
        
        /// <summary>
        /// Gets the operation parameter types in string.
        /// </summary>
        /// <param name="operation">Operation in question.</param>
        /// <returns>Comma separated operation parameter types enclosed in parantheses.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
            Justification = "This method is used for matching the name of the operation to something written by the server. So using the name is safe without resolving the type from the client.")]
        private static string ParameterTypesToString(this IEdmOperation operation)
        {
            // TODO: Resolve duplication of operationImport and operation
            return JsonLightConstants.FunctionParameterStart +
                string.Join(JsonLightConstants.FunctionParameterSeparator, operation.Parameters.Select(p => p.Type.FullName()).ToArray()) +
                JsonLightConstants.FunctionParameterEnd;
        }

        /// <summary>
        /// Returns Collection item type name or null if the provided type name is not a collection.
        /// </summary>
        /// <param name="typeName">Collection type name.</param>
        /// <param name="isNested">Whether it is a nested (recursive) call.</param>
        /// <returns>Collection element type name or null if not a collection.</returns>
        /// <remarks>
        /// The following rules are used for collection type names:
        /// - it has to start with "Collection(" and end with ")" - trailing and leading whitespaces make the type not to be recognized as collection.
        /// - there is to be no characters (including whitespaces) between "Collection" and "(" - otherwise it won't berecognized as collection
        /// - collection item type name has to be a non-empty string - i.e. "Collection()" won't be recognized as collection
        /// - nested collection - e.g. "Collection(Collection(Edm.Int32))" - are not supported - we will throw
        /// Note the following are examples of valid type names which are not collection:
        /// - "Collection()"
        /// - " Collection(Edm.Int32)"
        /// - "Collection (Edm.Int32)"
        /// - "Collection("
        /// </remarks>
        private static string GetCollectionItemTypeName(string typeName, bool isNested)
        {
            int collectionTypeQualifierLength = CollectionTypeQualifier.Length;

            // to be recognized as a collection wireTypeName must not be null, has to start with "Collection(" and end with ")" and must not be "Collection()"
            if (typeName != null &&
                typeName.StartsWith(CollectionTypeQualifier + "(", StringComparison.Ordinal) &&
                typeName[typeName.Length - 1] == ')' &&
                typeName.Length != collectionTypeQualifierLength + 2)
            {
                if (isNested)
                {
                    throw new ODataException(ErrorStrings.ValidationUtils_NestedCollectionsAreNotSupported);
                }

                string innerTypeName = typeName.Substring(collectionTypeQualifierLength + 1, typeName.Length - (collectionTypeQualifierLength + 2));

                // Check if it is not a nested collection and throw if it is
                GetCollectionItemTypeName(innerTypeName, true);

                return innerTypeName;
            }

            return null;
        }

        /// <summary>
        /// Gets the operation import parameter types in string.
        /// </summary>
        /// <param name="operationImport">Function import in question.</param>
        /// <returns>Comma separated operation import parameter types enclosed in parantheses.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
            Justification = "This method is used for matching the name of the operation import to something written by the server. So using the name is safe without resolving the type from the client.")]
        private static string ParameterTypesToString(this IEdmOperationImport operationImport)
        {
            // TODO: Resolve duplication of operationImport and operation
            return JsonLightConstants.FunctionParameterStart +
                string.Join(JsonLightConstants.FunctionParameterSeparator, operationImport.Operation.Parameters.Select(p => p.Type.ODataFullName()).ToArray()) +
                JsonLightConstants.FunctionParameterEnd;
        }

        /// <summary>
        /// Filters the by operation parameter types.
        /// </summary>
        /// <param name="operationImports">The operation imports.</param>
        /// <param name="operationNameWithoutParameterTypes">The operation name without parameter types.</param>
        /// <param name="originalFullOperationImportName">Name of the original full operation import.</param>
        /// <returns>A list of EdmOperations that filters out operations that don't match.</returns>
        private static IEnumerable<IEdmOperationImport> FilterByOperationParameterTypes(this IEnumerable<IEdmOperationImport> operationImports, string operationNameWithoutParameterTypes, string originalFullOperationImportName)
        {
            Debug.Assert(operationImports != null, "operationImports != null");

            foreach (IEdmOperationImport operationImport in operationImports)
            {
                // If the operation name is not a full name then filter accordingly by parameter name.
                if (operationNameWithoutParameterTypes.IndexOf(".", StringComparison.Ordinal) > -1)
                {
                    if ((operationImport.FullNameWithParameters().Equals(originalFullOperationImportName, StringComparison.Ordinal)) ||
                        ((operationImport.Container.Name + "." + operationImport.NameWithParameters()).Equals(originalFullOperationImportName, StringComparison.Ordinal)))
                    {
                        yield return operationImport;
                    }
                }
                else
                {
                    if (operationImport.NameWithParameters().Equals(originalFullOperationImportName, StringComparison.Ordinal))
                    {
                        yield return operationImport;
                    }
                }
            }
        }

        /// <summary>
        /// Inheritances the type of the level from specified inherited.
        /// </summary>
        /// <param name="structuredType">Type of the structured.</param>
        /// <param name="rootType">Type of the root.</param>
        /// <returns>Get the inheritance level.</returns>
        private static int InheritanceLevelFromSpecifiedInheritedType(this IEdmStructuredType structuredType, IEdmStructuredType rootType)
        {
            Debug.Assert(structuredType.IsOrInheritsFrom(rootType), "Roottype should be a base of the structuredtype.");

            IEdmStructuredType currentType = structuredType;
            int inheritanceLevelsFromBase = 0;
            while (currentType.InheritsFrom(rootType))
            {
                currentType = currentType.BaseType;
                inheritanceLevelsFromBase++;
            }

            return inheritanceLevelsFromBase;
        }
#endif
        #endregion

        #region Everyone
        /// <summary>
        /// Gets a reference to a primitive kind definition of the appropriate kind.
        /// </summary>
        /// <param name="primitiveType">Primitive type to create a reference for.</param>
        /// <param name="nullable">Flag specifying if the referenced type should be nullable per default.</param>
        /// <returns>A new primitive type reference.</returns>
        private static EdmPrimitiveTypeReference ToTypeReference(IEdmPrimitiveType primitiveType, bool nullable)
        {
            EdmPrimitiveTypeKind kind = primitiveType.PrimitiveKind;
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                    return new EdmPrimitiveTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.Binary:
                    return new EdmBinaryTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.String:
                    return new EdmStringTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.Decimal:
                    return new EdmDecimalTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                    return new EdmTemporalTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return new EdmSpatialTypeReference(primitiveType, nullable);
                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_PrimitiveTypeReference));
            }
        }

        /// <summary>
        /// Equality comparer for an IEdmType.
        /// </summary>
        private sealed class EdmTypeEqualityComparer : IEqualityComparer<IEdmType>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type to compare.</param>
            /// <param name="y">The second object of type to compare.</param>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            public bool Equals(IEdmType x, IEdmType y)
            {
                return x.IsEquivalentTo(y);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The obj.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(IEdmType obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion
        #endregion
    }
}
#endif
