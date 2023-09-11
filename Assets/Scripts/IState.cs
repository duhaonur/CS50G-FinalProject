public interface IState
{
    void OnEnter(StateMachine stateMachine);
    void OnUpdate(StateMachine stateMachine);
    void OnExit(StateMachine stateMachine);
}
