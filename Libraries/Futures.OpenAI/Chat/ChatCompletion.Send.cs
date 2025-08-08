using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static TOut Send<TOut>(this Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> future, params OAI.ChatMessage[] messages)
    {
        return future.Next(messages, null);
    }

    public static Task<TOut> SendAsync<TOut>(this Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> future, params OAI.ChatMessage[] messages)
    {
        return future.NextAsync(messages, null);
    }

    public static TOut Send<TOut>(this Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> future, params string[] messages)
    {
        return future.Send(messages.Select(m => new OAI.UserChatMessage(m)).ToArray());
    }

    public static Task<TOut> SendAsync<TOut>(this Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, TOut> future, params string[] messages)
    {
        return future.SendAsync(messages.Select(m => new OAI.UserChatMessage(m)).ToArray());
    }
}