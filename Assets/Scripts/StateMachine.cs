using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public IState currentState;

    public void ChangeState(IState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState.OnEnter(this);
    }
    protected virtual void Update()
    {
        currentState?.OnUpdate(this);
    }
}
