using System.ClientModel;

using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.ChatCompletion> Storage
    (
        this IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.ChatCompletion> future,
        IList<OAI.ChatMessage>? messages = null
    )
    {
        messages ??= [];

        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.ChatCompletion>(args =>
        {
            var (input, options) = args;

            foreach (var message in input)
            {
                messages.Add(message);
            }

            var completion = future.Next((messages, options));
            var assistantMessage = OAI.ChatMessage.CreateAssistantMessage(completion);
            messages.Add(assistantMessage);
            return completion;
        });
    }

    public static IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), IFuture<OAI.StreamingChatCompletionUpdate>> Storage
    (
        this IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), CollectionResult<OAI.StreamingChatCompletionUpdate>> future,
        IList<OAI.ChatMessage>? messages = null
    )
    {
        messages ??= [];

        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), IFuture<OAI.StreamingChatCompletionUpdate>>(args =>
        {
            var (input, options) = args;

            foreach (var message in input)
            {
                messages.Add(message);
            }

            var stream = future.Next((messages, options));
            var updates = new Future<OAI.StreamingChatCompletionUpdate>(future.Token);

            _ = Task.Run(() =>
            {
                var builder = new Streaming.CompletionBuilder();

                foreach (var update in stream)
                {
                    updates.Next(update);
                    builder.Append(update);
                }

                var completion = builder.Build();
                var message = OAI.ChatMessage.CreateAssistantMessage(completion);
                messages.Add(message);
            });

            return updates;
        });
    }
}