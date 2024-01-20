namespace LocalLLMDemo;

public class LocalServerClientHandler(string url) : HttpClientHandler
{
    private readonly Uri uri = new(url);
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.RequestUri = uri;
        return base.SendAsync(request, cancellationToken);
    }
}