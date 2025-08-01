using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

public static partial class ChatCompletionExtensions
{
    public static IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> Options<TOut>(
        this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> future,
        Chat.ChatCompletionOptions options
    )
    {
        return new Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut>(args =>
        {
            return future.Next((args.Item1, args.Item2 ?? options));
        });
    }
}