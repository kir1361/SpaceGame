using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System;

public class Enemy : MonoBehaviour
{
    Player player;

    Rigidbody2D rb;
    [SerializeField] int Health;
    [SerializeField] int MaxHealth;

    [SerializeField] float DodgeSpeed, StopDistance, Speed, DodgeCheckRadius, DodgeTime, BulletDetectionAngle, BackDistance;
    protected bool isDodging = false;
    [SerializeField] private GameObject[] flames;
    [SerializeField] public GameObject coinPrefab;
    Animator Anim;
    public bool isDeathEnemy = false;
    protected float distanceToPlayer;
    protected float DropChance = 0.7f;

    public GameObject HealthBarPrefab;  
    private HealthSlider HealthBar;
    public event Action OnEnemyDeath;
    private bool isMovingToFightPosition = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public virtual void Start()
    {
        Health = MaxHealth;

        Sounds.Instance.PlaySoundEffect(Sounds.Instance.enemyEnterSoundEffectClip, volume: 0.3f);
        if (isMovingToFightPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, -3f), 5f * Time.deltaTime);
            isMovingToFightPosition = false;
        }

        GameObject HealthBarObj = Instantiate(HealthBarPrefab, transform.position, Quaternion.identity);
        HealthBarObj.transform.SetParent(transform);
        HealthBar = HealthBarObj.GetComponent<HealthSlider>();
        HealthBar.SetHealth(Health, MaxHealth);
        HealthBar.AdjustSize(MaxHealth);

        rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        player = Player.instance;
    }
    public void DamageEnemy(int Damage)
    {
        if (isDeathEnemy) return;
        Health -= Damage;
        HealthBar.SetHealth(Health, MaxHealth);

        if (Health <= 0)
        {
            Health = 0;
            isDeathEnemy = true;

            SetFlamesActive(false);
            Anim.SetTrigger("isDeathEnemy");
            OnEnemyDeath?.Invoke();
            StopAllCoroutines();
            StartCoroutine(DestroyAfterAnimationEnemy());   
            
        }
    }
    IEnumerator DestroyAfterAnimationEnemy()
    {
       
        yield return new WaitForSeconds(Anim.GetCurrentAnimatorStateInfo(0).length);
        if (UnityEngine.Random.value < DropChance)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        OnEnemyDeath -= LevelManager.instance.EnemyDefeated;
    }
    protected virtual void FixedUpdate()
    {
        if (!isDodging)
        {
            MoveEnemy();
            AvoidOtherEnemies();
        }
        DetectIncomingBullets();
    }

    void AvoidOtherEnemies()
    {
        if (isMovingToFightPosition) return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f); 
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy") && col.gameObject != gameObject)
            {
                Vector2 awayDirection = (transform.position - col.transform.position).normalized;
                transform.position += (Vector3)awayDirection * Time.fixedDeltaTime * 2; 
            }
        }
    }

    protected void MoveEnemy()
    {
        if (isMovingToFightPosition) return;
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer >= StopDistance)
        {
            transform.position =  Vector2.MoveTowards(transform.position, player.transform.position, Speed * Time.fixedDeltaTime);
            SetFlamesActive(true);
        }
        else if (distanceToPlayer <= BackDistance)
        {
            transform.position +=  Vector3.up * Speed * Time.fixedDeltaTime;
            SetFlamesActive(true);
        }
        else
        {
            SetFlamesActive(false);
        }
    }

    protected void DetectIncomingBullets()
    {
        Collider2D[] bullets = Physics2D.OverlapCircleAll(transform.position, DodgeCheckRadius);
         foreach (Collider2D bullet in bullets)
        {
            Bullet bulletScript = bullet.GetComponent<Bullet>(); 
            if (bulletScript != null && bulletScript.type == Bullet.Type.Enemy)
            {
                continue; 
            }
            if (bullet.CompareTag("Bullet"))
            {
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    Vector2 toEnemy = (Vector2)transform.position - bulletRb.position;
                    Vector2 bulletDirection = bulletRb.linearVelocity.normalized;
                    float angle = Vector2.Angle(bulletDirection, toEnemy);
                    if (angle < BulletDetectionAngle && !isDodging)
                    {
                        Dodge();
                        return;
                    }
                }
            }
        }
    }

    protected void Dodge()
    {
        if (isDeathEnemy) return;
        if (isMovingToFightPosition) return;
        isDodging = true;
        Vector2 dodgeDirection = (UnityEngine.Random.value > 0.5f) ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(rb.position, dodgeDirection, 1f);
        if (hit.collider != null)
        {
            dodgeDirection = -dodgeDirection;
            SetFlamesActive(true);
        }
        StartCoroutine(DodgeCoroutine(dodgeDirection));
    }

    IEnumerator DodgeCoroutine(Vector3 dodgeDirection)
    {
        isDodging = true;
        float elapsedTime = 0;
        SetFlamesActive(false);

        while (elapsedTime < DodgeTime)
        {
            if (isDeathEnemy) yield break;
            transform.position += (dodgeDirection * DodgeSpeed * Time.deltaTime);
            SetFlamesActive(true);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
    }

    public void SetFlamesActive(bool state)
    {
        foreach (GameObject flame in flames)
        {
            flame.SetActive(state);
        }
    }
}
