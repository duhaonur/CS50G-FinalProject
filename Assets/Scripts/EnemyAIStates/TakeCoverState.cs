using UnityEngine;
using UnityEngine.AI;

public class TakeCoverState : IState
{
    private readonly EnemyAI enemyAI;
    private readonly NavMeshAgent agent;

    private readonly float healTime = 5f;

    private float timer;

    public TakeCoverState(EnemyAI enemyAI, NavMeshAgent agent)
    {
        this.enemyAI = enemyAI;
        this.agent = agent;
    }

    public void OnEnter(StateMachine stateMachine)
    {
        agent.enabled = false;

        enemyAI.gettingHit = false;

        timer = 0;
    }

    public void OnUpdate(StateMachine stateMachine)
    {
        timer += Time.deltaTime;

        if (timer > healTime && !enemyAI.gettingHit)
        {
            enemyAI.ResetHealth();
            enemyAI.ChangeState(enemyAI.goToPlayerState);
        }
        else if (Random.value > 0.5f && enemyAI.gettingHit)
        {
            enemyAI.ChangeState(enemyAI.runToCoverState);
        }
    }

    public void OnExit(StateMachine stateMachine)
    {

    }
}
