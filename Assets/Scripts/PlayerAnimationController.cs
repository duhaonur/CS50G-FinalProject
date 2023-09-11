using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private BulletSpawner bulletSpawner;

    [SerializeField] private float fireRate;

    [SerializeField] private int magSize;
    [SerializeField] private int bulletInsideMag;


    private ObjectPool<Bullet> pool;

    private Animator animator;

    private float fireTime;

    private bool reloading;
    private bool gameEnded;

    private void OnEnable()
    {
        MyEvents.OnDecreaseHealth += PlayHurtAnimation;
        MyEvents.OnPlayerDeath += PlayerDied;
        MyEvents.OnPlayerDeath += GameEnded;
    }
    private void OnDisable()
    {
        MyEvents.OnDecreaseHealth -= PlayHurtAnimation;
        MyEvents.OnPlayerDeath -= PlayerDied;
        MyEvents.OnPlayerDeath -= GameEnded;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();

        pool = bulletSpawner.bulletPool;

        fireTime = 0;

        reloading = false;
        gameEnded = false;

        MyEvents.CallGetBulletAmount(bulletInsideMag);
    }

    private void Update()
    {
        if (gameEnded || reloading)
            return;

        if (Input.GetMouseButton(0) && bulletInsideMag > 0)
        {
            if (Input.GetMouseButtonDown(0))
                Shoot();
            else
            {
                if (fireTime > fireRate)
                    Shoot();
                else
                    fireTime += Time.deltaTime / fireRate;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !reloading)
            Reload();
    }
    private void Shoot()
    {
        animator.SetTrigger("Fire");

        pool.Get();

        bulletInsideMag--;

        MyEvents.CallGetBulletAmount(bulletInsideMag);

        fireTime = 0;
    }
    private void GameEnded()
    {
        gameEnded = true;
    }
    public void Reload()
    {
        if (!reloading)
        {
            animator.SetTrigger("Reload");
        }
        else
        {
            bulletInsideMag = magSize;
            MyEvents.CallGetBulletAmount(bulletInsideMag);
        }

        reloading = !reloading;
    }
    private void PlayHurtAnimation() => animator.SetTrigger("Hurt");
    private void PlayerDied() => animator.SetBool("Dead", true);
}
