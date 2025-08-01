using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

public class ChatCompletion : Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>, IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>
{
    public ChatCompletion(Chat.ChatClient client, CancellationToken cancellation = default) : base(cancellation)
    {
        Resolver = (args) =>
        {
            var (messages, options) = args;
            var res = client.CompleteChatAsync(args.Item1, args.Item2 ?? options, Token).GetAwaiter().GetResult();
            return res.Value;
        };
    }

    ~ChatCompletion()
    {
        Dispose();
    }
}