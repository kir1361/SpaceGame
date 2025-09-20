using Unity.Burst.Intrinsics;
using UnityEngine;

public class ExplodingProjectile : Bullet
{
    private bool hasExploded = false;
    [SerializeField] private GameObject explosionPrefab;
    Animator Anim;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        explosionPrefab.transform.localScale = new Vector3(0.7f, 0.7f, 0f);

    }
    void Explode()
    {
        hasExploded = true;
        //Anim.SetTrigger("Explode");
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject); //Чтобы анимация взрыва успела проиграться ,0.3f
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Player") && (type == Type.BoomEnemy))
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(Damage);
            Explode();

        }
        else if (!hasExploded && (collision.gameObject.tag == "Floor") && (type == Type.BoomEnemy))//collision.gameObject.tag == "Wall" 
        {
            Destroy(gameObject,2f);
        }
    }

}
