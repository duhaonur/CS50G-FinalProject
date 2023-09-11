using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed;
    [SerializeField] float maxDistanceToGo;

    [HideInInspector] public Transform firedPos;

    private IObjectPool<Bullet> bulletPool;

    private Rigidbody bulletRB;

    private float maxSpeed = 30;

    public bool fired;

    private void Start() => bulletRB = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        if (fired)
            bulletRB.AddForce(bulletSpeed * transform.forward, ForceMode.Impulse);

        if (fired && bulletRB.velocity.magnitude > maxSpeed)
            bulletRB.velocity = bulletRB.velocity.normalized * maxSpeed;

        if (fired && Vector3.Distance(firedPos.position, transform.position) > maxDistanceToGo)
        {
            bulletPool.Release(this);
            bulletRB.velocity = Vector3.zero;
        }
    }

    public void GetPool(IObjectPool<Bullet> pool) => bulletPool = pool;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !gameObject.CompareTag("PlayerBullet"))
        {
            bulletPool.Release(this);
            bulletRB.velocity = Vector3.zero;
        }
        else if (other.gameObject.CompareTag("Enemy") && !gameObject.CompareTag("EnemyBullet"))
        {
            bulletPool.Release(this);
            bulletRB.velocity = Vector3.zero;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            bulletPool.Release(this);
            bulletRB.velocity = Vector3.zero;
        }
    }
}
