namespace DynamicEdmModelCreation
{
	using System.Net.Http;

	public class HttpRequestMessageProvider : IHttpRequestMessageProvider
	{
		/// <inheritdoc />
		public HttpRequestMessage Request { get; set; }
	}
}