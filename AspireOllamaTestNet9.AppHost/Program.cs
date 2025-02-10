var builder = DistributedApplication.CreateBuilder(args);

var ollama =
        builder.AddOllama("ollama")
               .WithDataVolume()
               .WithOpenWebUI();

var chat = ollama.AddModel("chat", "llama3.2");

var apiService = builder.AddProject<Projects.AspireOllamaTestNet9_ApiService>("apiservice")
    .WithReference(chat)
    .WaitFor(chat);

builder.AddProject<Projects.AspireOllamaTestNet9_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

 builder.AddProject<Projects.AspireOllamaTestNet9_ConsoleApp>("consoleapp")
     .WithReference(chat)
     .WaitFor(chat);

builder.Build().Run();
