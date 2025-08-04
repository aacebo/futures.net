using System.Text.Json;

using Futures;
using Futures.OpenAI.Chat;

using OpenAI;

var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var future = Providers.From.ChatStreamAsync(client.GetChatClient("gpt-3.5-turbo"));

foreach (var update in future.Next("hi"))
{
    Console.WriteLine(JsonSerializer.Serialize(update, new JsonSerializerOptions()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    }));
}

Console.WriteLine("done!");