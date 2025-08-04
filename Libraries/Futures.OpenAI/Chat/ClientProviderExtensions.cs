using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static class ClientProviderExtensions
{
    public static IFuture<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion> Chat(this ClientProvider _, OAI.ChatClient client, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion>((messages, opts) =>
        {
            var res = client.CompleteChat(messages, opts ?? options, cancellation);
            return res.Value;
        }, cancellation);
    }

    public static IFuture<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, IFuture<OAI.StreamingChatCompletionUpdate>> ChatStream(this ClientProvider _, OAI.ChatClient client, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, IFuture<OAI.StreamingChatCompletionUpdate>>((messages, opts) =>
        {
            var res = client.CompleteChatStreaming(messages, opts ?? options, cancellation);
            return Future<OAI.StreamingChatCompletionUpdate>.From(res);
        }, cancellation);
    }

    public static IFuture<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, IFuture<OAI.StreamingChatCompletionUpdate>> ChatStreamAsync(this ClientProvider _, OAI.ChatClient client, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, IFuture<OAI.StreamingChatCompletionUpdate>>((messages, opts) =>
        {
            var res = client.CompleteChatStreamingAsync(messages, opts ?? options, cancellation);
            return Future<OAI.StreamingChatCompletionUpdate>.From(res);
        }, cancellation);
    }
}