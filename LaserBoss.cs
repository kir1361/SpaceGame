using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LaserBoss : BossBase
{
    //private float rotateCooldown = 5f;
    private float laserDuration = 6f;
    [SerializeField] private float timeBeetwenAttacks;
    float angle;



    public Transform[] laserPoints;
    public GameObject laserPrefab;
    public GameObject standartBulletPrefab;

    int standartBulletCount = 5;

    public bool isShootingLaser = false;
    private Coroutine BossFightRoutine;
    private Transform fightingPoint;
    private Tween rotationTween;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float radius = 4f;
    [SerializeField] private float delayBeforeRotate = 2f;
    [SerializeField] private float rotationSpeed = 100f;
    private bool canMove = true;


    protected override void StartBehaviour()
    {
        fightingPoint = GameObject.Find("BossFightPoint").transform;
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.bossEnterSoundEffectClip, volume: 0.3f);
        transform.DOMove(fightingPoint.position, 1f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            PickNewTargetPosition();
            BossFightRoutine = StartCoroutine(BossAttacks());

        });
    }
    protected override void StopBehaviour()
    {
        if (BossFightRoutine != null) StopCoroutine(BossFightRoutine);

    }

    private void Update()
    {
        if (isAlive && !isShootingLaser && canMove)
        {
            Move();
        }
   
    }

    IEnumerator LaserAttackRoutine()
    {
        isShootingLaser = true;
        float currentZ = transform.eulerAngles.z;
        rotationTween = transform.DORotate(new Vector3(0, 0, currentZ + 360), 10f, RotateMode.FastBeyond360)
                               .SetEase(Ease.Linear)
                               .SetLoops(-1, LoopType.Restart);
        GameObject[] lasers = new GameObject[laserPoints.Length];

        for (int i = 0; i < laserPoints.Length; i++)
        {
            GameObject laser = Instantiate(laserPrefab, laserPoints[i].position, laserPoints[i].rotation, laserPoints[i]);
            lasers[i] = laser;
            LaserController lc = laser.GetComponent<LaserController>();
            lc.targetLength = 5.75f;
            lc.growDuration = 0.5f;
            lc.SetupLaser();
        }

        yield return new WaitForSeconds(laserDuration);
        for (int i = 0; i < lasers.Length; i++)
        {
            if (lasers[i] != null)
                Destroy(lasers[i]);
        }
        rotationTween.Kill();
        isShootingLaser = false;
    }
    IEnumerator StandartAttackRoutine()
    {
        float duration = 1f;
        float shootInterval = 1f;
        float spreadAngle = 180f;

        float currentZ = transform.eulerAngles.z;
        rotationTween = transform.DORotate(new Vector3(0, 0, currentZ + 360), 5f, RotateMode.FastBeyond360)
           .SetEase(Ease.Linear)
           .SetLoops(-1, LoopType.Restart);
        float elapsed = 0f;
        //canMove = false;

        while (elapsed < duration)
        {
            for (int i = 0; i < laserPoints.Length; i++)
            {
                float startAngle = -spreadAngle / 2f;
                float angleStep = spreadAngle / (standartBulletCount - 1);

                for (int j = 0; j < standartBulletCount; j++)
                {
                    angle = startAngle + angleStep * j;
                    Vector2 dir = Quaternion.Euler(0, 0, angle) * laserPoints[i].up;

                    GameObject proj = Instantiate(standartBulletPrefab, laserPoints[i].position, Quaternion.identity);
                    float projectileSpeed = proj.GetComponent<Bullet>().Speed;

                    Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = dir.normalized * projectileSpeed;
                    }
                }
            }
            yield return new WaitForSeconds(shootInterval);
            elapsed += shootInterval;
        }
        rotationTween.Kill();   
    }

//    IEnumerator RotateProjectile(Transform projTransform)
//   {
//         while (true)
//         {
//             projTransform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
//             yield return null;
//         }
//     }   
    IEnumerator BossAttacks()
    {
        while (isAlive)
        {
            if (secondPhase == false)
            {
                yield return StartCoroutine(StandartAttackRoutine());
                yield return new WaitForSeconds(timeBeetwenAttacks);
            }
            else
            {
                moveSpeed = 4f;
                yield return StartCoroutine(LaserAttackRoutine());
                yield return new WaitForSeconds(timeBeetwenAttacks);
                yield return StartCoroutine(TwoLaserAttack());
                yield return new WaitForSeconds(timeBeetwenAttacks);
                 yield return StartCoroutine(StandartAttackRoutine());
                yield return new WaitForSeconds(timeBeetwenAttacks);
            }
           
          
        }
    }
    IEnumerator TwoLaserAttack()
    {
        int laserCount = 2;
        int pairIndex = Random.Range(0, 1);
        Transform[] choosenLaserPoints;
        if (pairIndex == 0)
        {
            choosenLaserPoints = new Transform[] { laserPoints[0], laserPoints[3] };
        }
        else
        {
            choosenLaserPoints = new Transform[] { laserPoints[1], laserPoints[2] };

        }

        isShootingLaser = true;
        float currentZ = transform.eulerAngles.z;
        rotationTween = transform.DORotate(new Vector3(0, 0, currentZ + 360), 10f, RotateMode.FastBeyond360)
                               .SetEase(Ease.Linear)
                               .SetLoops(-1, LoopType.Restart);
        
        GameObject[] lasers = new GameObject[laserCount];

        for (int i = 0; i < laserCount; i++)
        {
            GameObject laser = Instantiate(laserPrefab, choosenLaserPoints[i].position, choosenLaserPoints[i].rotation, laserPoints[i]);
            lasers[i] = laser;
            LaserController lc = laser.GetComponent<LaserController>();
            lc.targetLength = 5.75f;
            lc.growDuration = 0.5f;
            lc.SetupLaser();
        }

        yield return new WaitForSeconds(laserDuration);
        for (int i = 0; i < lasers.Length; i++)
        {
            if (lasers[i] != null)
                Destroy(lasers[i]);
        }
        rotationTween.Kill();
        isShootingLaser = false;
    }

    
}
