using Microsoft.Extensions.AI;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddOllamaSharpChatClient("chat");

//  builder.AddKeyedOllamaSharpChatClient("chat");
//  builder.Services.AddChatClient(sp => sp.GetRequiredKeyedService("chat"))
//                  .UseFunctionInvocation()
//                  .UseOpenTelemetry(configure: t => t.EnableSensitiveData = true)
//                  .UseLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.MapGet("/chat", async (IChatClient chatClient, string question) =>
// {
//     var response = await chatClient.CompleteAsync(question);
//     return response.Message;
// });

app.MapGet("/chat", async (IChatClient chatClient, string question) =>
{
    return GetChatResponses(chatClient, question);
});

static async IAsyncEnumerable<StreamingChatCompletionUpdate> GetChatResponses(IChatClient chatClient, string question)
{
    await foreach (var item in chatClient.CompleteStreamingAsync(question))
    {
        yield return item;
    }
}

app.MapPost("/chat", async (IChatClient chatClient, IList<ChatMessage> questions) =>
{
    return PostChatResponses(chatClient, questions);
});

static async IAsyncEnumerable<StreamingChatCompletionUpdate> PostChatResponses(IChatClient chatClient, IList<ChatMessage> questions)
{
    await foreach (var item in chatClient.CompleteStreamingAsync(questions))
    {
        yield return item;
    }
}

app.MapDefaultEndpoints();

app.Run();