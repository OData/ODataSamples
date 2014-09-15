using System;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Visitors;

namespace ODataSamples.UriParser.ParserExt
{
    class QueryNodeToStringVisitor : QueryNodeVisitor<string>
    {
        private const string Ident = "  ";

        private string _currentIdent = string.Empty;

        public override string Visit(SingleValueFunctionCallNode nodeIn)
        {
            return WrapWithIdent(
                string.Format("Func:[{0}]", nodeIn.Name),
                () => string.Join(Environment.NewLine, nodeIn.Parameters.Select(_ => _.Accept(this))));
        }

        public override string Visit(NamedFunctionParameterNode nodeIn)
        {
            return WrapWithIdent(
                string.Format("Parameter:[{0}]", nodeIn.Name),
                () => nodeIn.Value.Accept(this));
        }

        public override string Visit(ConstantNode nodeIn)
        {
            return WrapWithIdent(nodeIn.ToLogString());
        }

        public override string Visit(BinaryOperatorNode nodeIn)
        {
            return WrapWithIdent(
                string.Format("BinaryNode:[{0}]", nodeIn.OperatorKind),
                () => nodeIn.Left.Accept(this) + Environment.NewLine + nodeIn.Right.Accept(this));
        }

        public override string Visit(SingleValuePropertyAccessNode nodeIn)
        {
            return WrapWithIdent(string.Format("Property:[{0}]", nodeIn.Property.Name));
        }

        private string WrapWithIdent(string current, Func<string> inner = null)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(_currentIdent);
            builder.Append("{");
            builder.Append(current);
            if (inner != null)
            {
                builder.Append(Environment.NewLine);
                string saveIdent = _currentIdent;
                _currentIdent = _currentIdent + Ident;
                builder.Append(inner());
                _currentIdent = saveIdent;
                builder.Append(Environment.NewLine);
                builder.Append(_currentIdent);
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
