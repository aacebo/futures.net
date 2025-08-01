using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

public static partial class ChatCompletionExtensions
{
    public static IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), (IEnumerable<Chat.ChatMessage>, Chat.AssistantChatMessage, Chat.ChatCompletion)> Storage(
        this IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion> future,
        IList<Chat.ChatMessage>? messages = null
    )
    {
        messages ??= [];

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
}