using Microsoft.Extensions.AI;

namespace AspireOllamaTestNet9.Web;

public class ChatApiClient(HttpClient httpClient)
{
    public async Task<string> GetChatCompletionAsync(string question, CancellationToken cancellationToken = default)
    {
        var requestUri = $"/chat?question={question}";
        var response = 
            await httpClient.GetFromJsonAsync<Response>(requestUri, cancellationToken);

        return response?.Value ?? "";
    }

    public async IAsyncEnumerable<StreamingChatCompletionUpdate> GetChatCompletionStreamingAsync(string question, CancellationToken cancellationToken = default)
    {
        var requestUri = $"/chat?question={question}";
        await foreach (var item in httpClient.GetFromJsonAsAsyncEnumerable<StreamingChatCompletionUpdate>(requestUri, cancellationToken))
        {
            // We're streaming the response, so we get each message as it arrives
            yield return item;
        }
    }
}

public record Response(string Value);
