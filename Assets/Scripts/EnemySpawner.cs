using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyAI enemy;

    [SerializeField] private GameObject player;

    [SerializeField] private float newEnemySpawnTime;
    [SerializeField] private float timer;

    [SerializeField] private int activePool, inactivePool;

    private GameObject parent;

    public ObjectPool<EnemyAI> enemyPool;

    private void Awake() => enemyPool = new ObjectPool<EnemyAI>(SpawnEnemy, GetEnemyFromPool, ReturnEnemyToPool, null, false, 10);

    private void Start() => parent = GameObject.FindGameObjectWithTag("EnemyParent");

    private void Update()
    {
        activePool = enemyPool.CountActive;
        inactivePool = enemyPool.CountInactive;

        timer += Time.deltaTime;

        if (timer > newEnemySpawnTime)
        {
            timer = 0;
            enemyPool.Get();
        }
    }

    private EnemyAI SpawnEnemy()
    {
        var spawnedEnemy = Instantiate(enemy, transform.position, enemy.transform.rotation);
        spawnedEnemy.transform.SetParent(parent.transform);
        spawnedEnemy.GetPool(enemyPool);
        spawnedEnemy.player = player;
        return spawnedEnemy;
    }

    public void GetEnemyFromPool(EnemyAI enemy)
    {
        enemy.transform.position = transform.position;
        enemy.gameObject.SetActive(true);

        if(enemy.goToPlayerState != null)
            enemy.ChangeState(enemy.goToPlayerState);
    }

    private void ReturnEnemyToPool(EnemyAI enemy) => enemy.gameObject.SetActive(false);
}
