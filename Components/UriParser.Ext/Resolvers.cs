using System;
using System.Collections.Generic;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;

namespace ODataSamples.UriParser.ParserExt
{
    /// <summary>
    /// Composition of two built in resolvers.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// new ODataUriParser(model, uri) { Resolver = new AllInOneResolver() {EnableCaseInsensitive = true}; };
    /// </remarks>
    class AllInOneResolver : ODataUriResolver
    {
        private StringAsEnumResolver stringAsEnum = new StringAsEnumResolver();
        private UnqualifiedODataUriResolver unqualified = new UnqualifiedODataUriResolver();

        private bool enableCaseInsensitive;

        public override bool EnableCaseInsensitive
        {
            get
            {
                return this.enableCaseInsensitive;
            }
            set
            {
                this.enableCaseInsensitive = value;
                stringAsEnum.EnableCaseInsensitive = this.enableCaseInsensitive;
                unqualified.EnableCaseInsensitive = this.enableCaseInsensitive;
            }
        }

        public override IEnumerable<IEdmOperation> ResolveBoundOperations(
            IEdmModel model,
            string identifier,
            IEdmType bindingType)
        {
            return unqualified.ResolveBoundOperations(model, identifier, bindingType);
        }

        public override void PromoteBinaryOperandTypes(
            BinaryOperatorKind binaryOperatorKind,
            ref SingleValueNode leftNode,
            ref SingleValueNode rightNode,
            out IEdmTypeReference typeReference)
        {
            stringAsEnum.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
        }

        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
            IEdmEntityType type,
            IDictionary<string, string> namedValues,
            Func<IEdmTypeReference, string, object> convertFunc)
        {
            return stringAsEnum.ResolveKeys(type, namedValues, convertFunc);
        }

        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
            IEdmEntityType type,
            IList<string> positionalValues,
            Func<IEdmTypeReference, string, object> convertFunc)
        {
            return stringAsEnum.ResolveKeys(type, positionalValues, convertFunc);
        }

        public override IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(
            IEdmOperation operation, IDictionary<string, SingleValueNode> input)
        {
            return stringAsEnum.ResolveOperationParameters(operation, input);
        }
    }

    class StringRepResolver : ODataUriResolver
    {
        public override void PromoteBinaryOperandTypes(
            BinaryOperatorKind binaryOperatorKind,
            ref SingleValueNode leftNode,
            ref SingleValueNode rightNode,
            out IEdmTypeReference typeReference)
        {
            if (binaryOperatorKind == BinaryOperatorKind.Multiply
                && leftNode.TypeReference != null
                && leftNode.TypeReference.IsString()
                && rightNode.TypeReference != null
                && rightNode.TypeReference.IsInt32())
            {
                // The result type should be Edm.String, as it could be nullable or not, we just took the left
                // node's type reference.
                typeReference = leftNode.TypeReference;
                return;
            }

            // fallback
            base.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
        }
    }
}
