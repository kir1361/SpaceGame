using UnityEngine;

public class ToolTipTrigger : MonoBehaviour
{
    private Items shopItem;
    Coroutine hideTooltipCoroutine;

    private void Start()
    {
        shopItem = GetComponentInParent<Items>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && shopItem != null)
        {
            shopItem.ShowTooltip();
            hideTooltipCoroutine = StartCoroutine(shopItem.HideTooltipAfterDelay(10f));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && shopItem != null)
        {
            if (hideTooltipCoroutine != null)
            {
                StopCoroutine(hideTooltipCoroutine);
            }
            shopItem.HideTooltip();
        }
    }
    private void OnDestroy()
    {
        if (hideTooltipCoroutine != null)
        {
            StopCoroutine(hideTooltipCoroutine);
        }
        if (shopItem != null)
        {
            shopItem.HideTooltipImmediate();
        }

    }
    
}
