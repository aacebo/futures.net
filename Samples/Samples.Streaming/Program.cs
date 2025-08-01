using System.Text.Json;

using Futures.OpenAI.Chat;

using OpenAI;

var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var future = new ChatCompletion(client.GetChatClient("gpt-3.5-turbo"))
    .Stream(chunk => Console.WriteLine("hit..."));

future.Next("hi");
var message = future.Complete();
Console.WriteLine(JsonSerializer.Serialize(message));