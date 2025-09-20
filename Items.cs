using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;


public class Items : MonoBehaviour
{
    public int Price;
    public string ItemEffect;
    private bool isPurchased = false;
    public ItemType itemType;
    public int PurchasedItems;
    CanvasGroup canvasGroupTooltip;
    private Collider2D itemCollider;

    [Range(0, 100)]
    public int discountPercent = 0;
    private int originalPrice;
    

    [Header("Items Description")]

    [SerializeField] private GameObject ItemDescriptionsBuffsPref; //Prefab for item description and buffs
    [HideInInspector] public GameObject ItemDescriptionsBuffs;


    private GameObject TextPrice;
    [SerializeField] private GameObject priceTextPrefab; //Prefab for price text
    private TextMeshProUGUI priceText;
    private Transform PriceTextContainer;
    private GameObject TextDescription;
    [SerializeField] private GameObject descriptionTextPrefab; //Prefab for item description text
    private TextMeshProUGUI itemDescriptionText;
    private Transform ItemDescriptionContainer;
    public bool isPlayerMaxHp;


    private void Start()
    {
        itemCollider = GetComponent<Collider2D>();
        if (itemCollider == null)
        {
            Debug.LogError("Item collider not found on " + gameObject.name);
        }
        itemCollider.enabled = false;

        originalPrice = Price;

        DOVirtual.DelayedCall(5f, () =>
        {
            itemCollider.enabled = true; // Enable the collider after a short delay
        });
    }
    public void ApplyDiscount(int percent)
    {
        discountPercent = Mathf.Clamp(percent, 0, 100);
        Price = originalPrice - (originalPrice * discountPercent / 100);

        // Обновляем текст цены, если он уже отображается
        if (priceText != null)
        {
            UpdatePriceDisplay();
        }
    }
    public void ResetDiscount()
    {
        discountPercent = 0;
        Price = originalPrice;

        if (priceText != null)
        {
            UpdatePriceDisplay();
        }
    }
    public static readonly Dictionary<ItemType, string> Descriptions = new Dictionary<ItemType, string>()
    {
        { ItemType.Shield, "An energy shield that blocks damage." },
        //{ ItemType.RocketLauncher, "Fires powerful rockets at enemies." },
        { ItemType.Coin2X, "Gives x2 coins for each enemy killed." },
        { ItemType.AddHp, "Adds 5hp to current health." },
        { ItemType.Laser, "Fires a laser beam that damages enemies. Replaces current weapon." },
        {ItemType.Dash, "Grants the ability to dash quickly in any direction." }
    };
    private void UpdatePriceDisplay()
    {
        if (discountPercent > 0)
        {
            // Показываем цену со скидкой и перечеркнутую старую цену
            priceText.text = $"<s>{originalPrice}</s> <color=green>{Price}</color>";
        }
        else
        {
            priceText.text = Price.ToString();
        }
    }

