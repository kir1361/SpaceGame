using System;
using MONStudiosLLC.ButtonEffects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour
{
    public string sceneToLoad = "LVL1"; 

    [Header("Менеджеры")]
    public GameObject gameManagerPrefab;
    public GameObject levelManagerPrefab;
    public GameObject playerPrefab;
    public GameObject dirDialoguePrefab;
    public GameObject blackHoleStExPrefab;
    public GameObject healthUIPrefab;
    public GameObject buffUiManager;
    public GameObject soundsManager;
    public GameObject PauseManagerPrefab;
    public GameObject GameDataManagerPrefab;
    public GameObject ScriptAnimationPrefab;
    public GameObject MathGameManagerPrefab;
    public GameObject TraderManagerPrefab;
    //public GameObject DrawingSystemPrefab;

    private void Awake()
    {
        if (FindObjectOfType<GameManager>() == null)
        {
            var gm = Instantiate(gameManagerPrefab);
            DontDestroyOnLoad(gm);
        }
        if (FindObjectOfType<LevelManager>() == null)
        {
            var lm = Instantiate(levelManagerPrefab);
            DontDestroyOnLoad(lm);
        }
        if (FindObjectOfType<Player>() == null)
        {
            var p = Instantiate(playerPrefab);
            DontDestroyOnLoad(p);
        }
        if (FindObjectOfType<DirDialogue>() == null)
        {
            var dd = Instantiate(dirDialoguePrefab);
            DontDestroyOnLoad(dd);
        }
        if (FindObjectOfType<BlackHoleStart>() == null)
        {
            var bh = Instantiate(blackHoleStExPrefab);
            DontDestroyOnLoad(bh);
        }
        if (FindObjectOfType<HealthUI>() == null)
        {
            var hu = Instantiate(healthUIPrefab);
            DontDestroyOnLoad(hu);
        }
        if (FindObjectOfType<BuffUIManager>() == null)
        {
            var bm = Instantiate(buffUiManager);
            DontDestroyOnLoad(bm);
        }
        if (FindObjectOfType<Sounds>() == null)
        {
            var ss = Instantiate(soundsManager);
            DontDestroyOnLoad(ss);
        }
        if (FindObjectOfType<PauseManager>() == null)
        {
            var pm = Instantiate(PauseManagerPrefab);
            DontDestroyOnLoad(pm);
        }
        if (FindObjectOfType<GameDataManager>() == null)
        {
            var gmd = Instantiate(GameDataManagerPrefab);
            DontDestroyOnLoad(gmd);
        }
        if (FindObjectOfType<ButtonEffects>() == null)
        {
            var be = Instantiate(ScriptAnimationPrefab);
            DontDestroyOnLoad(be);
        }
        if (FindObjectOfType<MathGameManager>() == null)
        {
            var mgm = Instantiate(MathGameManagerPrefab);
            DontDestroyOnLoad(mgm);
        }
        if (FindObjectOfType<TraderManager>() == null)
        {
            var tm = Instantiate(TraderManagerPrefab);
            DontDestroyOnLoad(tm);
        }
        // if (FindObjectOfType<DrawingSystem>() == null)
        // {
        //     var ds = Instantiate(DrawingSystemPrefab);
        //     DontDestroyOnLoad(ds);
        // }
        SceneManager.LoadScene(sceneToLoad);
    }
}
