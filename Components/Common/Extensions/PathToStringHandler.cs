using System.Linq;
using System.Text;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Visitors;
using Microsoft.OData.Edm;

namespace ODataSamples.Common.Extensions
{
    public class PathToStringTranslator : PathSegmentTranslator<string>
    {
        public static PathToStringTranslator Instance = new PathToStringTranslator();

        private PathToStringTranslator()
        {
        }

        public override string Translate(TypeSegment segment)
        {
            return string.Format("{{Type:[{0}]}}", segment.EdmType.FullTypeName());
        }

        public override string Translate(NavigationPropertySegment segment)
        {
            return string.Format("{{NavProp:[{0}]}}", segment.NavigationProperty.Name);
        }

        public override string Translate(EntitySetSegment segment)
        {
            return string.Format("{{EntitySet:[{0}]}}", segment.EntitySet.Name);
        }

        public override string Translate(KeySegment segment)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var keyValuePair in segment.Keys)
            {
                builder.AppendFormat("{0}={1},", keyValuePair.Key, keyValuePair.Value);
            }

            return string.Format("{{Keys:[{0}]}}", builder.ToString());
        }

        public override string Translate(PropertySegment segment)
        {
            return string.Format("{{Prop:[{0}]}}", segment.Property.Name);
        }

    
        public override string Translate(OperationSegment segment)
        {
            return string.Format(
                "{{Operation:[{0}]{1}}}",
                segment.Operations.First().FullName(),
                string.Join(",", segment.Parameters.Select(Translate)));
        }

        public override string Translate(OperationImportSegment segment)
        {
            return string.Format("{{OperImp:[{0}]}}", segment.OperationImports.First().Name);
        }

        public override string Translate(OpenPropertySegment segment)
        {
            return string.Format("{{OpenProp:[{0}]}}", segment.PropertyName);
        }

        private string Translate(OperationSegmentParameter parameter)
        {
            string val = parameter.Value is ConstantNode
                ? ((ConstantNode)parameter.Value).ToLogString()
                : parameter.Value.ToString();

            return string.Format("{0}={1}", parameter.Name, val);
        }
    }
}
