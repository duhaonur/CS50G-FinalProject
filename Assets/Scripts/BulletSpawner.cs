using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bullet;

    [SerializeField] private Transform firePos;

    [SerializeField] private int activePool, inactivePool;

    private GameObject parent;

    public ObjectPool<Bullet> bulletPool;

    public string bulletTag;

    private void Awake() => bulletPool = new ObjectPool<Bullet>(SpawnBullet, GetBulletFromPool, ReturnBulletToPool, null, false, 120);
    private void Start() => parent = GameObject.FindGameObjectWithTag("BulletParent");
    private void Update()
    {
        activePool = bulletPool.CountActive;
        inactivePool = bulletPool.CountInactive;
    }

    private Bullet SpawnBullet()
    {
        var spawnedBullet = Instantiate(bullet);
        spawnedBullet.transform.SetParent(parent.transform);
        spawnedBullet.tag = bulletTag;
        spawnedBullet.GetPool(bulletPool);
        return spawnedBullet;
    }

    public void GetBulletFromPool(Bullet bul)
    {
        bul.gameObject.SetActive(true);
        bul.gameObject.transform.position = firePos.position;
        bul.transform.rotation = firePos.rotation;
        bul.firedPos = firePos;
        bul.fired = true;
    }

    private void ReturnBulletToPool(Bullet bul)
    {
        bul.gameObject.SetActive(false);
        bul.fired = false;
    }
}
