using Blaztore.States;
using Blaztore.Tests.Unit.States;
using Microsoft.Extensions.DependencyInjection;

namespace Blaztore.Tests.Unit;

public abstract class TestBase
{
    private readonly ServiceProvider _serviceProvider;
    protected readonly IStore Store;

    protected TestBase()
    {
        _serviceProvider = new ServiceCollection()
            .AddBlaztore(x => x with
            {
                ConfigureMediator = configuration => configuration.RegisterServicesFromAssemblyContaining<TestGlobalState>()
            })
            .BuildServiceProvider();
        
        Store = GetService<IStore>();
    }

    protected T GetService<T>() => _serviceProvider.GetRequiredService<T>();
}