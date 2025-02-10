// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;


var builder = Host.CreateApplicationBuilder();
builder.AddServiceDefaults();

//builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:62303"), "llama3.2"));
builder.AddOllamaSharpChatClient("chat");


var app = builder.Build();

var chatClient = app.Services.GetRequiredService<IChatClient>();

//var chatCompletion = await chatClient.CompleteAsync("What is .NET? Reply in 50 words max.");

//Console.WriteLine(chatCompletion.Message.Text);
var chatHistory = new List<ChatMessage>();

while (true)
{
   Console.WriteLine("Enter your prompt:");
   var userPrompt = Console.ReadLine();
   chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

   Console.WriteLine("Response from AI:");
   var chatResponse = "";
   await foreach (var item in chatClient.CompleteStreamingAsync(chatHistory))
   {
       // We're streaming the response, so we get each message as it arrives
       Console.Write(item.Text);
       chatResponse += item.Text;
   }
   chatHistory.Add(new ChatMessage(ChatRole.Assistant, chatResponse));
   Console.WriteLine();
}