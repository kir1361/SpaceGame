using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;



public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public GameObject parcelPrefab;
    //public ParticleSystem blackHoleParticles;
    public Transform parcelSpawnPoint;
   // public Transform player;
    public Sprite[] parcelSprites;
    private string[] parcelNames = {"Green Parcel", "Red Parcel", "Yellow Parcel"};
    private string currentParcelName;
    public TextMeshProUGUI parcelText;
    public int fadeDuration;
    public CanvasGroup canvasGroup;
    private int enemiesAlive = 0;
    private int bossAlive = 0;

    public bool hasParcelSpawned = false;
    bool hasEnemiesSpawned = false;
    //public GameObject blackHolePrefab;
    [SerializeField] private GameObject newParcel;
    public Image Coins;
    public string currentSceneName;
    public GameObject BossTextComing;
    public GameObject MeteorTextPref;
    public Transform BossTextComingPoint;
    public Transform MeteorTextComingPoint;

    [System.Serializable]
    public class LevelScenario
    {
        public float initialDelay;
        public EventData[] events;
    }

    [System.Serializable]
    public class EventData
    {
        public EventType eventType;
        public float delay;
        public GameObject[] enemies;
        public Transform[] EnSpawnPoints;


        [Header("Boss Data")]
        public GameObject Boss;
        public Transform BossSpawnPoints;
    }

    public enum EventType
    {
        SpawnEnemies,
        Pause,
        SpawnMeteors,

        SpawnShopTruck,

        EndLevel,

        SpawnBoss
    }

    private Dictionary<string, Color> parcelColors = new Dictionary<string, Color>()
    {
        { "Green Parcel", Color.green },
        { "Red Parcel", Color.red },
        { "Yellow Parcel", Color.yellow }
    };

    public LevelScenario[] levels;

    [Header("Level Management")]
    public int currentLevelIndex = 0;
    public static readonly string[] levelSceneNames =
    {
        "LVL1",
        "LVL2",
        "GameOver"
    }; 
    private string _menuSceneName = "MainMenu";
    private bool hasBossSpawned;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
        StopAllCoroutines();

    }

    // void Start()
    // {
    //     SetCurrentLevelFromScene(); 
    // }
    void Update()
    {
        if (Player.IsPlayerDead)
        {
            StopAllCoroutines();
        }
    }
    public void SetCurrentLevelFromScene()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < levelSceneNames.Length; i++)
        {
            if (currentSceneName == levelSceneNames[i])
            {
                currentLevelIndex = i;
                break;
            }
        }

        if (currentLevelIndex >= levelSceneNames.Length)
        {
            Debug.LogError("Current level index is out of bounds: " + currentLevelIndex);
            currentLevelIndex = 0; 
        }
    }

    public void StartLevel(int level)
    {
        //Debug.Log("Level Start: " + level);
        GameManager.Instance.blackHoleStarted = false;
        hasEnemiesSpawned = false;
        if (hasEnemiesSpawned) return;
        hasEnemiesSpawned = true;
        hasParcelSpawned = false;

        SpawnParcel(level);
        ShowFloatingText();
        StartCoroutine(ExecuteLevelScenario(level));
    }

    IEnumerator ExecuteLevelScenario(int level)
    {
        if (level < 0 || level >= levels.Length)
        {
            Debug.LogError("Invalid level index: " + level);
            yield break;
        }

        LevelScenario scenario = levels[level];

        yield return new WaitForSeconds(scenario.initialDelay);

        foreach (EventData eventData in scenario.events)
        {
            if (Player.IsPlayerDead) yield break;
            yield return new WaitForSeconds(eventData.delay);
            
            switch (eventData.eventType)
            {

                case EventType.SpawnEnemies:
                    yield return StartCoroutine(SpawnEnemies(eventData));
                    break;

                case EventType.Pause:
                    yield return new WaitForSeconds(eventData.delay);
                    break;

                case EventType.SpawnMeteors:
                    yield return StartCoroutine(SpawnMeteors());
                    break;

                case EventType.EndLevel:
                    yield return StartCoroutine(EndLevelSequence());
                    break;

                case EventType.SpawnShopTruck:
                    yield return StartCoroutine(SpawnShopTruck());
                    break;

                case EventType.SpawnBoss:
                    yield return StartCoroutine(SpawnBoss(eventData));
                    break;
            }
        }
    }
    IEnumerator SpawnShopTruck()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.SpawnTruck();
        yield return new WaitUntil(() => ShopTruckController.instance.eventOver);

    }

    IEnumerator EndLevelSequence()
    {
        yield return new WaitForSeconds(1f);
        
        BlackHoleStart.instance.ExitBlackHoleSequence();

        DirDialogue.Instance.OnDialogClosed = () =>
        {
            StartCoroutine(PlayerToBlackHole());
        };
    }

    IEnumerator PlayerToBlackHole()
    {
        newParcel.transform.position = Player.instance.transform.position;
        yield return new WaitForSeconds(3f);

        if (newParcel != null)
        {
            newParcel.SetActive(true);
            newParcel.transform.DOMove(BlackHoleStart.instance.blackHoleStart_Exit.transform.position, 2f).SetEase(Ease.InBack);
            newParcel.transform.DOScale(Vector2.zero, 2f).SetEase(Ease.InQuad).OnComplete(() => Destroy(newParcel.gameObject));
        }
        yield return new WaitForSeconds(1f);
        Player.instance.transform.DOMove(BlackHoleStart.instance.blackHoleStart_Exit.transform.position, 2f).SetEase(Ease.InBack);
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.PlayerEnter_ExitBlackHole, volume: 0.05f);

        Player.instance.transform.DOScale(Vector2.zero, 2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            StartCoroutine(Transition.Instance.FadeIn());
        });
        yield return new WaitForSeconds(5f);
        Player.instance.gameObject.SetActive(false);
        //blackHoleParticles.gameObject.SetActive(false);
        Destroy(BlackHoleStart.instance.blackHoleStart_Exit);
        //BlackHoleStart.instance.blackHoleParticles.gameObject.SetActive(false);
        yield return StartCoroutine(LoadNextLevel());
    }

     IEnumerator LoadNextLevel()
    {
        string nextSceneName = GetNextLevelScene();

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            //yield return StartCoroutine(Transition.Instance.FadeIn());
            yield return StartCoroutine(Transition.Instance.MyLoadSceneAsync(nextSceneName));
            SetCurrentLevelFromScene();
        }
        else
        {
            //yield return StartCoroutine(Transition.Instance.FadeIn());
            yield return StartCoroutine(Transition.Instance.MyLoadSceneAsync(_menuSceneName));
        }
    }

    private string GetNextLevelScene()
    {
        int nextLevelIndex = currentLevelIndex + 1;
        if (nextLevelIndex < levelSceneNames.Length)
        {
            return levelSceneNames[nextLevelIndex];
        }
        else
        {
            return null; 
        }
    }

    // public bool IsLastLevel()
    // {
    //     return currentLevelIndex >= levelSceneNames.Length - 1;
    // }

    // public void RestartCurrentLevel()
    // {
    //     string currentScene = levelSceneNames[currentLevelIndex];
    //     StartCoroutine(Transition.Instance.LoadSceneRoutine(currentScene));
    // }

    IEnumerator SpawnMeteors()
    {
        if (Player.IsPlayerDead) yield break;
        yield return StartCoroutine(SpawnMeteorComing());
        MeteorSpawner meteorSpawner = FindObjectOfType<MeteorSpawner>();
        if(meteorSpawner == null && meteorSpawner.gameObject.activeInHierarchy) 
        {
            Debug.LogWarning("MeteorSpawner not available!");
            yield break;
        }   
        meteorSpawner.RepeatMeteor();
        yield return new WaitUntil(() => meteorSpawner.SpawnCount == meteorSpawner.spawnLimit);
    }
    IEnumerator SpawnMeteorComing()
    {
        if (Player.IsPlayerDead) yield break;

        GameObject _meteorText = Instantiate(MeteorTextPref, MeteorTextComingPoint);

        Vector2 offset = new Vector2(-60f, 0f);
        _meteorText.transform.localPosition += (Vector3)offset;

        TextMeshProUGUI _meteorTextUGUI = _meteorText.GetComponentInChildren<TextMeshProUGUI>();
        _meteorTextUGUI.DOFade(1f, 1f).OnComplete(() =>
        {
            _meteorTextUGUI.DOFade(0f, 1f).SetDelay(2f);
        });
        yield return new WaitForSeconds(5f);
        Destroy(_meteorText);
    }
    IEnumerator SpawnBossComing()
    {        
        if (Player.IsPlayerDead) yield break;
        GameObject bossText = Instantiate(BossTextComing, BossTextComingPoint);
        bossText.transform.localPosition = Vector3.zero;
        TextMeshProUGUI BossText = bossText.GetComponentInChildren<TextMeshProUGUI>();
        Sounds.Instance.PlayBossMusic(Sounds.Instance.bossMusicSound);
        BossText.DOFade(1f, 1f).OnComplete(() =>
        {
            BossText.DOFade(0f, 1f).SetDelay(2f);
        });
        yield return new WaitForSeconds(4f);
    }
    IEnumerator SpawnBoss(EventData eventData)
    {       
        if (Player.IsPlayerDead) yield break;
        yield return StartCoroutine(SpawnBossComing());
        bossAlive = 1;
        GameObject spawnedBoss = Instantiate(eventData.Boss, eventData.BossSpawnPoints.position, Quaternion.identity);
        spawnedBoss.GetComponent<BossBase>().OnBossDeath += BossDefeated;
        yield return new WaitForSeconds(1f);
        //yield return new WaitUntil(() => bossAlive == 0);
        yield return new WaitUntil(() => LaserPickup.eventOver == true);
        hasBossSpawned = true;
    }
    IEnumerator SpawnEnemies(EventData eventData)
    {
        if (Player.IsPlayerDead) yield break;

        enemiesAlive = eventData.enemies.Length;
        Sounds.Instance.PlayEnemyMusic(Sounds.Instance.enemyFightMusic);
        foreach (GameObject enemy in eventData.enemies)
        {
            Transform spawnPoint = eventData.EnSpawnPoints[Random.Range(0, eventData.EnSpawnPoints.Length)];
            GameObject spawnedEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
            spawnedEnemy.GetComponent<Enemy>().OnEnemyDeath += EnemyDefeated;
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitUntil(() => enemiesAlive == 0);
        hasEnemiesSpawned = true;
        StartCoroutine(Sounds.Instance.FadeOut(Sounds.Instance.enemyFightSource));
        Sounds.Instance.musicSource.time = Sounds.Instance.lastPlaybackTime; // Restore the last playback time
        Sounds.Instance.PlayMusic(Sounds.Instance.BackgroundMusic, volume: 0.05f);
    }
    public void BossDefeated()
    {
        bossAlive--;
        StartCoroutine(Sounds.Instance.FadeOut(Sounds.Instance.bossSource));
        Sounds.Instance.musicSource.time = Sounds.Instance.lastPlaybackTime; // Restore the last playback time
        Sounds.Instance.PlayMusic(Sounds.Instance.BackgroundMusic, volume: 0.05f);
    }
    public void EnemyDefeated()
    {
        enemiesAlive--;
    }

    private void SpawnParcel(int level)
    {
        if (hasParcelSpawned) return;
        if (level >= parcelNames.Length) level = parcelNames.Length - 2 ;
        currentParcelName = parcelNames[level];
        Sprite selectedSprite = parcelSprites[level];
        newParcel = Instantiate(parcelPrefab, parcelSpawnPoint.position, Quaternion.Euler(0, 0, 15f));
        hasParcelSpawned = true;
        Parcel parcel = newParcel.GetComponent<Parcel>();
        parcel.GetComponent<SpriteRenderer>().sprite = selectedSprite;

        parcel.Setup(Player.instance.transform);
    }
    public void ShowFloatingText()
    {
        if (Player.IsPlayerDead) return;

        parcelText.text = "Delivery level: " + currentParcelName;
        if (parcelColors.TryGetValue(currentParcelName, out Color color))
        {
            parcelText.color = color;
        }
        canvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                canvasGroup.DOFade(0f, fadeDuration).SetLink(canvasGroup.gameObject,LinkBehaviour.KillOnDestroy);
            });
        });
    }

}
