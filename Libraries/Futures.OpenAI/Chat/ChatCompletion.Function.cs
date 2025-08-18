using System.Text.Json;

using Futures.Operators;

using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public static partial class ChatCompletionExtensions
{
    public static Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion> Function<TParams>
    (
        this Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion> future,
        string name,
        string? description = null,
        BinaryData? parameters = null,
        Func<TParams, string>? handler = null,
        bool strict = false
    )
    {
        var tool = OAI.ChatTool.CreateFunctionTool(
            name,
            description,
            parameters,
            strict
        );

        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion>((messages, options) =>
        {
            options ??= new();
            options.Tools.Add(tool);
            var completion = future.Next(messages, options);

            if (handler is not null)
            {
                var message = OAI.ChatMessage.CreateAssistantMessage(completion);
                var calls = message.ToolCalls.Where(call => call.FunctionName == name);

                while (calls.Any())
                {
                    messages = messages.Append(message);

                    foreach (var call in calls)
                    {
                        var @params = JsonSerializer.Deserialize<TParams>(call.FunctionArguments) ?? throw new ArgumentException("could not deserialize params");
                        var res = handler(@params);
                        messages = messages.Append(OAI.ChatMessage.CreateToolMessage(call.Id, res));
                    }

                    completion = future.Next(messages, options);
                    message = OAI.ChatMessage.CreateAssistantMessage(completion);
                    calls = message.ToolCalls.Where(call => call.FunctionName == name);
                }
            }

            return completion;
        });
    }

    public static Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>> Function<TParams>
    (
        this Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>> future,
        string name,
        string? description = null,
        BinaryData? parameters = null,
        Func<TParams, string>? handler = null,
        bool strict = false
    )
    {
        var tool = OAI.ChatTool.CreateFunctionTool(
            name,
            description,
            parameters,
            strict
        );

        Future<OAI.StreamingChatCompletionUpdate> Send(IEnumerable<OAI.ChatMessage> messages, OAI.ChatCompletionOptions? options)
        {
            options ??= new();
            options.Tools.Add(tool);

            var updates = future.Next(messages, options);
            var builder = new Streaming.CompletionBuilder();

            return updates.MergeMap(update =>
            {
                var next = Future.FromValue(update);
                Console.WriteLine($"stream update: {update.CompletionId}");
                builder.Append(update);

                if (handler is null || !update.ToolCallUpdates.Any())
                {
                    return next;
                }

                if (update.FinishReason == OAI.ChatFinishReason.ToolCalls)
                {
                    var message = OAI.ChatMessage.CreateAssistantMessage(builder.Build());
                    messages = messages.Append(message);

                    while (message.ToolCalls.Any())
                    {
                        foreach (var call in message.ToolCalls)
                        {
                            Console.WriteLine($"tool call: {call.Id}");
                            var @params = JsonSerializer.Deserialize<TParams>(call.FunctionArguments) ?? throw new ArgumentException("could not deserialize params");
                            var res = handler(@params);
                            messages = messages.Append(OAI.ChatMessage.CreateToolMessage(call.Id, res));
                        }

                        var updates = Send(messages, options);
                        message = OAI.ChatMessage.CreateAssistantMessage(Streaming.CompletionBuilder.From(updates).Build());
                        messages = messages.Append(message);
                    }

                    return Send(messages, options);
                }

                return next;
            }).Fork();
        }

        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>>(Send);
    }
}