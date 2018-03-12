namespace DynamicEdmModelCreation
{
	using System.Net.Http;

	public interface IHttpRequestMessageProvider
	{
		HttpRequestMessage Request { get; set; }
	}
}