using Futures;
using Futures.OpenAI.Chat;
using Futures.Operators;

using OpenAI;

var messages = new List<OpenAI.Chat.ChatMessage>();
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var future = Providers.From
    .ChatStream(client.GetChatClient("gpt-3.5-turbo"))
    .Storage(messages)
    .Map(v => v.Select(u => u.ContentUpdate.FirstOrDefault()?.Text));

foreach (var update in future.Send("hi, how are you today? Tell me a long story"))
{
    Console.Write(update);
}

Console.Write(Environment.NewLine);
Console.WriteLine($"done! {messages.Count}");