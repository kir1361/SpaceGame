using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Items;

public class BuffUIManager : MonoBehaviour
{
    public static BuffUIManager Instance;
    public GameObject buffIconPrefab;
    public Sprite shieldIcon;
    public Sprite rocketIcon;
    public Sprite Coin2XIcon;
    public Sprite extraLifeIcon;
    public Transform buffContainer;


    private Dictionary<ItemType, Sprite> iconDictionary;

    public Dictionary<ItemType, GameObject> activeBuffIcons = new Dictionary<ItemType, GameObject>();

    public HashSet<ItemType> activeBuffs = new HashSet<ItemType>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        iconDictionary = new Dictionary<ItemType, Sprite>
        {
            { ItemType.Shield, shieldIcon },
            //ItemType.AddHp, extraLifeIcon
            { ItemType.Coin2X, Coin2XIcon },

        };
    }

    public void AddBuffIcon(ItemType itemType)
    {
        if (!iconDictionary.ContainsKey(itemType) || activeBuffs.Contains(itemType))
            return;

        GameObject newIcon = Instantiate(buffIconPrefab, buffContainer);
        newIcon.transform.localPosition = Vector3.zero;
        newIcon.GetComponent<Image>().sprite = iconDictionary[itemType];
        activeBuffs.Add(itemType);
        activeBuffIcons[itemType] = newIcon;
    }
    public void UpdateBuffIcons()
    {
        foreach (var icon in activeBuffIcons.Values)
        {
            Destroy(icon);
        }
        activeBuffIcons.Clear();

        foreach (ItemType itemType in activeBuffs)
        {
            GameObject icon = Instantiate(buffIconPrefab, buffContainer);
            icon.transform.localPosition = Vector3.zero;
            icon.GetComponent<Image>().sprite = iconDictionary[itemType];
            activeBuffIcons[itemType] = icon;
        }
    }
    public void SetBuffIconsVisible(bool visible)
    {
        if (activeBuffIcons != null)
        {
            foreach (GameObject buffIcon in activeBuffIcons.Values)
            {
                if (buffIcon != null) buffIcon.SetActive(visible);
            }
        }
    }
    public void RemoveBuffIcon(ItemType itemType)
    {
        if (activeBuffIcons.TryGetValue(itemType, out GameObject icon))
        {
            Destroy(icon);
            activeBuffIcons.Remove(itemType);
            activeBuffs.Remove(itemType);
        }
    }
}
