using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

public class ChatCompletion : Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>, IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>
{
    public ChatCompletion(Chat.ChatClient client, Chat.ChatCompletionOptions? options = null, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (args) => client.CompleteChat(args.Item1, args.Item2 ?? options).Value;
    }
}

public static partial class IFutureExtensions
{
    public static IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion> Function(this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion> future, string name, string description, BinaryData parameters, bool strict = false)
    {
        var tool = Chat.ChatTool.CreateFunctionTool(
            name,
            description,
            parameters,
            strict
        );

        return new Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>(args =>
        {
            var options = args.Item2 ?? new();
            options.Tools.Add(tool);
            return future.Next(args.Item1, options);
        });
    }

    public static IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion> Options(this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion> future, Chat.ChatCompletionOptions options)
    {
        return new Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>(args =>
        {
            return future.Next((args.Item1, args.Item2 ?? options));
        });
    }

    public static TOut Next<TOut>(this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> future, params Chat.ChatMessage[] messages)
    {
        return future.Next((messages, null));
    }

    public static TOut Next<TOut>(this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> future, params string[] messages)
    {
        return future.Next(messages.Select(m => new Chat.UserChatMessage(m)).ToArray());
    }

    public static TOut Next<TOut>(this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), TOut> future, IEnumerable<Chat.ChatMessage> messages, Chat.ChatCompletionOptions options)
    {
        return future.Next((messages, options));
    }
}