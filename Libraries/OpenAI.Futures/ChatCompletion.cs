using Futures;

namespace OpenAI.Futures;

public class ChatCompletion : Future<Chat.ChatCompletionOptions, Chat.ChatCompletion>
{
    public readonly Chat.ChatClient Client;

    public ChatCompletion(Chat.ChatClient client, CancellationToken cancellation = default) : base(cancellation)
    {
        Client = client;
        Resolver = options => client.CompleteChat([], options);
    }

    public IFuture<Chat.ChatCompletionOptions, Chat.ChatCompletion> Function(string name, string description, BinaryData parameters, bool strict = false)
    {
        var tool = Chat.ChatTool.CreateFunctionTool(
            name,
            description,
            parameters,
            strict
        );

        return new Future<Chat.ChatCompletionOptions, Chat.ChatCompletion>(
            options =>
            {
                options.Tools.Add(tool);
                return Client.CompleteChat([], options);
            }
        );
    }
}