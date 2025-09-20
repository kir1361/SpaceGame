using DG.Tweening;
using UnityEngine;

public class LaserPickup : MonoBehaviour
{
    [SerializeField] private float pickupDuration;
    public static bool eventOver = false;

    private void Start()
    {
        eventOver = false;
        DG.Tweening.DOVirtual.DelayedCall(pickupDuration, () =>
        {
            Destroy(gameObject);
            Player.instance.SetPlayerControl(true, true, true, true, Vector2.zero);
            GameManager.Instance.SetBackgroundScrolling(true);
        }).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Player.instance != null)
            {
                Sounds.Instance.PlaySoundEffect(Sounds.Instance.buffAddLaser, volume: 0.05f);
                Player.instance.EnableLaser();
                Destroy(gameObject);
                eventOver = true;
                Player.instance.SetPlayerControl(true, true, true, true, Vector2.zero);
                GameManager.Instance.SetBackgroundScrolling(true);
            }
        }
    }
}
