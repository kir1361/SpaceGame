using System.Threading;
using UnityEngine;
using System.Collections;


public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

     public float Speed;
     public int Damage;
     public Type type;
    [System.Serializable]
    public enum Type
    {
        Player,
        Enemy,
        BoomEnemy,
        LaserBoss
    }

    private void Start()
    {
        Destroy(gameObject, 5f); 
    }
    protected void Update()
    {
        if (type == Type.Player)
        {
            transform.Translate(Vector3.up * Time.deltaTime * Speed);
        }
        else if (type == Type.Enemy)
        {
            transform.Translate(Vector3.down * Time.deltaTime * Speed);
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if ((collision.gameObject.tag == "Floor") && (type == Type.Enemy || type == Type.Player || type == Type.LaserBoss))//collision.gameObject.tag == "Wall"
        {
            Death();
        }
        else if (collision.gameObject.tag == "TopWall" && type == Type.Player)
        {
            Death();
        }
        else if (collision.gameObject.tag == "Enemy" && type == Type.Player)
        {
            collision.gameObject.GetComponent<Enemy>().DamageEnemy(Damage);
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.enemyHitSoundEffectClip, volume: 0.2f);
            Death();
        }
        else if (collision.gameObject.tag == "LaserBoss" && type == Type.Player)
        {
            collision.gameObject.GetComponent<BossBase>().TakeDamage(Damage);
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.enemyHitSoundEffectClip, volume: 0.2f);
            
            Death();
        }

        else if ((collision.gameObject.tag == "Player") && (type == Type.Enemy || type == Type.LaserBoss))
        {
            collision.gameObject.GetComponent<Player>().DamagePlayer(Damage);
            Death();
        }
        else if (collision.gameObject.tag == "Meteor")
        {
            Death();
        }
        else if (collision.gameObject.tag == "ShopTruck")
        {
            Death();
        }

    }


    public void Death()
    {
        Destroy(gameObject);
    }
}
