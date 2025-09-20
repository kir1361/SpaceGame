using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthUI : MonoBehaviour
{
    public static HealthUI instance;
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private GameObject shieldHealthPrefab;
    public Transform healthContainer;
    public Transform bossHealthContainer;

    public Transform shieldContainer;
    [SerializeField] private Sprite HundredPercentHeart, EightyPercentHeart, SixtyPercentHeart, FiftyPercentHeart, TwentyFivePercentHeart, TenthPercentHeart, EmptyHeart;

    [SerializeField] private Sprite BossHundredPercentHeart, BossEightyPercentHeart, BossSixtyPercentHeart, BossFiftyPercentHeart, BossTwentyFivePercentHeart, BossTenthPercentHeart, BossEmptyHeart;

    [SerializeField] private Sprite HundredPercentShield, EightyPercentShield, SixtyPercentShield, FiftyPercentShield, TwentyFivePercentShield, TenthPercentShield, EmptyShield;

    // private List<Image> HealthHearts = new List<Image>();
    // private List<Image> ShieldHearts = new List<Image>();
    bool ShieldState;
    public GameObject Heart,BossHeart;
    public GameObject Shield;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShieldCheck(bool ShieldSt)
    {
        ShieldState = ShieldSt;
    }

    public void SetupUIHealth()//int maxHealth
    {
        if (Heart == null)
        {
            Heart = Instantiate(healthPrefab, healthContainer);
            Heart.gameObject.SetActive(true);
        }
    }
    public void SetupBossUIHealth()
    {
        if (BossHeart == null)
        {
            BossHeart = Instantiate(healthPrefab, bossHealthContainer);
            BossHeart.gameObject.SetActive(true);
        }
    }
    public void SetupUIShield()
    {
        if (ShieldState == true)
        {
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.shieldSoundEffectClip, volume: 0.05f);
            Shield = Instantiate(shieldHealthPrefab, shieldContainer);
            Shield.gameObject.SetActive(true);
        }
    }


    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        // for (int i = 0; i < HealthHearts.Count; i++)
        // {
        //     int HeartValue = Mathf.Clamp(currentHealth - (i * 10), 0, 10);

        //     if (HeartValue == 10) HealthHearts[i].sprite = fullHeart;
        //     else if (HeartValue >= 7) HealthHearts[i].sprite = threeQuarterHeart;
        //     else if (HeartValue >= 5) HealthHearts[i].sprite = halfHeart;
        //     else if (HeartValue >= 2) HealthHearts[i].sprite = quarterHeart;
        //     else HealthHearts[i].sprite = emptyHeart;
        // }
        float healthPercent = (float)currentHealth / maxHealth * 100f;
        if (Heart == null) return;
        if (healthPercent >= 100f)
            Heart.GetComponent<Image>().sprite = HundredPercentHeart;
        else if (healthPercent >= 80f)
            Heart.GetComponent<Image>().sprite = EightyPercentHeart;
        else if (healthPercent >= 60f)
            Heart.GetComponent<Image>().sprite = SixtyPercentHeart;
        else if (healthPercent >= 50f)
            Heart.GetComponent<Image>().sprite = FiftyPercentHeart;
        else if (healthPercent >= 25f)
            Heart.GetComponent<Image>().sprite = TwentyFivePercentHeart;
        else if (healthPercent >= 10f)
            Heart.GetComponent<Image>().sprite = TenthPercentHeart;
        else if (healthPercent == 0f)
            Heart.GetComponent<Image>().sprite = EmptyHeart;
    }
    public void UpdateShieldUI(int currentShield)
    {
        // for (int i = 0; i < ShieldHearts.Count; i++)
        // {
        //     int ShieldValue = Mathf.Clamp(currentShield - (i * 10), 0, 10);

        //     if (ShieldValue == 10) ShieldHearts[i].sprite = fullShield;
        //     else if (ShieldValue >= 7) ShieldHearts[i].sprite = threeQuarterShield;
        //     else if (ShieldValue >= 5) ShieldHearts[i].sprite = halfShield;
        //     else if (ShieldValue >= 2) ShieldHearts[i].sprite = quarterShield;
        //     else ShieldHearts[i].sprite = emptyShield;

        // }
        float shieldPercent = (float)currentShield / Player.instance.maxShield * 100f;
        if (Shield == null) return;
        if (shieldPercent >= 100f)
            Shield.GetComponent<Image>().sprite = HundredPercentShield;
        else if (shieldPercent >= 80f)
            Shield.GetComponent<Image>().sprite = EightyPercentShield;
        else if (shieldPercent >= 60f)
            Shield.GetComponent<Image>().sprite = SixtyPercentShield;
        else if (shieldPercent >= 50f)
            Shield.GetComponent<Image>().sprite = FiftyPercentShield;
        else if (shieldPercent >= 25f)
            Shield.GetComponent<Image>().sprite = TwentyFivePercentShield;
        else if (shieldPercent >= 6f)
            Shield.GetComponent<Image>().sprite = TenthPercentShield;
        else if (shieldPercent == 0f)
            Shield.GetComponent<Image>().sprite = EmptyShield;
    }
    public void UpdateHealthUILaserBoss(int currentHealth, int maxHealth)
    {
        float healthPercent = (float)currentHealth / maxHealth * 100f;
        if (BossHeart == null) return;
        if (healthPercent >= 100f)
            BossHeart.GetComponent<Image>().sprite = BossHundredPercentHeart;
        else if (healthPercent >= 80f)
            BossHeart.GetComponent<Image>().sprite = BossEightyPercentHeart;
        else if (healthPercent >= 60f)
            BossHeart.GetComponent<Image>().sprite = BossSixtyPercentHeart;
        else if (healthPercent >= 50f)
            BossHeart.GetComponent<Image>().sprite = BossFiftyPercentHeart;
        else if (healthPercent >= 25f)
            BossHeart.GetComponent<Image>().sprite = BossTwentyFivePercentHeart;
        else if (healthPercent >= 10f)
            BossHeart.GetComponent<Image>().sprite = BossTenthPercentHeart;
        else if (healthPercent == 0f)
            BossHeart.GetComponent<Image>().sprite = BossEmptyHeart;
    }

}
