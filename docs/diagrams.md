# Flux / redux explained with diagrams
Diagrams are simplified to be more readable.

## 1. Component first load
The first time a component is rendered, it needs a state. The Store is responsible to instanciate and store the initial state.
```mermaid
flowchart LR
    Component -- 1. get state --> Store
    Store -- 2 create initial state --> Store
    Store -.-> |3 state| Component
    Component -.-> |4 render| Component
```

## 2. Reducer
A reducer create a new state from the action and the current state.

```mermaid
flowchart LR
    Component -->|1. dispatch action| Reducer
    Reducer -- 2. get current state --> Store
    Store -.-> |3 current state| Reducer
    Reducer -.-> |4 create new state| Reducer
    Reducer -- 5. store new state --> Store
    Reducer -.-> |6 new state| Component
    Component -.-> |7 render| Component
```

## 3. Effect
An effect can execute code that have "side effects" like calling an api.

```mermaid
flowchart LR
    Component -->|dispatch action 1| Effect
    Effect -- get state --> Store
    Store -.-> |state| Effect
    Effect --> |call| Api
    Api -.-> |result| Effect
    Effect -..-> |process data| Effect
    Effect -->|dispatch action 2| End
```

## 4. Effect then Reducer
This example illustrates how effect and reducer collaborate to load data in a state rendered in a component.

```mermaid
flowchart LR
    Component -->|1 dispatch load action| Effect
    Effect --> |2 call| Api
    Api -.-> |3 result| Effect
    Effect -..-> |4 process data| Effect
    Effect -->|5 dispatch loaded action| Reducer
    Reducer -.-> |6 create new state| Reducer
    Reducer -- 7. store state --> Store
    Reducer -.-> |8 state| Component
    Component -.-> |9 render| Component

```