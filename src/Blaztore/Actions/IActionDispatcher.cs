namespace Blaztore.Actions;

public interface IActionDispatcher
{
    /// <summary>
    /// Dispatches an action to its single action handler (effect, reducer or handler).
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    Task Dispatch(IAction action);

    /// <summary>
    /// Dispatches multiple actions in sequence.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="others"></param>
    public async Task Dispatch(IAction action, params IAction[] others)
    {
        foreach (var actionToDispatch in others.Prepend(action))
        {
            await Dispatch(actionToDispatch);
        }
    }
}