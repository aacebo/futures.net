using Futures;
using Futures.OpenAI.Chat;

using OpenAI;

using Samples.StreamingFunctions;

var messages = new List<OpenAI.Chat.ChatMessage>();
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var future = Providers.From
    .ChatStream(client.GetChatClient("gpt-3.5-turbo"))
    .Function<Params>(
        "increment",
        "increment a given value",
        BinaryData.FromString(@"{
            ""type"": ""object"",
            ""properties"": {
                ""value"": { ""type"": ""integer"" }
            },
            ""required"": [""value""]
        }"),
        handler: (args) => (args.Value + 1).ToString()
    )
    .Storage(messages);

foreach (var update in future.Send("hi, please increment the value 203"))
{
    foreach (var chunk in update.ContentUpdate)
    {
        Console.Write(chunk.Text);
    }
}

Console.Write(Environment.NewLine);

foreach (var update in future.Send("hi, please increment the value 500"))
{
    foreach (var chunk in update.ContentUpdate)
    {
        Console.Write(chunk.Text);
    }
}

Console.Write(Environment.NewLine);
Console.WriteLine($"done! {messages.Count}");