# Blaztore

[![.NET](https://github.com/pierregillon/Blaztore/actions/workflows/dotnet.yml/badge.svg)](https://github.com/pierregillon/Blaztore/actions/workflows/dotnet.yml)

![blaztore home](./assets/blaztore_home.png)

A simple and modest library to implement **Flux/Redux** pattern in **[.NET Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)**.

## Introduction

If you are not familiar with the Flux/Redux model, have a look on the following nice resources:
- [ReduxJs](https://redux.js.org/tutorials/essentials/part-1-overview-concepts)

You can find diagrams [**here**](docs/diagrams.md) to help you understand.

## Goal

Blazor does not contain a native Flux/Redux or MVU api internally.

The objective of this library is to provide a very **simple and minimalist** api to implement a Flux/Redux architecture.

This repository is largely inspired by the following existing repositories:
- [blazor-state](https://github.com/TimeWarpEngineering/blazor-state)
- [Fluxor](https://github.com/mrpmorris/Fluxor/tree/master)

If you are not satisfied by this library, don't hesitate to check them out, they are great.

## Advantages

Compared to the listed existing libraries, Blaztore has the following advantages:
- ✅ Focused on **immutability** for every concepts (State, Action, ...)
- ✅ Never force you to inherit from a **base class or a base record**. Every concepts are based on interfaces. It allows you to structure your code as you like (multiple handling, file structure, ...)
- ✅ Use the underlying [MediatR](https://github.com/jbogard/MediatR) library to dispatch actions. 
It is highly extendable and allows you to easily implement **pipeline** or **preprocessing** to add loggers, retry pattern, ...
- ✅ Use the **Flux/Redux** terminology and not a custom one.
- ✅ Enable to store **multiple instances** of the same state type, identified by a unique id.

## Installation

You can download the latest release NuGet packages from the official Blaztor nuget pages.

- [Blaztore](https://www.nuget.org/packages/Blaztore) ![Nuget](https://img.shields.io/badge/dynamic/xml?color=blue&label=Nuget&prefix=v&query=//Project/PropertyGroup/Version/text()&url=https://raw.githubusercontent.com/pierregillon/Blaztore/main/src/Blaztore/Blaztore.csproj)

## Getting started

You can find below examples to illustrate how to implement concepts with **Blazstore**.

### State
A state represents the data of a particular component.
In .NET **record** is largely recommended for state immutability.

```csharp
public record TaskCreationState(bool IsAddingTask, string? NewTaskDescription) : IState
{
    // Mandatory static factory method to create the initial state.
    public static TaskCreationState Initialize() => new(false, null);
}
```
### Action
Actions are messages that can represent a **command** to mutate the system or an **event** that happened in the system.
You must implement `IAction<TState>` to explicitly define for which state is this action.

```csharp
public record StartAddingNewTask : IAction<TaskCreationState>;

public record DefineNewDescription(string NewDescription) : IAction<TaskCreationState>;

public record TaskListLoaded(IReadOnlyCollection<TaskListItem> Payload) : IAction<TaskListState>;
```

### Get state reference and dispatch actions from a component
A base component `StateComponent` is provided to easily access the `Dispatch<TAction>(TAction action)` and `GetState<TState>()` method.
```html
@inherits Blaztore.Components.StateComponent
```
You can dispatch actions directly from the html, you have no more *code-behind* !
```html
<button onclick="@(() => Dispatch(new StartAddingNewTask()))">
    New task
</button>
```
```csharp
@code {

    private TaskCreationState State => GetState<TaskCreationState>();

    protected override Task OnAfterInitialRenderAsync() =>
        Dispatch(new TaskCreationState.Load());
}
```
### Reducer

A pure reducer is a function that execute an action on a state, returning a new state.
Theoretically, it should not have any dependencies and generates no side effects.

```csharp
public record TaskCreationStateReducer(IStore Store) 
    : IPureReducer<TaskCreationState, StartAddingNewTask>
{
    public TaskCreationState Reduce(TaskCreationState state, StartAddingNewTask action) =>
        state with
        {
            IsAddingTask = true
        };
}
```

You can organize you reducers as you prefer: a reducer *for each* action or a *single* reducer for all your actions.

```csharp
public record StartAddingNewTaskReducer(IStore Store) 
    : IPureReducer<TaskCreationState, StartAddingNewTask>,
      IPureReducer<TaskCreationState, EndAddingNewTask>
{
    public TaskCreationState Reduce(TaskCreationState state, StartAddingNewTask action) =>
        state with
        {
            IsAddingTask = true
        };
        
    public TaskCreationState Reduce(TaskCreationState state, EndAddingNewTask action) =>
        state with
        {
            NewTaskDescription = null,
            IsAddingTask = false
        };
}
```


### Effect

An effect allows you to execute **side effects** on external system and dispatching new actions.

```csharp
public record ExecuteTaskCreationEffect(
    IStore Store, 
    ITodoListApi Api, 
    IActionDispatcher ActionDispatcher
) : IEffect<TaskCreationState, ExecuteTaskCreation>
{
    public async Task Effect(TaskCreationState state, ExecuteTaskCreation action)
    {
        if (string.IsNullOrWhiteSpace(state.NewTaskDescription))
        {
            return;
        }

        await Api.Create(Guid.NewGuid(), state.NewTaskDescription);
        await ActionDispatcher.Dispatch(new EndAddingNewTask());
        await ActionDispatcher.Dispatch(new TodoListState.Load());
    }
}
```

You can find more code on the [examples folder](/src/Blaztore.Examples.Wasm).