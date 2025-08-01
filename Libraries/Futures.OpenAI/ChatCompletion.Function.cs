using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

public static partial class ChatCompletionExtensions
{
    public static IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> Function<TOut>(
        this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> future,
        string name,
        string description,
        BinaryData parameters,
        bool strict = false
    )
    {
        var tool = Chat.ChatTool.CreateFunctionTool(
            name,
            description,
            parameters,
            strict
        );

        return new Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut>(args =>
        {
            var options = args.Item2 ?? new();
            options.Tools.Add(tool);
            return future.Next(args.Item1, options);
        });
    }
}