using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

public class ChatCompletion : Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>, IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>
{
    public readonly IFuture<List<Chat.ChatMessage>> Messages;

    public ChatCompletion(Chat.ChatClient client, Chat.ChatCompletionOptions? options = null, CancellationToken cancellation = default) : base(cancellation)
    {
        Messages = new Future<List<Chat.ChatMessage>>([]);
        Resolver = (args) => client.CompleteChat(args.Item1, args.Item2 ?? options).Value;
    }
}