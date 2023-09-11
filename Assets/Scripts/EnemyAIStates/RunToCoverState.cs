using UnityEngine;
using UnityEngine.AI;

public class RunToCoverState : IState
{
    private readonly EnemyAI enemyAI;
    private readonly GameObject player;
    private readonly NavMeshAgent agent;
    private readonly Animator animator;

    private GameObject closestCover;
    private Vector3 playerPos;

    private bool hasPath;
    public RunToCoverState(EnemyAI enemyAI, GameObject player, NavMeshAgent agent, Animator animator)
    {
        this.enemyAI = enemyAI;
        this.player = player;
        this.agent = agent;
        this.animator = animator;
    }

    public void OnEnter(StateMachine stateMachine)
    {
        hasPath = false;

        agent.updateRotation = true;

        Collider[] hitColliders = Physics.OverlapSphere(enemyAI.transform.position, 20, enemyAI.layerMask);

        float previousColliderDistance = 0;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Wall"))
            {
                if (previousColliderDistance == 0)
                {
                    previousColliderDistance = Vector3.Distance(enemyAI.transform.position, hitCollider.transform.position);
                    closestCover = hitCollider.gameObject;
                }
                else if (previousColliderDistance > Vector3.Distance(enemyAI.transform.position, hitCollider.transform.position))
                {
                    previousColliderDistance = Vector3.Distance(enemyAI.transform.position, hitCollider.transform.position);
                    closestCover = hitCollider.gameObject;
                }
            }
        }

        float distBeforeNP = Vector3.Distance(enemyAI.transform.position, closestCover.transform.position);

        playerPos = player.transform.forward;
        playerPos.x = 0;
        playerPos.y = 0;
        playerPos.z = Mathf.Abs(playerPos.z);

        float angle = Vector3.SignedAngle(playerPos, enemyAI.transform.position - player.transform.position, Vector3.up);

        float x = Mathf.Sin(angle * Mathf.Deg2Rad);
        float z = Mathf.Cos(angle * Mathf.Deg2Rad);

        Vector3 hidePosition = closestCover.transform.position;
        hidePosition.x += x;
        hidePosition.z += z;

        float distanceToHidePos = Vector3.Distance(enemyAI.transform.position, hidePosition);

        Vector3 directionToCover = (hidePosition - enemyAI.transform.position).normalized;
        
        hidePosition += directionToCover * ((distanceToHidePos - distBeforeNP) + agent.radius * 2);

        agent.enabled = true;
        agent.speed = 5;
        agent.SetDestination(hidePosition);

        animator.SetBool("RunToCover", true);

        hasPath = true;
    }

    public void OnUpdate(StateMachine stateMachine)
    {
        if (!agent.hasPath && hasPath)
            enemyAI.ChangeState(enemyAI.takeCoverState);
    }

    public void OnExit(StateMachine stateMachine)
    {
        agent.speed = 1;
        animator.SetBool("RunToCover", false);
    }
}
