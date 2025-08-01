using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

public static partial class ChatCompletionExtensions
{
    public static TOut Next<TOut>(this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> future, params Chat.ChatMessage[] messages)
    {
        return future.Next((messages, null));
    }

    public static TOut Next<TOut>(this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> future, params string[] messages)
    {
        return future.Next(messages.Select(m => new Chat.UserChatMessage(m)).ToArray());
    }

    public static TOut Next<TOut>(
        this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> future,
        IEnumerable<Chat.ChatMessage> messages,
        Chat.ChatCompletionOptions options
    )
    {
        return future.Next((messages, options));
    }
}