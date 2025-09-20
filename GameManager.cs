using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static Transform SpawnPoint;
    public BackgroundScroller[] backgrounds;

    public ShopTruckController ShopTruck;
    public bool blackHoleStarted = false;
    public int currentExpectedDigit; 
    public int playerFirstDigit = -1;
    public int playerSecondDigit = -1;
    public int playerAnswer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!blackHoleStarted && scene.name != "MainMenu" && scene.name != "Bootstrap" && scene.name != "GameOver")
        {
            Sounds.Instance.PlayMusic(Sounds.Instance.BackgroundMusic, volume: 0.05f);
            blackHoleStarted = true;
            StopAllCoroutines();
            StartCoroutine(DelayStartBlackHole());
            if (Player.IsPlayerDead == true)
            {
                Player.IsPlayerDead = false;
                return;
            }
            else
            {
                GameDataManager.instance.SaveGame();
                Player.IsPlayerDead = false;
            }    

        }
    }

    void Start()
    {
        SpawnPoint = this.transform;
        if (Player.instance != null)
        {
            Player.instance.gameObject.SetActive(false);
            //BlackHoleStart.instance.StartBlackHoleSequence();
        }

        //   InvokeRepeating(nameof(SpawnTruck), 5f, 60f);
    }

    IEnumerator DelayStartBlackHole()
    {
        yield return new WaitForSeconds(1f);
        BlackHoleStart.instance.StartBlackHoleSequence();
    }

    public void SpawnTruck()
    {
        if (Player.instance == null) return;
        Player.instance.rb.gravityScale = 0f;
        Player.instance.SetPlayerControl(true, true, false, false, Vector2.zero);
        SetBackgroundScrolling(false);
        ShopTruck.Arrive();

    }
    public void SetBackgroundScrolling(bool state)
    {
        foreach (var bg in backgrounds)
        {
            bg.SetScrolling(state);
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
   
}
