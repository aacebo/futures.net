using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static TOut Next<TOut>(this IFuture<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> future, params OAI.ChatMessage[] messages)
    {
        return future.Next(messages, null);
    }

    public static TOut Next<TOut>(this IFuture<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> future, params string[] messages)
    {
        return future.Next(messages.Select(m => new OAI.UserChatMessage(m)).ToArray());
    }

    public static TOut Next<TOut>
    (
        this IFuture<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> future,
        IEnumerable<OAI.ChatMessage> messages,
        OAI.ChatCompletionOptions options
    )
    {
        return future.Next(messages, options);
    }
}