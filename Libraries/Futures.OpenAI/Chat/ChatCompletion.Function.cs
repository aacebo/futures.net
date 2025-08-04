using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), TOut> Function<TOut, TParams>
    (
        this IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), TOut> future,
        string name,
        string? description = null,
        BinaryData? parameters = null,
        bool strict = false
    )
    {
        var tool = OAI.ChatTool.CreateFunctionTool(
            name,
            description,
            parameters,
            strict
        );

        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), TOut>(args =>
        {
            var options = args.Item2 ?? new();
            options.Tools.Add(tool);
            return future.Next(args.Item1, options);
        });
    }
}