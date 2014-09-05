using System;
using System.Collections.Generic;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;

namespace ParserExt
{
    class AllInOneResolver : ODataUriResolver
    {
        private StringAsEnumResolver stringAsEnum = new StringAsEnumResolver();
        private UnqualifiedODataUriResolver unqualified = new UnqualifiedODataUriResolver();

        public override IEnumerable<IEdmOperation> ResolveBoundOperations(
            IEdmModel model,
            string identifier,
            IEdmType bindingType)
        {
            unqualified.EnableCaseInsensitive = this.EnableCaseInsensitive;
            return unqualified.ResolveBoundOperations(model, identifier, bindingType);
        }

        public override void PromoteBinaryOperandTypes(
            BinaryOperatorKind binaryOperatorKind,
            ref SingleValueNode leftNode,
            ref SingleValueNode rightNode,
            out IEdmTypeReference typeReference)
        {
            stringAsEnum.EnableCaseInsensitive = this.EnableCaseInsensitive;
            stringAsEnum.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
        }

        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
            IEdmEntityType type,
            IDictionary<string, string> namedValues,
            Func<IEdmTypeReference, string, object> convertFunc)
        {
            stringAsEnum.EnableCaseInsensitive = this.EnableCaseInsensitive;
            return stringAsEnum.ResolveKeys(type, namedValues, convertFunc);
        }

        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
            IEdmEntityType type,
            IList<string> positionalValues,
            Func<IEdmTypeReference, string, object> convertFunc)
        {
            stringAsEnum.EnableCaseInsensitive = this.EnableCaseInsensitive;
            return stringAsEnum.ResolveKeys(type, positionalValues, convertFunc);
        }

        public override IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(
            IEdmOperation operation, IDictionary<string, SingleValueNode> input)
        {
            stringAsEnum.EnableCaseInsensitive = this.EnableCaseInsensitive;
            return stringAsEnum.ResolveOperationParameters(operation, input);
        }
    }
}
