using Blaztore;
using Blaztore.Examples.Wasm;
using Blaztore.Examples.Wasm.Components;
using Blaztore.Examples.Wasm.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlaztore(configuration => configuration.RegisterServicesFromAssemblyContaining<TodoListState>());
builder.Services.AddScoped<ITodoListApi, InMemoryTodoListApi>();

await builder.Build().RunAsync();