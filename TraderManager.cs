using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class TraderManager : MonoBehaviour
{
    public static TraderManager instance;
    public CanvasGroup DialogGroup;
    public Image CoinsImage;
    public float TypingSpeed;
    public TextMeshProUGUI DialogText;
    private bool _isWaitingForInput = false;
    private bool _stopTyping = false;
    private string currentText = "";
    public bool isDiscountText = true;
    private static readonly IReadOnlyList<string> _textToShow = new[]
    {
        "Ah, traveler! Fortune smiles upon you today! My prices have taken a most agreeable plunge. But such a discount is not simply given, it must be earned! Solve a simple riddle of numbers for me",
        "Press E to claim your well-earned discount!"
    };

    private bool _isTyping;
    public Transform TaskPosition;
    [SerializeField] private GameObject _drawingAreaPref;
    public Transform DrawingAreaPos;
    public bool WasTraderInvoked = false;
    [SerializeField] private GameObject _taskMessagePrefab;
    [SerializeField] private CanvasGroup _canvasGroupTask;
    [SerializeField] private GameObject _textForCountTask;
    [SerializeField] private GameObject _textTaskPrefab;
    private GameObject _textTask;
    private Transform _taskTextContainer;
    private GameObject _taskMessage;
    private string _textForTask;
    public TextMeshProUGUI TaskText;
    public Vector2 _originalTextPosition;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
    }
    void Start()
    {
        DrawingSystem.OnReminderNeeded += ShowTraderDialog;
    }
    // Update is called once per frame
    void Update()
    {

        if (_isWaitingForInput && Input.GetKeyDown(KeyCode.Space) && isDiscountText)
        {
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.PitchSoundEffectClip, volume: 0.05f);
            FadeOutTraderDialog();
            GettingInterference.Instance.TaskCount = 0;
            PauseManager.Instance.canBePaused = true;
            _textForTask = MathGameManager.Instance.GenerateNewProblem();
            GameObject da = Instantiate(_drawingAreaPref, DrawingAreaPos.position, Quaternion.identity);
            ClearDrawingArea();
            //DrawingSystem daDrawingSystem = da.GetComponent<DrawingSystem>();
            da.SetActive(true);
            da.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetLink(da, LinkBehaviour.KillOnDestroy);
            ShowTask();
        }
        else if (_isWaitingForInput && Input.GetKeyDown(KeyCode.Space) && !isDiscountText)
        {
            PauseManager.Instance.canBePaused = true;
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.PitchSoundEffectClip, volume: 0.05f);
            FadeOutTraderDialog();
        }
        else if (_isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            PauseManager.Instance.canBePaused = false;
            _isTyping = false; // Остановим корутину на следующей итерации
            _stopTyping = true; // Устанавливаем флаг остановки набора текста
            DialogText.text = currentText; // Показать весь текст сразу
            _isWaitingForInput = true;
            return;
        }
    }
    void ClearDrawingArea()
    {
        if (DrawingSystem.instance != null)
        {
            Texture2D d1t = DrawingSystem.instance.digit1Texture;
            Texture2D d2t = DrawingSystem.instance.digit2Texture;
            DrawingSystem.instance.ClearDrawingTexture(d1t);
            DrawingSystem.instance.ClearDrawingTexture(d2t);
        }
    }
    public void Init()
    {
        DialogGroup.alpha = 0;
        CoinsImage.gameObject.SetActive(false);
        _stopTyping = false;
        StopAllCoroutines();
    }
    public void ShowTraderDialog()
    {
        PauseManager.Instance.canBePaused = false;
        _stopTyping = false;
        WasTraderInvoked = true;
        if (HealthUI.instance.Heart != null)
        {
            HealthUI.instance.Heart.SetActive(false);
        }
        if (HealthUI.instance.Shield != null)
        {
            HealthUI.instance.Shield.SetActive(false);
        }
        CoinsImage.gameObject.SetActive(false);
        GameManager.Instance.SetBackgroundScrolling(false);
        BuffUIManager.Instance.SetBuffIconsVisible(false);
        Player.instance.SetPlayerControl(false, false, false, false, Vector2.zero);
        StartCoroutine(ShowAndTypeText());
    }
    IEnumerator ShowAndTypeText()
    {
        AppearanceTraderDialog();
        DialogText.text = "";
        currentText = isDiscountText ? _textToShow[0] : _textToShow[1];
            Debug.Log("Starting to type: " + currentText);

        foreach (char letter in currentText)
        {
            if (_stopTyping) // Проверяем, нужно ли остановить набор текста
            {
                yield break;
            }
            DialogText.text += letter;
            _isTyping = true;
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.PitchSoundEffectClip, volume: 0.05f);
            yield return new WaitForSeconds(TypingSpeed);
        }
        Debug.Log("Dialog text shown: " + _textToShow);
        _isWaitingForInput = true;
        //continueButton.gameObject.SetActive(true); // Показываем кнопку на мобильных
    }
    public void AppearanceTraderDialog()
    {
        DirDialogue.Instance.PortraitAnimation(false);
        Player.instance.SetPlayerControl(false, false, false, false, Vector2.zero);
        DialogGroup.DOFade(1, 0.5f);
    }

    public void FadeOutTraderDialog()
    {
        isDiscountText = false;
        _isTyping = false;
        _isWaitingForInput = false;

        DialogGroup.DOFade(0f, 0.5f).OnComplete(() =>
        {
            // Весь этот код выполнится после завершения анимации
            Player.instance.SetPlayerControl(true, false, false, false, Vector2.zero);
            DirDialogue.Instance.PortraitAnimation(true);
            CoinsImage.gameObject.SetActive(true);
            BuffUIManager.Instance.SetBuffIconsVisible(true);

            if (HealthUI.instance.Heart != null)
            {
                HealthUI.instance.Heart.SetActive(true);
            }
            if (HealthUI.instance.Shield != null)
            {
                HealthUI.instance.Shield.SetActive(true);
            }
        });
    }
    public void ApplyTraderDiscountToAllItems(int discountPercent)
    {
        Items[] allItems = FindObjectsOfType<Items>();
        foreach (Items item in allItems)
        {
            item.ApplyDiscount(discountPercent);
        }
    }
    public void ResetTraderDiscountToAllItems()
    {
        Items[] allItems = FindObjectsOfType<Items>();
        foreach (Items item in allItems)
        {
            item.ResetDiscount();
        }
    }
    public void ShowTask()
    {
        Canvas canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        _taskMessage = Instantiate(_taskMessagePrefab, canvas.transform);
        _taskMessage.transform.SetSiblingIndex(8); // Устанавливаем нужный порядок в иерархии

        Vector3 pos = TaskPosition.transform.position;

        _taskMessage.transform.position = pos;

        _canvasGroupTask = _taskMessage.GetComponent<CanvasGroup>();
        _canvasGroupTask.DOFade(1f, 0.5f).SetEase(Ease.OutBack).SetLink(_taskMessage, LinkBehaviour.KillOnDestroy);
        _taskTextContainer = _taskMessage.transform.Find("TextTaskContainer");
        //_countTaskContainer = TaskMessage.transform.Find("CountTaskContainer");

        //canvasGroupTooltip.DOFade(1f, 0.5f).SetEase(Ease.OutBack);
        if (_textTask == null)
        {
            _textTask = Instantiate(_textTaskPrefab, _taskTextContainer.transform);
            _textTask.transform.localPosition = Vector3.zero;
            _textTask.transform.localScale = Vector3.zero;
            TaskText = _textTask.GetComponent<TextMeshProUGUI>();
            //priceText.text = Price.ToString();
            TaskText.text = _textForTask;
            _textTask.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            _originalTextPosition = TaskText.transform.position;
        }
    }
    public void FadeOutTask()
    {
        if (_canvasGroupTask != null && _taskMessage != null)
        {
            _canvasGroupTask.DOFade(0, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                if (_taskMessage != null)
                {
                    Destroy(_taskMessage);
                }
            })
            .SetLink(_taskMessage, LinkBehaviour.KillOnDestroy);
        }
    }
    void OnDestroy()
    {
        DrawingSystem.OnReminderNeeded -= ShowTraderDialog;
    }
}
