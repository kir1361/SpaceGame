using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

using System.Collections.Generic;
using DG.Tweening;
public class DirDialogue : MonoBehaviour
{
    public static DirDialogue Instance; 
    public CanvasGroup dialogGroup; 
    public TextMeshProUGUI dialogText;
    public Image Coins;
    public bool isGreeting = true;
   // public Button continueButton; // Кнопка для мобильных
    private Dictionary<string, string> greetingsTexts = new Dictionary<string, string>();
    private Dictionary<string, string> farewellTexts = new Dictionary<string, string>();
    public float typingSpeed; 
    public bool isWaitingForInput = false; 
    public System.Action OnDialogClosed;
    public bool stopTyping = false;
     private bool isProcessingInput = false;

    private bool isTyping = false; 
    string textToShow = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //continueButton.gameObject.SetActive(false);

        // if (Coins != null) Coins.gameObject.SetActive(false);
        greetingsTexts.Add("LVL1", "You're late again! Deliver the package to the destination. Hopefully, you'll manage it this time.");
        greetingsTexts.Add("LVL2", "Great job on the last delivery! Now, take this package to the exit point.");

        farewellTexts.Add("LVL1", "You've exceeded expectations. Now, enter.");
        farewellTexts.Add("LVL2", "Well done. Now, proceed to the exit.");
    }
    public void Init()
    {
        dialogGroup.alpha = 0;
        isGreeting = true;
        Coins.gameObject.SetActive(false);
        stopTyping = false;
        StopAllCoroutines();
    }
    
    public void ShowDialog()
    {
        if(PauseManager.Instance != null) PauseManager.Instance.canBePaused = false;
        if (HealthUI.instance.Heart != null)
        {
            HealthUI.instance.Heart.SetActive(false);
        }
        if (HealthUI.instance.Shield != null)
        {
            HealthUI.instance.Shield.SetActive(false);
        }
        Coins.gameObject.SetActive(false);
        GameManager.Instance.SetBackgroundScrolling(false);
        if (BuffUIManager.Instance != null)
        {
            BuffUIManager.Instance.SetBuffIconsVisible(false);
        }
        Player.instance.SetPlayerControl(false, false, false, false, Vector2.zero);
        StartCoroutine(ShowAndTypeText());
    }

    IEnumerator ShowAndTypeText()
    {
        StartCoroutine(AppearanceDialog());
        stopTyping = false;
        dialogText.text = "";
        LevelManager.instance.SetCurrentLevelFromScene();
        string currentLevel = LevelManager.instance.currentSceneName;

        textToShow = "";

        if (isGreeting && greetingsTexts.ContainsKey(currentLevel))
        {
            textToShow = greetingsTexts[currentLevel];
        }
        else if (!isGreeting && farewellTexts.ContainsKey(currentLevel))
        {
            textToShow = farewellTexts[currentLevel];
        }

        foreach (char letter in textToShow)
        {

            if (stopTyping) // Проверяем, нужно ли остановить набор текста
            {
                yield break;
            }
            dialogText.text += letter;
            isTyping = true;
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.PitchSoundEffectClip, volume: 0.05f);
            yield return new WaitForSeconds(typingSpeed);
        }
        Debug.Log("Dialog text shown: " + textToShow);
        isWaitingForInput = true;
      //continueButton.gameObject.SetActive(true); // Показываем кнопку на мобильных
    }

    private void Update()
    {
        if (isProcessingInput) return;
        if (isWaitingForInput && Input.GetKeyDown(KeyCode.Space) && isGreeting)
        {
            isProcessingInput = true;
            StartCoroutine(HandleGreetingInput());

        }
        else if (isWaitingForInput && Input.GetKeyDown(KeyCode.Space) && !isGreeting)
        {
            isProcessingInput = true;
            StartCoroutine(HandleFarewellInput());

        }
        else if (isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            isTyping = false; // Остановим корутину на следующей итерации
            stopTyping = true; // Устанавливаем флаг остановки набора текста
            dialogText.text = textToShow; // Показать весь текст сразу
            isWaitingForInput = true;
            return;
        }
    }
    private IEnumerator HandleFarewellInput()
    {
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.PitchSoundEffectClip, volume: 0.05f);
        HideDialog();
        OnDialogClosed?.Invoke();
        Sounds.Instance.StopLoopEffect();

        // Ждем завершения анимаций
        yield return new WaitForSeconds(0.3f);

        if (BuffUIManager.Instance != null) 
            BuffUIManager.Instance.SetBuffIconsVisible(false);

        isProcessingInput = false; 
    }
    private IEnumerator HandleGreetingInput()
    {
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.PitchSoundEffectClip, volume: 0.05f);
        HideDialog();
        StartCoroutine(BlackHoleStart.instance.FadeOutBlackHole());

        // Ждем немного перед загрузкой уровня
        yield return new WaitForSeconds(0.5f);

        LevelManager.instance.StartLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 2);
        isGreeting = false;
        Sounds.Instance.StopLoopEffect();
        PauseManager.Instance.canBePaused = true;

        if (BuffUIManager.Instance != null)
            BuffUIManager.Instance.SetBuffIconsVisible(true);

        isProcessingInput = false; // ← Разблокируем вход
    }

    public void HideDialog()
    {
        if (isGreeting)
            StartCoroutine(FadeOutGreetDialog());
        else
            StartCoroutine(FadeOutEndDialog());
    }
    public void PortraitAnimation(bool MakeVisible)
    {
          if (Player.instance != null && 
            Player.instance.BorderPortrait != null && 
            Player.instance.Portrait != null) 
        
            if (MakeVisible)
            {
                Player.instance.Portrait.DOFade(1, 0.25f).SetEase(Ease.InOutCubic).SetLink(Player.instance.Portrait.gameObject, LinkBehaviour.KillOnDestroy);
            }
            else
            {
                Player.instance.Portrait.DOFade(0, 0.25f).SetEase(Ease.InOutCubic).SetLink(Player.instance.Portrait.gameObject, LinkBehaviour.KillOnDestroy);
            }
        
    }
    IEnumerator AppearanceDialog()
    {
        PortraitAnimation(false);
        yield return new WaitForSeconds(0.25f);
        float time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            dialogGroup.alpha = Mathf.Lerp(0, 1, time / 0.5f);
            yield return null;
        }
        dialogGroup.alpha = 1f;
    }

    IEnumerator FadeOutGreetDialog()
    {
        isTyping = false; // Остановим корутину на следующей итерации
        float time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            dialogGroup.alpha = Mathf.Lerp(1, 0, time / 0.5f);
            yield return null;
        }
        dialogGroup.alpha = 0f;
        isWaitingForInput = false;
        // continueButton.gameObject.SetActive(false);

        GameManager.Instance.SetBackgroundScrolling(true);
        Coins.gameObject.SetActive(true);

        Player.instance.SetPlayerControl(true, true, true, true, Vector2.zero);
        HealthUI.instance.SetupUIHealth();
        HealthUI.instance.UpdateHealthUI(Player.instance.currentHealth, Player.instance.maxHealth);
        if (HealthUI.instance != null)
            {
                HealthUI.instance.ShieldCheck(Player.instance.hasShield);
                HealthUI.instance.SetupUIShield();
                HealthUI.instance.UpdateShieldUI(Player.instance.currentShield);
            }
        if (BuffUIManager.Instance.activeBuffs != null && BuffUIManager.Instance.activeBuffs.Count > 0)
        {
            Debug.LogWarning("Updating buff icons: " + BuffUIManager.Instance.activeBuffs.Count);
            BuffUIManager.Instance.UpdateBuffIcons();
            BuffUIManager.Instance.SetBuffIconsVisible(true);
        }
        else
            Debug.LogWarning("No active buff icons to update.");


        Player.instance.RestoreShield();
        PortraitAnimation(true);
    }
    IEnumerator FadeOutEndDialog()
    {
        isTyping = false;
        float time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            dialogGroup.alpha = Mathf.Lerp(1, 0, time / 0.5f);
            yield return null;
        }

        dialogGroup.alpha = 0;
        isWaitingForInput = false;
        // continueButton.gameObject.SetActive(false);
        GameManager.Instance.SetBackgroundScrolling(false);
        PortraitAnimation(false);
        Coins.gameObject.SetActive(false);
        BuffUIManager.Instance.SetBuffIconsVisible(false);
        Player.instance.SetPlayerControl(false, false, false, false, Vector2.zero);
       }

}
