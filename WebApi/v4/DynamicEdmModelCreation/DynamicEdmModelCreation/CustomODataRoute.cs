namespace DynamicEdmModelCreation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Http;
	using System.Web.Http;
	using System.Web.Http.Routing;
	using System.Web.OData.Routing;

	public class CustomODataRoute : ODataRoute
	{
		private static readonly string escapedHashMark = Uri.HexEscape('#');
		private static readonly string escapedQuestionMark = Uri.HexEscape('?');

		private readonly bool canGenerateDirectLink;

		public CustomODataRoute(string routePrefix, ODataPathRouteConstraint pathConstraint)
			: base(routePrefix, pathConstraint)
		{
			this.canGenerateDirectLink = routePrefix != null && this.RoutePrefix.IndexOf('{') == -1;
		}

		public override IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values)
		{
			if (values == null || !values.Keys.Contains(HttpRouteKey, StringComparer.OrdinalIgnoreCase))
			{
				return null;
			}

			if (!values.TryGetValue(ODataRouteConstants.ODataPath, out object odataPathValue))
			{
				return null;
			}

			if (odataPathValue is string odataPath)
			{
				return this.GenerateLinkDirectly(request, odataPath) ?? base.GetVirtualPath(request, values);
			}

			return null;
		}

		private HttpVirtualPathData GenerateLinkDirectly(HttpRequestMessage request, string odataPath)
		{
			HttpConfiguration configuration = request.GetConfiguration();
			if (configuration == null || !this.canGenerateDirectLink)
			{
				return null;
			}

			string dataSource = request.Properties[Constants.ODataDataSource] as string;
			string link = CombinePathSegments(this.RoutePrefix, dataSource);
			link = CombinePathSegments(link, odataPath);
			link = UriEncode(link);

			return new HttpVirtualPathData(this, link);
		}

		private static string CombinePathSegments(string routePrefix, string odataPath)
		{
			return string.IsNullOrEmpty(routePrefix)
				? odataPath
				: (string.IsNullOrEmpty(odataPath) ? routePrefix : routePrefix + '/' + odataPath);
		}

		private static string UriEncode(string str)
		{
			string escape = Uri.EscapeUriString(str);
			escape = escape.Replace("#", escapedHashMark);
			escape = escape.Replace("?", escapedQuestionMark);
			return escape;
		}
	}
}