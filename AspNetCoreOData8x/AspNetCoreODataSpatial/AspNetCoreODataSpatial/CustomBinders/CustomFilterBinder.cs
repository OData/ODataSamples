using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.OData.UriParser;
using Microsoft.Spatial;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Geometry = NetTopologySuite.Geometries.Geometry;

namespace GeometryWebAPI.CustomBinders
{
    public class CustomFilterBinder : FilterBinder
    {
        internal const string GeoDistanceFunctionName = "geo.distance";

        private static readonly MethodInfo distanceMethodDb = typeof(Geometry).GetMethod("Distance");

        public override Expression BindSingleValueFunctionCallNode(SingleValueFunctionCallNode node, QueryBinderContext context)
        {
            switch (node.Name)
            {
                case GeoDistanceFunctionName:
                    return BindGeoDistance(node, context);

                default:
                    return base.BindSingleValueFunctionCallNode(node, context);
            }
        }

        public Expression BindGeoDistance(SingleValueFunctionCallNode node, QueryBinderContext context)
        {
            Expression[] arguments = BindArguments(node.Parameters, context);

            string propertyName = null;

            foreach(var queryNode in node.Parameters)
            {
                if(queryNode.GetType() == typeof(SingleValuePropertyAccessNode))
                {
                    SingleValuePropertyAccessNode svpan = queryNode as SingleValuePropertyAccessNode;
                    propertyName = svpan.Property.Name;
                }
            }

            GetPointExpressions(arguments, propertyName, out MemberExpression memberExpression, out ConstantExpression constantExpression);
            var ex = Expression.Call(memberExpression, distanceMethodDb, constantExpression);

            return ex;
        }

        private static void GetPointExpressions(Expression[] expressions, string propertyName, out MemberExpression memberExpression, out ConstantExpression constantExpression)
        {
            memberExpression = null;
            constantExpression = null;

            foreach (Expression expression in expressions)
            {
                var memberExpr = expression as MemberExpression;
                var constantExpr = memberExpr.Expression as ConstantExpression;

                if (constantExpr != null)
                {
                    GeographyPoint point = GetGeographyPointFromConstantExpression(constantExpr);
                    constantExpression = Expression.Constant(CreatePoint(point.Latitude, point.Longitude));
                }
                else
                {
                    memberExpression = Expression.Property(memberExpr.Expression, propertyName);
                }
            }
        }

        private static GeographyPoint GetGeographyPointFromConstantExpression(ConstantExpression expression)
        {
            GeographyPoint point = null;
            if (expression != null)
            {
                PropertyInfo constantExpressionValuePropertyInfo = expression.Type.GetProperty("Property");
                point = constantExpressionValuePropertyInfo.GetValue(expression.Value) as GeographyPoint;
            }

            return point;
        }

        private static Point CreatePoint(double latitude, double longitude)
        {
            // 4326 is most common coordinate system used by GPS/Maps
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            // see https://docs.microsoft.com/en-us/ef/core/modeling/spatial
            // Longitude and Latitude
            var newLocation = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));

            return newLocation;
        }
    }
}
