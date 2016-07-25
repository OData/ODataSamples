using System;
using System.Collections.ObjectModel;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriBuilder;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using ODataSamples.Common.Model;

namespace ODataSamples.UriBuilder
{
    class Program
    {
        private static readonly Uri V4Root = new Uri("http://services.odata.org/V4/OData/OData.svc/");
        private static readonly Uri TripPinRoot = new Uri("http://services.odata.org/V4/TripPinService/");
        private static readonly V4Model V4Model = new V4Model();
        private static readonly TripPinModel TripPinModel = new TripPinModel();
        private static readonly SchoolModel SchoolModel = new SchoolModel();

        static void Main(string[] args)
        {
            BuildOrderBy();
            BuildFilterWithBinaryOperator();
            BuildFilterWithNestedAny();
            BuildNavigationWithAny();
        }

        private static void BuildOrderBy()
        {
            var productTypeRef = new EdmEntityTypeReference(V4Model.Product, false);
            var supplierProperty = (IEdmNavigationProperty)V4Model.Product.FindProperty("Supplier");
            var nameProperty = V4Model.Supplier.FindProperty("Name");

            var topIt = new EntityRangeVariable("$it", productTypeRef, V4Model.ProductsSet);
            var topItRef = new EntityRangeVariableReferenceNode("$it", topIt);
            var supplierNavNode = new SingleNavigationNode(supplierProperty, topItRef);
            var nameNode = new SingleValuePropertyAccessNode(supplierNavNode, nameProperty);

            var orderby = new OrderByClause(null, nameNode, OrderByDirection.Ascending, topIt);
            var odataUri = new ODataUri
            {
                Path = new ODataPath(new EntitySetSegment(V4Model.ProductsSet)),
                ServiceRoot = V4Root,
                OrderBy = orderby
            };
            var builder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Console.WriteLine(builder.BuildUri());
            // http://services.odata.org/V4/OData/OData.svc/Products?$orderby=Supplier%2FName
        }

        private static void BuildFilterWithBinaryOperator()
        {
            var personTypeRef = new EdmEntityTypeReference(TripPinModel.Person, false);
            var friendsProp = (IEdmNavigationProperty)TripPinModel.Person.FindProperty("Friends");
            var firstNameProp = TripPinModel.Person.FindProperty("FirstName");
            var lastNameProp = TripPinModel.Person.FindProperty("LastName");

            var topIt = new EntityRangeVariable("$it", personTypeRef, TripPinModel.PeopleSet);
            var topItRef = new EntityRangeVariableReferenceNode("$it", topIt);
            var friendsNavNode = new CollectionNavigationNode(friendsProp, topItRef);
            var e0 = new EntityRangeVariable("e0", personTypeRef, friendsNavNode);
            var e0Ref = new EntityRangeVariableReferenceNode("e0", e0);
            var fun1 = new SingleValueFunctionCallNode(
                "startswith",
                new QueryNode[]
                {
                    new SingleValuePropertyAccessNode(e0Ref, firstNameProp),
                    new ConstantNode("var1", "'var1'")
                },
                EdmCoreModel.Instance.GetBoolean(false));

            var friendsNavNode2 = new CollectionNavigationNode(friendsProp, e0Ref);
            var e1 = new EntityRangeVariable("e1", personTypeRef, friendsNavNode2);
            var e1Ref = new EntityRangeVariableReferenceNode("e1", e1);
            var fun2 = new SingleValueFunctionCallNode(
                "contains",
                new QueryNode[]
                {
                    new SingleValuePropertyAccessNode(e1Ref, lastNameProp),
                    new ConstantNode("var2", "'var2'")
                },
                EdmCoreModel.Instance.GetBoolean(false));

            // Actually $it also needed, but would not be used in UriBuilder, so omit it here.
            var any2 = new AnyNode(new Collection<RangeVariable> { e1 }, e1)
            {
                Body = fun2,
                Source = friendsNavNode2
            };

            var any1 = new AnyNode(new Collection<RangeVariable> { e0 }, e0)
            {
                Body = new BinaryOperatorNode(BinaryOperatorKind.And, fun1, any2),
                Source = friendsNavNode
            };

            var odataUri = new ODataUri
            {
                Path = new ODataPath(new EntitySetSegment(TripPinModel.PeopleSet)),
                ServiceRoot = TripPinRoot,
                Filter = new FilterClause(any1, topIt)
            };
            var builder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Console.WriteLine(builder.BuildUri());

            // http://services.odata.org/V4/TripPinService/People?$filter=Friends%2Fany(
            // e0:startswith(e0%2FFirstName%2C'var1') and e0%2FFriends%2Fany(e1:contains(e1%2FLastName%2C'var2')))
        }