    public enum ItemType
    {
        Shield,
        Coin2X,
        AddHp,
        Laser,
        Dash
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isPurchased)
        {
            Player player = collision.GetComponent<Player>();
            int finalPrice = Price;
            if (player != null && player.currentCoins >= finalPrice)
            {
                if (Player.instance.currentHealth >= Player.instance.maxHealth && ItemEffect == "AddHp")
                {
                    isPlayerMaxHp = true;
                    Sounds.Instance.PlaySoundEffect(Sounds.Instance.negativeAnswerSound,volume: 0.05f);
                    //ShopTruckController.instance.CheckAmountItems();
                    return;
                }
                player.SpendCoins(finalPrice);
                ApplyEffect(player);
                isPurchased = true;
                Destroy(gameObject);
                Destroy(ItemDescriptionsBuffs);
                if (ShopTruckController.instance != null && GettingInterference.Instance != null)
                {
                   ShopTruckController.instance.TotalPurchased++;
                   ShopTruckController.instance.CheckAmountItems();
                   GettingInterference.Instance.DestroyTask_DrawingSystem(); 
                }
                
            }
        }
    }
    
    public void ShowTooltip()
    {
        HideTooltipImmediate();
        Canvas canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        ItemDescriptionsBuffs = Instantiate(ItemDescriptionsBuffsPref, canvas.transform);
        Vector3 pos = transform.position + Vector3.up * 2f;

        ItemDescriptionsBuffs.transform.position = pos;

        canvasGroupTooltip = ItemDescriptionsBuffs.GetComponent<CanvasGroup>();
        canvasGroupTooltip.DOFade(1f, 0.5f).SetEase(Ease.OutBack).SetLink(ItemDescriptionsBuffs, LinkBehaviour.KillOnDestroy);
        PriceTextContainer = ItemDescriptionsBuffs.transform.Find("PriceContainer");
        ItemDescriptionContainer = ItemDescriptionsBuffs.transform.Find("ItemDescriptionContainer");

        //canvasGroupTooltip.DOFade(1f, 0.5f).SetEase(Ease.OutBack);

        if (TextPrice == null)
        {
            TextPrice = Instantiate(priceTextPrefab, PriceTextContainer.transform);
            TextPrice.transform.localPosition = Vector3.zero;
            TextPrice.transform.localScale = Vector3.zero;
            priceText = TextPrice.GetComponent<TextMeshProUGUI>();
            //priceText.text = Price.ToString();
            UpdatePriceDisplay();
            TextPrice.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
        //////////////////////////////////////////////////////////////
        if (TextDescription == null)
        {
            TextDescription = Instantiate(descriptionTextPrefab, ItemDescriptionContainer);
            TextDescription.transform.localPosition = Vector3.zero;
            TextDescription.transform.localScale = Vector3.zero;
            itemDescriptionText = TextDescription.GetComponent<TextMeshProUGUI>();
            if (Descriptions.TryGetValue(itemType, out string description))
                itemDescriptionText.text = description;
            else
                itemDescriptionText.text = "No description available:(";

            TextDescription.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        }
    }
    void Update()
    {
        // float glow = Mathf.PingPong(Time.time * 0.5f, 0.7f)+ 1.5f;
        // Color color = GetComponent<SpriteRenderer>().color;
        // color.a = glow;
        // GetComponent<SpriteRenderer>().color = color;
        ShopTruckController.instance.CheckAmountItems();
    }
    public void HideTooltip()
    {
        if (canvasGroupTooltip != null)
        {
            DOTween.Kill(canvasGroupTooltip);
            canvasGroupTooltip.DOFade(0f, 0.5f).SetEase(Ease.OutBack).SetLink(ItemDescriptionsBuffs, LinkBehaviour.KillOnDestroy).OnComplete(() =>
            {
                if (ItemDescriptionsBuffs != null)
                    Destroy(ItemDescriptionsBuffs);
            });
        }
    }  
    public void HideTooltipImmediate()
    {
        if (ItemDescriptionsBuffs != null)
        {
            DOTween.Kill(ItemDescriptionsBuffs);
            Destroy(ItemDescriptionsBuffs);
            ItemDescriptionsBuffs = null;
            canvasGroupTooltip = null;
            TextPrice = null;
            TextDescription = null;
            priceText = null;
            itemDescriptionText = null;
        }
    }   
    public IEnumerator HideTooltipAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideTooltip();
    }
        void ApplyEffect(Player player)
        {
        switch (ItemEffect)
        {
            case "Shield":
                player.AddShield();
                break;
            //case "RocketLauncher":
            //    player.EnableRocketLauncher();
            //    break;
            case "Coin2X":
                player.AddCoins2X();
                break;
            case "AddHp":
                player.AddHp();
                break;
            case "Dash":
                player.AddDash();
                break;
            }
            BuffUIManager.Instance.AddBuffIcon(itemType);
        }
    void OnDestroy()
    {
        HideTooltipImmediate();
    }
}
