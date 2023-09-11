using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Pool;

public class FireToTargetState : IState
{
    private readonly EnemyAI enemyAI;
    private readonly EnemySO enemySO;

    private readonly NavMeshAgent agent;
    private readonly Animator animator;

    private readonly Rig lookPosRig;

    private readonly GameObject player;

    private readonly Transform shootPos;

    private readonly IObjectPool<Bullet> pool;

    private float maxMagSize;
    private float bulletInsideMag;
    private float rateOfFire;
    private float timer = 0;

    private int noLineOfSight;

    private bool canShoot = true;

    public FireToTargetState(EnemyAI enemyAI, GameObject player, NavMeshAgent agent, Animator animator, EnemySO enemySO,
                             IObjectPool<Bullet> pool, Transform shootPos, Rig lookPosRig)
    {
        this.enemyAI = enemyAI;
        this.player = player;
        this.agent = agent;
        this.animator = animator;
        this.enemySO = enemySO;
        this.pool = pool;
        this.shootPos = shootPos;
        this.lookPosRig = lookPosRig;
    }

    public void OnEnter(StateMachine stateMachine)
    {
        shootPos.localPosition = GetNewShootPos();
        SetLookRig();

        agent.updateRotation = false;

        rateOfFire = enemySO.rateOfFire;
        maxMagSize = enemySO.maxMagSize;
        bulletInsideMag = enemySO.maxMagSize;

        noLineOfSight = 0;
    }

    public void OnUpdate(StateMachine stateMachine)
    {
        LookToThePlayer();
        FireReload();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyReload") || animator.GetCurrentAnimatorStateInfo(0).IsName("FinishedReloading"))
            return;

        if (enemyAI.gettingHit)
        {
            if (enemyAI.currentHealth <= 50 && Random.value < 0.5f)
            {
                enemyAI.ChangeState(enemyAI.runToCoverState);
                return;
            }
        }

        timer += Time.deltaTime;

        if(timer > 1)
        {
            if (!CheckLineOfSight() && noLineOfSight >= 3)
                enemyAI.ChangeState(enemyAI.goToPlayerState);

            timer = 0;
        }


    }
    public void OnExit(StateMachine stateMachine)
    {
        lookPosRig.weight = 0;
    }
    private void FireReload()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyReload"))
            return;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("FinishedReloading"))
        {
            animator.ResetTrigger("Reload");
            animator.SetTrigger("FinishedReloading");

            bulletInsideMag = maxMagSize;
        }
        else if (bulletInsideMag <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyReload"))
        {
            animator.ResetTrigger("Fire");
            animator.SetTrigger("Reload");
        }
        else
        {
            if (canShoot)
                ChangeAim();

        }
    }
    private async void ChangeAim()
    {
        bool changeAim;

        if (Random.value > 0.5f)
            changeAim = true;
        else
            changeAim = false;

        canShoot = false;

        Vector3 newShootPos = GetNewShootPos();

        float timer = 0;

        while (timer < rateOfFire && !enemyAI.tokenSource.IsCancellationRequested)
        {
            if (changeAim)
                shootPos.localPosition = Vector3.Lerp(shootPos.localPosition, newShootPos, timer / rateOfFire);

            timer += Time.deltaTime;

            await Task.Yield();
        }

        if (enemyAI.tokenSource.IsCancellationRequested)
            return;

        animator.SetTrigger("Fire");
        pool.Get();
        bulletInsideMag--;
        canShoot = true;
    }
    private async void SetLookRig()
    {
        float timer = 0;

        while (timer < 1 && !enemyAI.tokenSource.IsCancellationRequested)
        {
            lookPosRig.weight = Mathf.InverseLerp(lookPosRig.weight, 1, timer);
            timer += Time.deltaTime;
            await Task.Yield();
        }
    }
    private Vector3 GetNewShootPos()
    {
        Vector3 localPos = enemyAI.transform.InverseTransformPoint(player.transform.position);
        float distance = (player.transform.position - enemyAI.transform.position).magnitude / 2;
        Vector3 newShootPos = new Vector3(Random.Range(localPos.x - distance, localPos.x + distance),
                                         Random.Range(localPos.y + 1, localPos.y + distance),
                                         Random.Range(localPos.z - distance, localPos.z + distance));
        return newShootPos;
    }

    private void LookToThePlayer()
    {
        Vector3 lookRot = player.transform.position - enemyAI.transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookRot);
        enemyAI.transform.rotation = Quaternion.Slerp(enemyAI.transform.rotation, rotation, 10 * Time.deltaTime);
    }
    private bool CheckLineOfSight()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = enemyAI.transform.position;
        playerPos.y = 1;
        enemyPos.y = 1;

        Vector3 directionToTarget = playerPos - enemyPos;

        if (Physics.Raycast(enemyPos, directionToTarget, directionToTarget.magnitude, enemyAI.RayLayers))
        {
            noLineOfSight++;
            return false;
        }

        noLineOfSight = 0;
        return true;
    }
}
