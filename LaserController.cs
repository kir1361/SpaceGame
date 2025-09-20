using UnityEngine;
using DG.Tweening;

public class LaserController : MonoBehaviour
{
    public Transform body;
    public Transform head;

    private SpriteRenderer bodyRenderer;
    [SerializeField] private int Damage;

    public float targetLength;
    public float growDuration;
    BoxCollider2D cl;

    private void Awake()
    {
        cl = GetComponent<BoxCollider2D>();
        bodyRenderer = body.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        SetupLaser();
    }
    void UpdateColliderSize(float length)
    {
        cl.size = new Vector2(cl.size.x, length);
        cl.offset = new Vector2(0, length / 2f);
    }
    public void SetupLaser()
    {
        bodyRenderer.size = new Vector2(bodyRenderer.size.x, 0);
        body.localPosition = Vector3.zero;
        head.localPosition = new Vector3(0, 0, 0);

        DOTween.To(() => bodyRenderer.size.y, y =>
        {
            bodyRenderer.size = new Vector2(bodyRenderer.size.x, y);
            head.localPosition = new Vector3(0, 2.92f, 0);

            UpdateColliderSize(y / 1.91f);
        },
        targetLength, growDuration).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    // public void DisableLaser()
    // {
    //     gameObject.SetActive(false);
    // }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Player.instance != null)
            {
                Player.instance.DamagePlayer(Damage);
            }
        }
    }
     
}
