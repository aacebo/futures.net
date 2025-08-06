using System.ClientModel;

using Futures.Operators;

using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion> Storage
    (
        this Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion> future,
        IList<OAI.ChatMessage>? messages = null
    )
    {
        messages ??= [];

        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion>((input, options) =>
        {
            foreach (var message in input)
            {
                messages.Add(message);
            }

            var completion = future.Next(messages, options);
            var assistantMessage = OAI.ChatMessage.CreateAssistantMessage(completion);
            messages.Add(assistantMessage);
            return completion;
        });
    }

    public static Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>> Storage
    (
        this Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>> future,
        IList<OAI.ChatMessage>? messages = null
    )
    {
        messages ??= [];

        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>>((input, options) =>
        {
            foreach (var message in input)
            {
                messages.Add(message);
            }

            var stream = future.Next(messages, options);
            var builder = new Streaming.CompletionBuilder();

            stream.Subscribe(new()
            {
                OnNext = value => builder.Append(value),
                OnComplete = () => messages.Add(OAI.ChatMessage.CreateAssistantMessage(builder.Build()))
            });

            return stream;
        });
    }
}