        private static void BuildFilterWithNestedAny()
        {
            var personTypeRef = new EdmEntityTypeReference(TripPinModel.Person, false);
            var tripTypeRef = new EdmEntityTypeReference(TripPinModel.Trip, false);

            var friendsProp = (IEdmNavigationProperty)TripPinModel.Person.FindProperty("Friends");
            var tripsProp = (IEdmNavigationProperty)TripPinModel.Person.FindProperty("Trips");
            var budgetProp = TripPinModel.Trip.FindProperty("Budget");

            var topIt = new EntityRangeVariable("$it", personTypeRef, TripPinModel.PeopleSet);
            var topItRef = new EntityRangeVariableReferenceNode("$it", topIt);

            var friendsNavNode0 = new CollectionNavigationNode(friendsProp, topItRef);
            var e0 = new EntityRangeVariable("e0", personTypeRef, friendsNavNode0);
            var e0Ref = new EntityRangeVariableReferenceNode("e0", e0);

            var friendsNavNode1 = new CollectionNavigationNode(friendsProp, e0Ref);
            var e1 = new EntityRangeVariable("e1", personTypeRef, friendsNavNode1);
            var e1Ref = new EntityRangeVariableReferenceNode("e1", e1);

            var tripNavNode = new CollectionNavigationNode(tripsProp, e1Ref);
            var e2 = new EntityRangeVariable("e2", tripTypeRef, friendsNavNode1);
            var e2Ref = new EntityRangeVariableReferenceNode("e2", e2);

            var bugetNode = new SingleValuePropertyAccessNode(e2Ref, budgetProp);

            var gt = new BinaryOperatorNode(
                BinaryOperatorKind.GreaterThan,
                bugetNode,
                new ConstantNode(1200, "1200"));

            var any2 = new AnyNode(new Collection<RangeVariable> { e2 }, e2) { Body = gt, Source = tripNavNode };
            var any1 = new AnyNode(new Collection<RangeVariable> { e1 }, e1) { Body = any2, Source = friendsNavNode1 };
            var any0 = new AnyNode(new Collection<RangeVariable> { e0 }, e0) { Body = any1, Source = friendsNavNode0 };


            var odataUri = new ODataUri
            {
                Path = new ODataPath(new EntitySetSegment(TripPinModel.PeopleSet)),
                ServiceRoot = TripPinRoot,
                Filter = new FilterClause(any0, topIt)
            };
            var builder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Console.WriteLine(builder.BuildUri());
            // http://services.odata.org/V4/TripPinService/People?$filter=Friends%2Fany(e0:e0%2FFriends%2Fany(e1:e1%2FTrips%2Fany(e2:e2%2FBudget gt 1200)))
        }

        private static void BuildNavigationWithAny()
        {
            // Request URI
            var requestUri = new Uri("Students?$filter=StudentId gt 100", UriKind.Relative);
            var requestODataUri = new ODataUriParser(SchoolModel.Model, requestUri).ParseUri();
            var requestFilterExpression = requestODataUri.Filter.Expression;

            // URI that contains additional condition
            var conditionUri = new Uri("Students?$filter=Courses/any(c:c/Teacher/Education/Degrees/any(d:d/Category eq 'English Literature'))", UriKind.Relative);
            var conditionODataUri = new ODataUriParser(SchoolModel.Model, conditionUri).ParseUri();
            var conditionExpression = conditionODataUri.Filter.Expression;

            // New $filter expression
            var newFilterExpression = new BinaryOperatorNode(BinaryOperatorKind.And, requestFilterExpression, conditionExpression);
            requestODataUri.Filter = new FilterClause(newFilterExpression, requestODataUri.Filter.RangeVariable);
            var builder = new ODataUriBuilder(ODataUrlConventions.Default, requestODataUri);
            Console.WriteLine(builder.BuildUri());

            // http://host/Students?$filter=StudentId%20gt%20100%20and%20Courses%2Fany%28c%3Ac%2FTeacher%2FEducation%2FDegrees%2Fany%28d%3Ad%2FCategory%20eq%20%27English%20Literature%27%29%29
            // http://host/Students?$filter=StudentId gt 100 and Courses/any(c:c/Teacher/Education/Degrees/any(d:d/Category eq 'English Literature'))
        }
    }
}
