using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Pool;

public class EnemyAI : StateMachine
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField] private Rig lookPosRig;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [SerializeField] private Transform shootPos;

    private CapsuleCollider collider;

    public GoToPlayerState goToPlayerState;
    public FireToTargetState fireState;
    public RunToCoverState runToCoverState;
    public TakeCoverState takeCoverState;
    public PlayDeadState playDeadState;

    public CancellationTokenSource tokenSource;

    public GameObject player;

    public IObjectPool<Bullet> bulletPool;
    public IObjectPool<EnemyAI> enemyPool;

    public LayerMask layerMask;
    public LayerMask RayLayers;

    public float currentHealth;

    public bool gettingHit;
    public bool isDead;

    private void OnDisable() => tokenSource.Cancel();
    private void OnEnable() => tokenSource = new CancellationTokenSource();
    private void Start()
    {
        bulletPool = bulletSpawner.bulletPool;

        tokenSource = new CancellationTokenSource();

        collider = GetComponent<CapsuleCollider>();

        goToPlayerState = new GoToPlayerState(enemyAI, player, agent, animator, enemySO.minRangeToShoot, enemySO.maxRangeToShoot);
        fireState = new FireToTargetState(enemyAI, player, agent, animator, enemySO, bulletPool, shootPos, lookPosRig);
        runToCoverState = new RunToCoverState(enemyAI, player, agent, animator);
        takeCoverState = new TakeCoverState(enemyAI, agent);
        playDeadState = new PlayDeadState(enemyAI, agent, enemyPool, animator, collider);

        ChangeState(goToPlayerState);

        isDead = false;
        currentHealth = enemySO.maxHealth;
    }
    protected override void Update()
    {
        if (currentHealth <= 0 && !isDead)
            ChangeState(playDeadState);

        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            if (currentHealth > 0)
            {
                currentHealth -= 20;
                gettingHit = true;
                animator.SetTrigger("Hit");
            }
        }
    }
    public void ResetHealth() => currentHealth = enemySO.maxHealth;
    public void GetPool(IObjectPool<EnemyAI> pool) => enemyPool = pool;
}
