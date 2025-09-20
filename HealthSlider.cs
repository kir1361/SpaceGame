using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSlider : MonoBehaviour
{
    public Slider healthSlider;
    public RectTransform healthBarTransform;
    private Camera mainCamera;
    private Transform enemyTransform;
    private CanvasGroup canvasGroup;
    private Coroutine hideCoroutine;
    public static HealthSlider instance;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    void Awake()
    {
        instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        canvasGroup.alpha = 0f;
        mainCamera = Camera.main;
        enemyTransform = transform.parent; 
    }

    void Update()
    {
       transform.position = enemyTransform.position + offset;
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
        ShowHealthBar();
    }
    private void ShowHealthBar()
    {
        canvasGroup.alpha = 1f;
        if (healthSlider.value <= 0)
        {
            healthSlider.gameObject.SetActive(false);
        }
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
            hideCoroutine = StartCoroutine(HideAfterDelay());
    }
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        float duration = 2f; 
        float startAlpha = canvasGroup.alpha;
        float targetAlpha = 0f; 
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }
        canvasGroup.alpha = targetAlpha;
    }
    public void AdjustSize(float maxHealth)
    {        
            float newWidth = (maxHealth * 15f);
            healthBarTransform.sizeDelta = new Vector2(newWidth, healthBarTransform.sizeDelta.y);
        
    }
}
