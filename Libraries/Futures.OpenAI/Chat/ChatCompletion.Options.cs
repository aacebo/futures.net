using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), TOut> Options<TOut>(
        this IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), TOut> future,
        OAI.ChatCompletionOptions options
    )
    {
        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), TOut>(args =>
        {
            return future.Next((args.Item1, args.Item2 ?? options));
        });
    }
}