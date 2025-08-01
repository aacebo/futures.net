using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

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

    public static IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), (IEnumerable<Chat.ChatMessage>, Chat.AssistantChatMessage, Chat.ChatCompletion)> AutoMessageStorage(this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion> future)
    {
        List<Chat.ChatMessage> messages = [];

        return new Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), (IEnumerable<Chat.ChatMessage>, Chat.AssistantChatMessage, Chat.ChatCompletion)>(args =>
        {
            var (input, options) = args;

            foreach (var message in input)
            {
                messages.Add(message);
            }

            var completion = future.Next((messages, options));
            var assistantMessage = Chat.ChatMessage.CreateAssistantMessage(completion);
            messages.Add(assistantMessage);
            return (messages, assistantMessage, completion);
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