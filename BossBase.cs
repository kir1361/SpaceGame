using System;
using DG.Tweening;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Globalization;

public abstract class BossBase : MonoBehaviour
{
    public int maxHealth = 500;
    protected int currentHealth;
    protected bool isAlive = true;
    protected bool secondPhase = false;
    [SerializeField] protected float moveSpeed = 2f;
    float minDistanceToPlayer = 3f;
    float wallPadding = 1f;

    Vector2 arenaMinBound = new Vector2(-3.5f, 2f);
    Vector2 arenaMaxBound = new Vector2(3.5f, 6);
    Vector2 targetPosition;
    int attempts = 0;
    bool validPositionFound = false;
    public Action OnBossDeath;

    [SerializeField] private GameObject deadExplosionEffectPrefab;
    [SerializeField] private GameObject beforeSmokeExplosionEffectPrefab;
    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private GameObject[] bossFragmentsPrefabs;
    [SerializeField] private GameObject LaserDropPrefab;
    [SerializeField] private Transform BossRewardPoint;
    [SerializeField] private Light2D bossRewardLight;
  
    public static BossBase instance;
    GameObject LaserDrop;
    public Transform smoke;
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        secondPhase = false;
        smoke = transform.Find("SmokeSpawnPoint");

        HealthUI.instance.SetupBossUIHealth();
        HealthUI.instance.UpdateHealthUILaserBoss(currentHealth, maxHealth);
        StartBehaviour();
    }
    
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        Vector3 offset = new Vector3(5f, 0f, 0f);
        currentHealth -= damage;
        HealthUI.instance.UpdateHealthUILaserBoss(currentHealth, maxHealth);
    
        float percent = (float)currentHealth / maxHealth;
        if (percent >= 0.49f && percent <= 0.5f && isAlive)
        {
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.BossExplosionPhase,volume: 0.1f);

            beforeSmokeExplosionEffectPrefab.transform.localScale = new Vector3(5f, 5f, 0f);
            Instantiate(beforeSmokeExplosionEffectPrefab, transform.position + offset, Quaternion.identity);
            beforeSmokeExplosionEffectPrefab.transform.localPosition = Vector3.zero;
            CameraShake.Instance.StartShake(0.5f, 0.2f);
            Instantiate(smokeEffectPrefab, smoke);
            secondPhase = true;
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HealthUI.instance.UpdateHealthUILaserBoss(currentHealth, maxHealth);
            Death();
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.BossDeathExplosion,volume: 0.1f);

            OnBossDeath?.Invoke();
        }
    }
    protected void PickNewTargetPosition()
    {
        attempts = 0;
        validPositionFound = false;

        while (!validPositionFound && attempts < 20)
        {
            float x = Random.Range(arenaMinBound.x + wallPadding, arenaMaxBound.x - wallPadding);
            float y = Random.Range(arenaMinBound.y + wallPadding, arenaMaxBound.y - wallPadding);
            targetPosition = new Vector2(x, y);
            if (Vector2.Distance(targetPosition, Player.instance.transform.position) >= minDistanceToPlayer)
            {
                validPositionFound = true;
            }
            attempts++;
        }
        if (!validPositionFound)
        {
            Debug.LogWarning("Could not find valid boss target position.");
        }
    }
    private void DieWithAnim()
    {
        Instantiate(deadExplosionEffectPrefab, transform.position, Quaternion.identity);

        foreach (GameObject fragment in bossFragmentsPrefabs)
        {
            GameObject piece = Instantiate(fragment, transform.position, Quaternion.identity);

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 forceDir = Random.insideUnitCircle.normalized;
                float force = Random.Range(2f, 5f);
                rb.AddForce(forceDir * force, ForceMode2D.Impulse);
            }
        }
        Destroy(gameObject); 
    }

    protected virtual void Death()
    {
        float targetIntensity = 1.5f;
        float duration = 1f;

        isAlive = false;
        Destroy(HealthUI.instance.BossHeart.gameObject);
        StopBehaviour();
        DieWithAnim();
        CameraShake.Instance.StartShake(0.5f, 0.2f);
        DOVirtual.DelayedCall(2f, () =>
        {
            if (LaserDropPrefab != null)
            {
                BossRewardPoint = GameObject.Find("BossRewardPoint").transform;
                LaserDrop = Instantiate(LaserDropPrefab, BossRewardPoint.position, Quaternion.identity);
       
                bossRewardLight = LaserDrop.GetComponentInChildren<Light2D>();
                if (bossRewardLight == null)
                {
                    Debug.LogError("Boss reward light not found!");
                    return;
                }
                LaserDrop.transform.localScale = Vector3.zero;
                LaserDrop.transform.DOScale(0.2f, 0.5f).SetEase(Ease.OutBack);
                LaserDrop.transform.localScale = Vector3.one * 0.2f;

                bossRewardLight.intensity = 0f;

                DOTween.To(() => bossRewardLight.intensity, x => bossRewardLight.intensity = x, targetIntensity, duration).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo).SetLink(bossRewardLight.gameObject, LinkBehaviour.KillOnDestroy);
                LaserDrop.transform.DOScale(0.18f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetLink(LaserDrop, LinkBehaviour.KillOnDestroy);

                GameManager.Instance.SetBackgroundScrolling(false);
                Player.instance.SetPlayerControl(true, false, false, false, Vector2.zero);
            }
        });
    }
    
    protected void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            PickNewTargetPosition();
        }
    }
   
    protected abstract void StartBehaviour();
    protected abstract void StopBehaviour();
}
