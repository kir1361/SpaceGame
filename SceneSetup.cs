using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static LevelManager;
using Image = UnityEngine.UI.Image;
public class SceneSetup : MonoBehaviour
{
    public static SceneSetup instance;
    [Header("LevelManager References")]
    [SerializeField] private Transform _parcelSpawnPoint;
    [SerializeField] private Transform[] _enemySpawnPoints;
    [SerializeField] private Transform _bossSpawnPoint;

    [Header("GameManager References")]
    [SerializeField] private BackgroundScroller[] _backgrounds;
    [SerializeField] private ShopTruckController _shopTruck;

    [Header("HealthUI References")]
    [SerializeField] private Transform _healthContainer;
    [SerializeField] private Transform _bossHealthContainer;
    [SerializeField] private Transform _shieldContainer;

    [Header("BossInfo References")]

    [Header("BlackHoleStart/Exit References")]
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _exitPoint;

    [SerializeField] private GameObject _blackHolePrefab;


    [Header("Scene UI References")]
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private Image _coinsImage;
    [SerializeField] private Canvas _sceneCanvas;
    [SerializeField] private TextMeshProUGUI _parcelText;
    [SerializeField] private CanvasGroup _canvasGroupFloatingText;
    [SerializeField] private Transform _buffUIManagerContainer;
    [SerializeField] private Image _borderPortrait;
    [SerializeField] private CanvasGroup _portrait;
    [SerializeField] private GameObject _bossTextComingPref;
    [SerializeField] private GameObject _meteorTextPref;
    [SerializeField] private Transform _textComingPoint;
    [SerializeField] private GameObject _pauseMenuUi;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("DirDialogue")]
    [SerializeField] private TextMeshProUGUI _textDir;
    [SerializeField] private CanvasGroup _canvasGroupDirDialogue;

    [Header("TraderDialogue")]

    [SerializeField] private TextMeshProUGUI _textTrader;
    [SerializeField] private CanvasGroup _canvasGroupTraderDialogue;
    [SerializeField] private Transform _drawingAreaPosition;
    [SerializeField] private Transform _taskPosition;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
        StartCoroutine(WaitForAllManagers());
    }
    IEnumerator WaitForAllManagers()
    {
        while (GameManager.Instance == null || LevelManager.instance == null || BlackHoleStart.instance == null || DirDialogue.Instance == null || HealthUI.instance == null || Player.instance == null || BuffUIManager.Instance == null || Sounds.Instance == null || PauseManager.Instance == null || TraderManager.instance == null)
            yield return null;

        SetupAllManagers();
    }

    private void SetupAllManagers()
    {
        SetupLevelManager();
        SetupGameManager();
        SetupBlackHole();
        SetupDirDialogue();
        SetupTraderManager();
        DirDialogue.Instance.Init();
        TraderManager.instance.Init();
        SetupUI();
        SetupBuffUIManager();
        SetupPauseManager();
        PauseManager.Instance.Init();
    }

    private void SetupLevelManager()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.parcelSpawnPoint = _parcelSpawnPoint;
            LevelManager.instance.canvasGroup = _canvasGroupFloatingText;

            LevelManager.instance.Coins = _coinsImage;
            LevelManager.instance.parcelText = _parcelText;

            // LVL1
            // LevelManager.EventData eventData = LevelManager.instance.levels[0].events[0];
            // eventData.BossSpawnPoints = bossSpawnPoint;

            foreach (LevelScenario level in LevelManager.instance.levels)
            {
                foreach (EventData eventData in level.events)
                {
                    if (eventData.eventType == LevelManager.EventType.SpawnEnemies && _enemySpawnPoints.Length > 0)
                    {
                        eventData.EnSpawnPoints = _enemySpawnPoints;
                    }

                    if (eventData.eventType == LevelManager.EventType.SpawnBoss && eventData.Boss != null)
                    {
                        eventData.BossSpawnPoints = _bossSpawnPoint;
                    }
                }
            }
        }
    }
    private void SetupBlackHole()
    {
        BlackHoleStart.instance.blackHolePrefab = _blackHolePrefab;
        BlackHoleStart.instance.blackHoleStartPoint = _startPoint;
        BlackHoleStart.instance.blackHoleExitPoint = _exitPoint;
    }

    private void SetupDirDialogue()
    {
        DirDialogue.Instance.dialogText = _textDir;
        DirDialogue.Instance.dialogGroup = _canvasGroupDirDialogue;
        DirDialogue.Instance.Coins = _coinsImage;
    }
    private void SetupUI()
    {
        HealthUI.instance.healthContainer = _healthContainer;
        HealthUI.instance.bossHealthContainer = _bossHealthContainer;

        HealthUI.instance.shieldContainer = _shieldContainer;
        Player.instance.CoinsText = _coinsText;
        Player.instance.BorderPortrait = _borderPortrait;
        Player.instance.Portrait = _portrait;

        LevelManager.instance.BossTextComing = _bossTextComingPref;
        LevelManager.instance.BossTextComingPoint = _textComingPoint;
        LevelManager.instance.MeteorTextPref = _meteorTextPref;
        LevelManager.instance.MeteorTextComingPoint = _textComingPoint;

    }
    private void SetupBuffUIManager()
    {
        BuffUIManager.Instance.buffContainer = _buffUIManagerContainer;
    }
    private void SetupGameManager()
    {
        GameManager.Instance.backgrounds = _backgrounds;
        GameManager.Instance.ShopTruck = _shopTruck;
    }
    private void SetupPauseManager()
    {
        PauseManager.Instance.pauseMenuUI = _pauseMenuUi;
        PauseManager.Instance.resumeButton = _resumeButton;
        PauseManager.Instance.mainMenuButton = _mainMenuButton;
    }
    private void SetupTraderManager()
    {
        TraderManager.instance.DialogGroup = _canvasGroupTraderDialogue;
        TraderManager.instance.CoinsImage = _coinsImage;
        TraderManager.instance.DialogText = _textTrader;
        TraderManager.instance.DrawingAreaPos = _drawingAreaPosition;
        TraderManager.instance.TaskPosition = _taskPosition;
    }

    // В SceneSetup:
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
          if (this == null || !gameObject.activeInHierarchy) 
        return;
    
        if (instance == null)
        {
            return;
        }
       
        // При каждой загрузке новой сцены (кроме меню) переинициализируем
        if (scene.name != "MainMenu" && scene.name != "GameOver")
        {
            Debug.Log("Re-initializing managers for scene: " + scene.name);
            StartCoroutine(WaitForAllManagers());
        }
    }

}
