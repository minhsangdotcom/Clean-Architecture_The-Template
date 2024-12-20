using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Contracts.Extensions;

namespace Application.SubcutaneousTests.Extensions;

public static class ClientExtension
{
    public static async Task<T?> ToResponse<T>(this HttpResponseMessage responseMessage)
    {
        return await responseMessage.Content.ReadFromJsonAsync<T>();
    }

    public static async Task<HttpResponseMessage> CreateRequestAsync(
        this HttpClient client,
        string uriString,
        HttpMethod method,
        object payload,
        string? contentType = null,
        string? token = null
    )
    {
        Uri uri = new(uriString);
        StringContent content =
            new(
                SerializerExtension.Serialize(payload).StringJson,
                Encoding.UTF8,
                contentType ?? "application/json"
            );
        using HttpRequestMessage httpRequest =
            new()
            {
                Method = method,
                RequestUri = uri,
                Content = content,
            };
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );
        }
        return await client.SendAsync(httpRequest);
    }
}
