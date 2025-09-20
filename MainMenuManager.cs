using System.Collections;
using MONStudiosLLC.ButtonEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip BackgroundMusic;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button NoSavePanelButton;
    [SerializeField] public CanvasGroup NoSavePanelCanvasGroup;
    [SerializeField] public GameObject NoSavePanel;
    public static MainMenuManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        playButton.interactable = false;
        newGameButton.interactable = false;
        quitButton.interactable = false;
        NoSavePanel.SetActive(false);
        StartCoroutine(DelayForButtons());

        Transition transition = FindObjectOfType<Transition>();
        ButtonEffects buttonEffects = FindObjectOfType<ButtonEffects>();
        if (playButton == null || quitButton == null || newGameButton == null)
        {
            Debug.LogError("MainMenuManager: assign play/quit/newGame buttons in inspector!");
            return;
        }

        RectTransform playButtonRect = playButton.GetComponent<RectTransform>();
        RectTransform quitButtonRect = quitButton.GetComponent<RectTransform>();
        RectTransform newGameButtonRect = newGameButton.GetComponent<RectTransform>();
        RectTransform NoSavePanelButtonRect = NoSavePanelButton.GetComponent<RectTransform>();
        if (buttonEffects != null)
        {
            playButton.onClick.RemoveAllListeners();
            newGameButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
            NoSavePanelButton.onClick.RemoveAllListeners();

            playButton.onClick.AddListener(() => buttonEffects.Press19BounceDown(playButtonRect));
            quitButton.onClick.AddListener(() => buttonEffects.Press19BounceDown(quitButtonRect));
            newGameButton.onClick.AddListener(() => buttonEffects.Press19BounceDown(newGameButtonRect));
            NoSavePanelButton.onClick.AddListener(() => buttonEffects.Press19BounceDown(NoSavePanelButtonRect));
            NoSavePanelButton.onClick.AddListener(() =>
            {
                Sounds.Instance.PlaySoundEffect(Sounds.Instance.buttonHitSoundEffectClip);
                NoSavePanelCanvasGroup.DOFade(0f, 0.5f)
                .SetEase(Ease.InBack)
                .SetLink(NoSavePanelCanvasGroup.gameObject, LinkBehaviour.KillOnDisable).OnComplete(() =>
                {
                    NoSavePanel.SetActive(false);
                });
            }); 
        }
        else Debug.LogWarning("ButtonEffects not found in scene. Button effects will not be applied.");

        if (transition != null)
        {
            quitButton.onClick.AddListener(() => QuitGame());
            playButton.onClick.AddListener(() => StartGame(false)); // Resume
            newGameButton.onClick.AddListener(() => StartGame(true)); // New Game
        }
        Sounds.Instance?.PlayMusic(BackgroundMusic, volume: 0.05f);
    }
    IEnumerator DelayForButtons()
    {
        yield return new WaitForSeconds(1f);
        playButton.interactable = true;
        newGameButton.interactable = true;
        quitButton.interactable = true;
    }
    public void StartGame(bool isNewGame)
    {
        Sounds.Instance?.PlaySoundEffect(Sounds.Instance.buttonHitSoundEffectClip);

        if (isNewGame)
        {
            StartCoroutine(Sounds.Instance?.FadeOut(Sounds.Instance.musicSource));
            playButton.interactable = false;
            newGameButton.interactable = false;
            quitButton.interactable = false;
            GameDataManager.instance.ResetSave();
            StartCoroutine(Transition.Instance.StartIntroSequence());
        }
        else
        {
             if (!GameDataManager.instance.LoadGame()) return;
            StartCoroutine(Sounds.Instance?.FadeOut(Sounds.Instance.musicSource));
            playButton.interactable = false;
            newGameButton.interactable = false;
            quitButton.interactable = false;
            StartCoroutine(Transition.Instance.FadeIn());
            StartCoroutine(Transition.Instance.MyLoadSceneAsync(LevelManager.instance.currentSceneName));
        }
    }

    public void QuitGame()
    {
        playButton.interactable = false;
        newGameButton.interactable = false;
        quitButton.interactable = false;
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.buttonHitSoundEffectClip);
        Application.Quit();
    } 
}
