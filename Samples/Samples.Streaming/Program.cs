using System.Text.Json;

using Futures;
using Futures.OpenAI.Chat;

using OpenAI;

List<OpenAI.Chat.ChatMessage> messages = [];
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var future = Providers.From.ChatStreamAsync(client.GetChatClient("gpt-3.5-turbo")).Storage(messages);

foreach (var _ in future.Next("hi"))
{
}

Console.WriteLine($"done! {messages.Count}");