﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

#if !INTERNAL_DROP

namespace Microsoft.OData.Core
{
    /// <summary>
    /// An enumeration that lists the internal errors that are shared between the OData library and the query library.
    /// </summary>
    internal enum InternalErrorCodesCommon
    {
        /// <summary>Unreachable codepath in EdmLibraryExtensions.ToTypeReference (unsupported type kind).</summary>
        EdmLibraryExtensions_ToTypeReference,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.ToClrType (unsupported type kind).</summary>
        EdmLibraryExtensions_ToClrType,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.PrimitiveTypeReference (unsupported primitive type kind).</summary>
        EdmLibraryExtensions_PrimitiveTypeReference,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.IsAssignableFrom(IEdmPrimitiveType, IEdmPrimitiveType).</summary>
        EdmLibraryExtensions_IsAssignableFrom_Primitive,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.IsAssignableFrom(IEdmType, IEdmType).</summary>
        EdmLibraryExtensions_IsAssignableFrom_Type,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.BaseType.</summary>
        EdmLibraryExtensions_BaseType,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.Clone for unexpected type kind.</summary>
        EdmLibraryExtensions_Clone_TypeKind,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.Clone for unexpected primitive type kind.</summary>
        EdmLibraryExtensions_Clone_PrimitiveTypeKind,
    }
}
#endif
