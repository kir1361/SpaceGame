using System.Collections;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Animator animator;
    private Collider2D cl;
    Rigidbody2D rb;
    public int hitCount = 0;
    public int maxHitCount = 2;
    public int maxHitCountPlayerDamage = 1;
    [SerializeField] private int Damage;
    [SerializeField] public GameObject coinPrefab;
    [SerializeField] private float DropChance;

    bool isDestroyed = false;
    bool fromBullet = false;



    void Awake()
    {
        animator = GetComponent<Animator>();
        cl = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void OnEnable()
    {
        cl.enabled = true;
        rb.simulated = true;
        isDestroyed = false;
        fromBullet = false;


        hitCount = 0; 
        animator.Rebind(); 
        animator.Update(0); 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(isDestroyed) 
            {
                rb.simulated = false;
                return;
            }
            hitCount++;

            if (hitCount >= maxHitCountPlayerDamage) 
            {
                hitCount = 0;
                collision.gameObject.GetComponent<Player>().DamagePlayer(Damage);
                rb.simulated = false;
                animator.SetTrigger("Explode");
            }
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            hitCount = 0;
            cl.enabled = false;
            DestroyMeteor();
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            cl.enabled = true; 
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;
        if (collision.gameObject.CompareTag("Bullet"))
        {
            hitCount++;
            fromBullet = true;
            if (hitCount >= maxHitCount)
            {
                Sounds.Instance.PlaySoundEffect(Sounds.Instance.enemyHitSoundEffectClip, volume: 0.2f);
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Collider2D>().enabled = false;
                hitCount = 0;
                animator.SetTrigger("Explode");
                isDestroyed = true;
                StartCoroutine(DestroyMeteorBullet());

            }
        }
        else if (collision.gameObject.CompareTag("Laser"))
        {
            hitCount = maxHitCount;
            fromBullet = true;
            if (hitCount >= maxHitCount)
            {
                Sounds.Instance.PlaySoundEffect(Sounds.Instance.enemyHitSoundEffectClip, volume: 0.2f);
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Collider2D>().enabled = false;
                hitCount = 0;
                animator.SetTrigger("Explode");
                isDestroyed = true;
                StartCoroutine(DestroyMeteorBullet());
            }
        }
    }

    IEnumerator DestroyMeteorBullet()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        DestroyMeteor();
    }


    public void DestroyMeteor()
    {
        animator.Rebind();
        gameObject.SetActive(false);
        if (fromBullet)
        {
            if (Random.value < DropChance)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }
        }
       
    }
}
