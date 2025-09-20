using UnityEngine;

public class Parcel : MonoBehaviour
{
    private Transform target;
    private float speed = 6f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Setup(Transform player)
    {
        target = player;
    }
    // Update is called once per frame
    void Update()
    {
        ParcelToPlayer();
    }
    public void ParcelToPlayer()
    {
        if (target == null) return;
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed*Time.deltaTime);
        if(Vector2.Distance(transform.position, target.position) < 0.1f && LevelManager.instance.hasParcelSpawned == true)
        {
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.parcelPickUpEffectClip, volume: 0.02f, p1:0.9f,p2:0.9f);
            gameObject.SetActive(false);
            LevelManager.instance.hasParcelSpawned = false;
        }
    }
}
