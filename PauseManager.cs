using MONStudiosLLC.ButtonEffects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public static PauseManager Instance;

    private bool isPaused = false;

    public Button resumeButton;
    public Button mainMenuButton;
    ButtonEffects buttonEffects;
    public bool canBePaused = false;

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
    
    
    public void Init()
    {
        // resumeButton.onClick.RemoveAllListeners();
        // mainMenuButton.onClick.RemoveAllListeners();

        buttonEffects = FindObjectOfType<ButtonEffects>();
        RectTransform resumeButtonRect = resumeButton.GetComponent<RectTransform>();
        RectTransform mainMenuButtonRect = mainMenuButton.GetComponent<RectTransform>();

        resumeButton.onClick.AddListener(() => Resume());
        mainMenuButton.onClick.AddListener(() => LoadMainMenu());
        resumeButton.onClick.AddListener(() =>
        {
            buttonEffects.Press19BounceDown(resumeButtonRect);
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.buttonHitSoundEffectClip);
        }
        );
        mainMenuButton.onClick.AddListener(() => buttonEffects.Press19BounceDown(mainMenuButtonRect));
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canBePaused)
        {
            if(pauseMenuUI == null)
            {
                return;
            }
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if(pauseMenuUI == null)
        {
            return;
        }
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        if(pauseMenuUI == null)
        {
            return;
        }
        pauseMenuUI.SetActive(true);
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.pauseSoundEffectClip, volume: 0.15f);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        if (Player.instance != null && LevelManager.instance != null && DirDialogue.Instance != null)
        {
            DirDialogue.Instance.stopTyping = true;
            Player.instance.gameObject.SetActive(false);
            Time.timeScale = 1f;
            LevelManager.instance.StopAllCoroutines();
            Sounds.Instance.StopAllMusic();
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    // public void QuitGame()
    // {
    //     Application.Quit();
    // }
}

