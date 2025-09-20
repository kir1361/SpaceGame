using UnityEngine;
using DG.Tweening;

public class LaserReward : MonoBehaviour
{
    [SerializeField] private float targetLength = 60f; // Length of the laser beam
    void Start()
    {
        transform.localScale = new Vector3(2f, 0f, 0f);
        transform.DOScaleY(targetLength,0.3f).SetEase(Ease.OutSine);
    }

    private void OnTriggerStay2D(Collider2D other)
     {
        if(other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DamageEnemy(20);
            }
        }
        
     }
}
