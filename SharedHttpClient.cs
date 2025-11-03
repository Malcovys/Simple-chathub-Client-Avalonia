using System.Net.Http;

namespace Client;

public static class SharedHttpClient
{
    public static HttpClient client = new()
    {
        BaseAddress = new System.Uri("http://localhost:3000")
    };
}