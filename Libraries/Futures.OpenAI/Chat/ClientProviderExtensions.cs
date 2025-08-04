using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static class ClientProviderExtensions
{
    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.ChatCompletion> Chat(this ClientProvider _, OAI.ChatClient client, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.ChatCompletion>(args =>
        {
            var res = client.CompleteChat(args.Item1, args.Item2 ?? options, cancellation);
            return res.Value;
        }, cancellation);
    }

    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), IFuture<OAI.StreamingChatCompletionUpdate, OAI.StreamingChatCompletionUpdate>> ChatStream(this ClientProvider _, OAI.ChatClient client, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), IFuture<OAI.StreamingChatCompletionUpdate, OAI.StreamingChatCompletionUpdate>>(args =>
        {
            var res = client.CompleteChatStreaming(args.Item1, args.Item2 ?? options, cancellation);
            return Future<OAI.StreamingChatCompletionUpdate>.From(res);
        }, cancellation);
    }

    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), IFuture<OAI.StreamingChatCompletionUpdate, OAI.StreamingChatCompletionUpdate>> ChatStreamAsync(this ClientProvider _, OAI.ChatClient client, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), IFuture<OAI.StreamingChatCompletionUpdate, OAI.StreamingChatCompletionUpdate>>(args =>
        {
            var res = client.CompleteChatStreamingAsync(args.Item1, args.Item2 ?? options, cancellation);
            return Future<OAI.StreamingChatCompletionUpdate>.From(res);
        }, cancellation);
    }
}