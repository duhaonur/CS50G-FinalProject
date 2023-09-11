using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class PlayDeadState : IState
{
    private readonly EnemyAI enemyAI;
    private readonly Animator animator;
    private readonly NavMeshAgent agent;

    private readonly IObjectPool<EnemyAI> pool;

    private readonly CapsuleCollider collider;

    private readonly float objReleaseTime = 15;

    private float timer = 0;

    private bool objReleased = false;

    public PlayDeadState(EnemyAI enemyAI, NavMeshAgent agent, IObjectPool<EnemyAI> pool, Animator animator, CapsuleCollider collider)
    {
        this.enemyAI = enemyAI;
        this.pool = pool;
        this.animator = animator;
        this.collider = collider;
        this.agent = agent;
    }

    public void OnEnter(StateMachine stateMachine)
    {
        MyEvents.CallIncreaseScore();

        animator.SetBool("Dead", true);

        agent.enabled = false;
        collider.enabled = false;
        enemyAI.isDead = true;
        objReleased = false;

        timer = 0;

    }
    public void OnUpdate(StateMachine stateMachine)
    {
        timer += Time.deltaTime;
        if (timer > objReleaseTime && !objReleased)
        {
            objReleased = true;
            pool.Release(enemyAI);
        }
    }

    public void OnExit(StateMachine stateMachine)
    {
        animator.SetBool("Dead", false);

        collider.enabled = true;

        enemyAI.ResetHealth();

        enemyAI.gettingHit = false;
        enemyAI.isDead = false;
    }

}
