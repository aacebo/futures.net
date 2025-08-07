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

        return new Future<IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?, OAI.ChatCompletion>((messages, opts) =>
        {
            var options = opts ?? new();
            options.Tools.Add(tool);
            var completion = future.Next(messages, options);

            if (handler is not null)
            {
                var message = OAI.ChatMessage.CreateAssistantMessage(completion);
                var calls = message.ToolCalls.Where(call => call.FunctionName == name);

                while (calls.Count() > 0)
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
}