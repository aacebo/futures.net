using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), (IEnumerable<OAI.ChatMessage>, OAI.AssistantChatMessage, OAI.ChatCompletion)> Storage
    (
        this IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.ChatCompletion> future,
        IList<OAI.ChatMessage>? messages = null
    )
    {
        messages ??= [];

        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), (IEnumerable<OAI.ChatMessage>, OAI.AssistantChatMessage, OAI.ChatCompletion)>(args =>
        {
            var (input, options) = args;

            foreach (var message in input)
            {
                messages.Add(message);
            }

            var completion = future.Next((messages, options));
            var assistantMessage = OAI.ChatMessage.CreateAssistantMessage(completion);
            messages.Add(assistantMessage);
            return (messages, assistantMessage, completion);
        });
    }
}