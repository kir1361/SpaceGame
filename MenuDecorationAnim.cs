using UnityEngine;

public class MenuDecorationAnim : MonoBehaviour
{
    [Header("Bezier Path Points")]
    public Transform startPoint;
    public Transform controlPoint;
    public Transform endPoint;

    [Header("Animation Settings")]
    public float duration = 5f;
    private float t = 0f;
    private bool isMoving = true;

    [Header("Sprite Animation")]
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    private void Start()
    {
        // Если точки пути не заданы через инспектор — пробуем найти по имени
        if (startPoint == null) startPoint = GameObject.Find("StartPoint").transform;
        if (controlPoint == null) controlPoint = GameObject.Find("ControlPoint").transform;
        if (endPoint == null) endPoint = GameObject.Find("EndPoint").transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sprites.Length > 0)
            spriteRenderer.sprite = sprites[0];
    }

    private void Update()
    {
        if (isMoving)
        {
            t += Time.deltaTime / duration;
            if (t >= 1f)
            {
                t = 1f;
                isMoving = false;
            }

            Vector3 m1 = Vector3.Lerp(startPoint.position, controlPoint.position, t);
            Vector3 m2 = Vector3.Lerp(controlPoint.position, endPoint.position, t);
            transform.position = Vector3.Lerp(m1, m2, t);
        }
        else
        {   
            if (sprites == null || sprites.Length == 0) return;
            transform.position = startPoint.position;
            //currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length;
            currentSpriteIndex++;
            if (currentSpriteIndex >= sprites.Length)
            currentSpriteIndex = 0;
            spriteRenderer.sprite = sprites[currentSpriteIndex];

            t = 0f;
            isMoving = true;
        }
    }
}
