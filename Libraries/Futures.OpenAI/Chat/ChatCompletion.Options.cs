using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static IStream<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> Options<TOut>
    (
        this IStream<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> future,
        OAI.ChatCompletionOptions options
    )
    {
        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut>((messages, o) =>
        {
            return future.Next(messages, o ?? options);
        }, future.Token);
    }
}