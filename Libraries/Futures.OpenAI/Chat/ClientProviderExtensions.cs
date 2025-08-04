using System.ClientModel;

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

    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), CollectionResult<OAI.StreamingChatCompletionUpdate>> ChatStream(this ClientProvider _, OAI.ChatClient client, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), CollectionResult<OAI.StreamingChatCompletionUpdate>>(args =>
        {
            return client.CompleteChatStreaming(args.Item1, args.Item2 ?? options, cancellation);
        }, cancellation);
    }

    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), AsyncCollectionResult<OAI.StreamingChatCompletionUpdate>> ChatStreamAsync(this ClientProvider _, OAI.ChatClient client, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), AsyncCollectionResult<OAI.StreamingChatCompletionUpdate>>(args =>
        {
            return client.CompleteChatStreamingAsync(args.Item1, args.Item2 ?? options, cancellation);
        }, cancellation);
    }
}