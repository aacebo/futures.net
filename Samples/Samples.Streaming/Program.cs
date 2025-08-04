using System.Text.Json;

using Futures;
using Futures.OpenAI.Chat;

using OpenAI;

var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var future = Providers.From.ChatStreamAsync(client.GetChatClient("gpt-3.5-turbo"));

future.Next("hi");
var message = future.Complete();
Console.WriteLine(JsonSerializer.Serialize(message));