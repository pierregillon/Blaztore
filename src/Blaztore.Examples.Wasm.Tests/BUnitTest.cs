using Blaztore.Examples.Wasm.Pages.TodoList.Components;
using Blaztore.Examples.Wasm.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Blaztore.Examples.Wasm.Tests;

public abstract class BUnitTest : TestContext
{
    protected BUnitTest()
    {
        Services
            .AddBlaztore(x => x with
            {
                ConfigureMediator = c => c.RegisterServicesFromAssemblyContaining<TodoListComponent>()
            })
            .AddScoped<ITodoListApi>(_ => Substitute.For<ITodoListApi>());
    }

    protected T GetService<T>() where T : notnull => Services.GetRequiredService<T>();
}