using Futures;
using Futures.OpenAI.Chat;

using OpenAI;

var messages = new List<OpenAI.Chat.ChatMessage>();
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var future = Providers.From
    .ChatStream(client.GetChatClient("gpt-3.5-turbo"))
    .Storage(messages);

foreach (var update in future.Send("hi, how are you today?"))
{
    foreach (var chunk in update.ContentUpdate)
    {
        Console.Write(chunk.Text);
    }
}

Console.Write(Environment.NewLine);
Console.WriteLine($"done! {messages.Count}");