using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class ShopTruckController : MonoBehaviour
{
    private Animator Anim;
    //  GameManager GameManager;
    public Transform[] ItemSpawnPoints;
    public GameObject[] ItemsToSpawn;
    GameObject[] ItemsToDelete;
    GameObject Item;
    private bool ItemsSpawned = false;
    public bool eventOver = false;


    //public GameObject priceTextPrefab; 
    //public Transform [] PriceTextContainer;

    //GameObject TextObj;
    //private TextMeshProUGUI PriceText;
    //private GameObject[] TextObjToDelete;
    Items shopItem;
    public int TotalPurchased;
    public static ShopTruckController instance;


    void Start()
    {
        instance = this;
        Anim = GetComponent<Animator>();
    }
    void Update()
    {
    }
    public void Arrive()
    {
        eventOver = false;
        TotalPurchased = 0;
        if (Player.instance != null)
        {
            Player.instance.SetPlayerControl(true, false, false, false, Vector2.zero);
        }
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.tradeManagerComingSoundEffectClip, volume: 0.05f, p1: 4f, p2: 4f);
        Sounds.Instance.PlayShopMusic(Sounds.Instance.shopMusic, volume: 0.05f);
        Anim.SetTrigger("StartShop");
        ItemsSpawned = false;
        StartCoroutine(OpenShopAfterArrival());
    }
    public void OpenShop()
    {
        Anim.SetTrigger("OpenShop");
        StartCoroutine(SpawnItems());

        if (Random.value <= 1f)//Поменять
        {
            TraderManager.instance.isDiscountText = true;
            TraderManager.instance?.ShowTraderDialog();
        }
        MerchantTimer.Instance?.StartMerchantEvent();
        StartCoroutine(CloseShopAfterTime());
    }
    private void EventOver()
    {
        if (eventOver)
        {
            StopAllCoroutines();
            MerchantTimer.Instance?.OnTimerEnd();
            Leave();
            return;
        }
    }
    public void Leave()
    {
        eventOver = true;
        TraderManager.instance.FadeOutTraderDialog();
        Anim.ResetTrigger("OpenShop");
        Anim.SetTrigger("ExitShop");
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.tradeManagerComingSoundEffectClip, volume: 0.05f, p1: 4f, p2: 4f);
        StartCoroutine(DestroyAfterAnimation());
        StartCoroutine(DestroyItems());
        GettingInterference.Instance.DestroyTask_DrawingSystem();

        Sounds.Instance.musicSource.time = Sounds.Instance.lastPlaybackTime;
        StartCoroutine(Sounds.Instance.Crossfade(Sounds.Instance.shopMusicSource, Sounds.Instance.musicSource, Sounds.Instance.fadeDuration));
    }


    IEnumerator OpenShopAfterArrival()
    {
        yield return new WaitForSeconds(0.5f);
        OpenShop();
        Anim.ResetTrigger("StartShop");

    }
    IEnumerator CloseShopAfterTime()
    {
        float timer = 0f;
        while (timer < 60f && !eventOver)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Leave: " + Time.time);
        if (!eventOver) Leave();
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        Anim.ResetTrigger("ExitShop");
        GameManager.Instance.SetBackgroundScrolling(true);
        if (Player.instance != null)
        {
            Player.instance.SetPlayerControl(true, true, true, true, Vector2.zero);
        }
        transform.position = GameManager.SpawnPoint.position;
    }

    IEnumerator SpawnItems()
    {
        yield return new WaitForSeconds(0.5f);
        if (!ItemsSpawned)
        {
            ItemsToDelete = new GameObject[3];
            List<GameObject> availableItems = new List<GameObject>(ItemsToSpawn);
            List<GameObject> chosenItems = new List<GameObject>();
            while (chosenItems.Count < 3 && availableItems.Count > 0)
            {
                int index = Random.Range(0, availableItems.Count);
                chosenItems.Add(availableItems[index]);
                availableItems.RemoveAt(index);
            }

            for (int i = 0; i < chosenItems.Count; i++)
            {
                Item = Instantiate(chosenItems[i], ItemSpawnPoints[i].position, Quaternion.identity);
                SpriteRenderer sprite = Item.GetComponent<SpriteRenderer>();
                shopItem = Item.GetComponent<Items>();

                if (shopItem.ItemEffect == "AddHp" && Player.instance.currentHealth >= Player.instance.maxHealth)
                {
                    sprite.color = Color.gray;
                }
                else
                {
                    sprite.DOColor(new Color(1f, 1f, 1f, 1.5f), 1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine).SetLink(Item, LinkBehaviour.KillOnDestroy);
                }
                ItemsToDelete[i] = Item;
                StartCoroutine(ShakeItem(Item.transform));
                Animator itemAnim = Item.GetComponent<Animator>();
                if (itemAnim != null)
                {
                    itemAnim.SetTrigger("SpawnItem");
                }
                if (shopItem != null)
                {
                    if (shopItem.ItemEffect == "AddHp" &&
                        Player.instance.currentHealth >= Player.instance.maxHealth)
                    {
                        shopItem.isPlayerMaxHp = true;
                        //sprite.GetComponent<SpriteRenderer>().color = Color.gray;
                    }
                    else shopItem.isPlayerMaxHp = false;

                }
            }

            ItemsSpawned = true;
        }
    }

    IEnumerator DelayEventOver(int time)
    {
        yield return new WaitForSeconds(time);
        StopAllCoroutines();
        eventOver = true;
        EventOver();
    }
    public void CheckAmountItems()
    {
        if (TotalPurchased == 3 && !eventOver)
        {
            StopAllCoroutines();
            eventOver = true;
            EventOver();
        }
        else if (Player.instance.currentCoins <= 2 && !eventOver)
        {
            StartCoroutine(DelayEventOver(10));
        }
        Debug.Log($"CheckAmountItems called. TotalPurchased: {TotalPurchased}");

        foreach (GameObject item in ItemsToDelete)
        {
            if (item != null)
            {
                Items itemComponent = item.GetComponent<Items>();
                if (itemComponent != null)
                {
                    Debug.Log($"Item: {item.name}, isPlayerMaxHp: {itemComponent.isPlayerMaxHp}");

                    if (!itemComponent.isPlayerMaxHp)
                    {
                        Debug.Log("Found purchasable item - returning");
                        return;
                    }
                }
            }
        }
        StartCoroutine(DelayEventOver(2));
    }

    IEnumerator DestroyItems()
    {

        for (int i = 0; i < ItemsToDelete.Length; i++)
        {
            if (ItemsToDelete[i] != null)
            {
                Destroy(ItemsToDelete[i]);
                Destroy(ItemsToDelete[i].GetComponent<Items>().ItemDescriptionsBuffs);
                // Destroy(TextObjToDelete[i]);
                yield return null;
            }
        }

    }
    IEnumerator ShakeItem(Transform item)
    {
        Vector3 startPos = item.localPosition;
        float duration = 0.7f;
        float magnitude = 0.09f;
        float elapsed = 0f;

        if (item == null) yield break;
        while (elapsed < duration)
        {
            if (item == null) yield break;
            float x = Random.Range(-magnitude, magnitude);
            float y = Random.Range(-magnitude, magnitude);
            item.localPosition = startPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
        if (item != null) item.localPosition = startPos;
    }
    
}