using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static class ClientProviderExtensions
{
    public static Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion> Chat(this ClientProvider _, OAI.ChatClient client, CancellationToken cancellation = default)
    {
        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion>((messages, options) =>
        {
            return client.CompleteChat(messages, options, cancellation).Value;
        }, cancellation);
    }

    public static Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>> ChatStream(this ClientProvider _, OAI.ChatClient client, CancellationToken cancellation = default)
    {
        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>>((messages, options) =>
        {
            var res = client.CompleteChatStreaming(messages, options, cancellation);
            return res.ToFuture();
        }, cancellation);
    }
}