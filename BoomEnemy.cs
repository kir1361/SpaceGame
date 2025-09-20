using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class BoomEnemy : EnemyNormalAttack
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform ShootPosBmEn, ShootPosBmEn1;
    protected Vector2 directionBm;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        // Timer = TimeBetweenAttack;
    }


    // Update is called once per frame
    void Update()
    {

    }
   
    protected override IEnumerator Shoot()
    {

        if (isDeathEnemy) yield break;

        Vector2 playerPos = playerTransform.transform.position;
        Vector2 playerVelocity = playerTransform.GetComponent<Rigidbody2D>().velocity;
        Vector2 futurePositionBm = playerPos;
        float estimatedTimeBm = 0f;
        float projectileSpeed = projectilePrefab.GetComponent<ExplodingProjectile>().Speed;

        estimatedTimeBm = Vector2.Distance(transform.position, futurePositionBm) / projectileSpeed;
        futurePositionBm = playerPos + playerVelocity * estimatedTimeBm;

        directionBm = (futurePositionBm - (Vector2)transform.position);//.normalized

        GameObject projectile1 = Instantiate(projectilePrefab, ShootPosBmEn.position, Quaternion.identity);
        GameObject projectile2 = Instantiate(projectilePrefab, ShootPosBmEn1.position, Quaternion.identity);

        projectile1.GetComponent<Rigidbody2D>().velocity = -directionBm * projectilePrefab.GetComponent<ExplodingProjectile>().Speed;
        projectile2.GetComponent<Rigidbody2D>().velocity = -directionBm * projectilePrefab.GetComponent<ExplodingProjectile>().Speed;

    }
}

