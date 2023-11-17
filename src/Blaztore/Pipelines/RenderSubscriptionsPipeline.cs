using MediatR;

namespace Blaztore.Pipelines;

internal record RenderSubscriptionsPipeline<TRequest, TResponse>(
    IStore Store,
    Subscriptions Subscriptions
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is not IAction action)
        {
            return await next();
        }

        var actionDescriptor = GetActionDescriptor(action);

        if (actionDescriptor is null)
        {
            return await next();
        }

        var initialState = actionDescriptor.GetState(Store);

        var result = await next();

        var newState = actionDescriptor.GetState(Store);
        
        if (!Equals(initialState, newState))
        {
            actionDescriptor.ReRenderComponents(Subscriptions);
        }

        return result;
    }

    private static ActionDescriptor? GetActionDescriptor(IAction action)
    {
        var actionInterfaceType = GetActionInterface(action);
        
        if (actionInterfaceType is null)
        {
            return null;
        }

        var stateType = actionInterfaceType.GetGenericArguments().Single();

        var scope = action is IActionOnScopedState stateWithScope
            ? stateWithScope.Scope
            : null;

        return new ActionDescriptor(stateType, scope);
    }

    private static Type? GetActionInterface(IAction action) =>
        action
            .GetType()
            .FindInterfaces(
                (type, _) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAction<>),
                null
            )
            .SingleOrDefault();

    private record ActionDescriptor(Type StateType, object? Scope)
    {
        public IState? GetState(IStore store) =>
            Scope is null
                ? store.GetState(StateType)
                : store.GetState(StateType, Scope);

        public void ReRenderComponents(Subscriptions subscriptions)
        {
            if (Scope is not null)
            {
                subscriptions.ReRenderSubscribers(StateType, Scope);
            }
            else
            {
                subscriptions.ReRenderSubscribers(StateType);
            }
        }
    }
}