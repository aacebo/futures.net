using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static class ClientProviderExtensions
{
    public static IFuture<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion> Chat(this ClientProvider _, OAI.ChatClient client, CancellationToken cancellation = default)
    {
        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion>((messages, options) =>
        {
            return client.CompleteChat(messages, options, cancellation).Value;
        }, cancellation);
    }

    public static IFuture<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, IFuture<OAI.StreamingChatCompletionUpdate>> ChatStream(this ClientProvider _, OAI.ChatClient client, CancellationToken cancellation = default)
    {
        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, IFuture<OAI.StreamingChatCompletionUpdate>>((messages, options) =>
        {
            var res = client.CompleteChatStreamingAsync(messages, options, cancellation);
            return Future<OAI.StreamingChatCompletionUpdate>.From(res);
        }, cancellation);
    }
}