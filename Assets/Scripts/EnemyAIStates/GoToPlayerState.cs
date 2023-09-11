using UnityEngine;
using UnityEngine.AI;

public class GoToPlayerState : IState
{
    private readonly EnemyAI enemyAI;
    private readonly GameObject player;
    private readonly NavMeshAgent agent;
    private readonly Animator animator;

    private readonly float minRangeToShoot;
    private readonly float maxRangeToShoot;

    private float randomRangeToShoot;

    private bool hasPath = false;

    public GoToPlayerState(EnemyAI enemyAI, GameObject player, NavMeshAgent agent, Animator animator, float minRangeToShoot, float maxRangeToShoot)
    {
        this.enemyAI = enemyAI;
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        this.minRangeToShoot = minRangeToShoot;
        this.maxRangeToShoot = maxRangeToShoot;
    }

    public void OnEnter(StateMachine stateMachine)
    {
        randomRangeToShoot = Random.Range(minRangeToShoot, maxRangeToShoot);

        agent.enabled = true;
        agent.speed = 3;
        agent.updateRotation = true;

        animator.SetBool("Move", true);

        hasPath = false;

        GetPath();
    }

    public void OnUpdate(StateMachine stateMachine)
    {
        if (enemyAI.gettingHit)
        {
            if (enemyAI.currentHealth <= 50 && Random.value < 0.3f)
            {
                enemyAI.ChangeState(enemyAI.runToCoverState);
                return;
            }
        }

        if (hasPath && !agent.hasPath)
            enemyAI.ChangeState(enemyAI.fireState);
    }

    private void GetPath()
    {
        NavMeshPath path = new NavMeshPath();

        Vector3 playerPosition = player.transform.position;

        int maxAttempts = 30;
        float range = randomRangeToShoot;
        Vector3 newPosition = Vector3.zero;

        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, 360f);

            newPosition = playerPosition + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * range;

            if (CheckLineOfSight(newPosition))
                break;
        }

        NavMesh.CalculatePath(enemyAI.transform.position, newPosition, NavMesh.AllAreas, path);

        agent.SetPath(path);
        hasPath = true;
    }

    private bool CheckLineOfSight(Vector3 targetPosition)
    {
        Vector3 playerPos = player.transform.position;
        playerPos.y = 1;
        targetPosition.y = 1;
        Vector3 directionToTarget = playerPos - targetPosition;

        if (Physics.Raycast(targetPosition, directionToTarget, directionToTarget.magnitude, enemyAI.RayLayers))
            return false;

        return true;
    }
    public void OnExit(StateMachine stateMachine)
    {
        agent.enabled = false;
        animator.SetBool("Move", false);
    }
}
