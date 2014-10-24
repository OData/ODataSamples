using Microsoft.OData.Core;
using Microsoft.OData.Core.UriBuilder;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;

namespace ODataSamples.Common.Extensions
{
    public static class ExtensionMethods
    {
        public static string ToLogString(this ODataUri odataUri)
        {
            return new ODataUriBuilder(ODataUrlConventions.Default, odataUri).BuildUri().ToString();
        }

        public static string ToLogString(this ODataPath path)
        {
            return string.Join("-", path.WalkWith(PathToStringTranslator.Instance));
        }

        public static string ToLogString(this QueryNode node)
        {
            return node.Accept(new QueryNodeToStringVisitor());
        }

        public static string ToLogString(this ConstantNode node)
        {
            return string.Format("Constant:[{0},{1}]", node.TypeReference, Object2String(node.Value));
        }

        private static string Object2String(object obj)
        {
            var value = obj as ODataEnumValue;
            return value != null ? value.Value : obj.ToString();
        }
    }
}
