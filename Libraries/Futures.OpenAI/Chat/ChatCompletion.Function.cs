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
        }, future.Token);
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

        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, Future<OAI.StreamingChatCompletionUpdate>>((messages, options) =>
        {
            options ??= new();
            options.Tools.Add(tool);

            var updates = future.Next(messages, options);
            var builder = new Streaming.CompletionBuilder();

            return updates.Pipe(update =>
            {
                builder.Append(update);

                if (handler is not null && update.FinishReason == OAI.ChatFinishReason.ToolCalls)
                {
                    var calls = builder.ToolCalls.Build();
                    var message = OAI.ChatMessage.CreateAssistantMessage(calls);

                    messages = messages.Append(message);

                    foreach (var call in calls)
                    {
                        var @params = JsonSerializer.Deserialize<TParams>(call.FunctionArguments) ?? throw new ArgumentException("could not deserialize params");
                        var res = handler(@params);
                        messages = messages.Append(OAI.ChatMessage.CreateToolMessage(call.Id, res));
                    }

                    return future.Next(messages, options);
                }

                return Future<OAI.StreamingChatCompletionUpdate>.From(update);
            });
        }, future.Token);
    }
}