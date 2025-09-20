using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Vector2 = UnityEngine.Vector2;



public class EnemyNormalAttack : Enemy
{
    private float Timer;
    [SerializeField] private float TimeBetweenAttack;
    [SerializeField] private float TimeBetweenAttacks;

    [SerializeField] private Transform ShootPosEn;

    protected Transform playerTransform;
    protected Vector2 direction;
    [SerializeField] private GameObject BulletPrefab;
    private float bulletSpeed;
    [SerializeField] private float fanBulletSpeed = 5f;
    [SerializeField] float spreadAngle = 30f;
    [SerializeField] float duration = 2f;
    [SerializeField] float shootInterval = 1f;
    [SerializeField] int standartBulletCount = 3;
    




    public override void Start()
    {
        base.Start();
        playerTransform = Player.instance.transform;
        bulletSpeed = BulletPrefab.GetComponent<Bullet>().Speed;

        // Timer = TimeBetweenAttack;

    }               
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Timer += Time.fixedDeltaTime;
        if (Timer >= TimeBetweenAttack)
        {
            StartCoroutine(VariationAttacks());
            Timer = 0;
        }
    }


    protected virtual IEnumerator Shoot()
    {
        Vector2 playerPos = playerTransform.transform.position;
        
        Vector2 playerVelocity = playerTransform.GetComponent<Rigidbody2D>().velocity;
        Vector2 futurePosition = playerPos;
        float estimatedTime = 0f;
        
        estimatedTime = Vector2.Distance(transform.position, futurePosition) / bulletSpeed;
        futurePosition = playerPos + playerVelocity * estimatedTime;
        if (isDeathEnemy) yield break;
        direction = (futurePosition - (Vector2)transform.position);//.normalized

        GameObject bullet = Instantiate(BulletPrefab, ShootPosEn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
    }
    IEnumerator FanShooting()
    {
        if (isDeathEnemy) yield break;
        float elapsed = 0f;


        while (elapsed < duration)
        {
            for (int i = 0; i < standartBulletCount; i++)
            {
                float angle = -spreadAngle / 2 + spreadAngle / (standartBulletCount - 1) * i;
                Quaternion rotation = ShootPosEn.rotation * Quaternion.Euler(0, 0, angle);
                Vector2 direction = rotation * Vector2.down;
                GameObject bullet = Instantiate(BulletPrefab, ShootPosEn.position, rotation);
                bullet.GetComponent<Rigidbody2D>().velocity =  direction * fanBulletSpeed;

            }
            yield return new WaitForSeconds(shootInterval);
            elapsed += shootInterval;
        }
    }
    IEnumerator VariationAttacks()
    {
        if (isDeathEnemy) yield break;
        else
        {
            yield return StartCoroutine(Shoot());
            if (isDeathEnemy) yield break;
            yield return new WaitForSeconds(TimeBetweenAttacks);
            yield return StartCoroutine(FanShooting());
            if (isDeathEnemy) yield break;
            yield return new WaitForSeconds(TimeBetweenAttacks);
        }   
    }
}
