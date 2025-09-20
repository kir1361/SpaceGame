using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameSaveData
{
    public string CurrentLevelSceneName;
    public bool hasShield;
    public bool hasLaser;
    public bool isActiveDashBuff;
    public bool isActiveCoin2xBuff;
    public int currentHealth;
    public int currentShield;
    public int currentCoins;
}
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    void Awake()
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
    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.CurrentLevelSceneName = SceneManager.GetActiveScene().name;
        saveData.hasShield = Player.instance.hasShield;
        saveData.hasLaser = Player.instance.hasLaser;
        saveData.isActiveCoin2xBuff = Player.instance.isActiveCoin2xBuff;
        saveData.isActiveDashBuff = Player.instance.isActiveDashBuff;
        saveData.currentHealth = Player.instance.currentHealth;
        saveData.currentShield = Player.instance.currentShield;

        saveData.currentCoins = Player.instance.currentCoins;

        string jsonData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("GameSave", jsonData);
        PlayerPrefs.Save();
    }
    public bool LoadGame()
    {
        if (PlayerPrefs.HasKey("GameSave"))
        {
            string jsonData = PlayerPrefs.GetString("GameSave");
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);

            if (saveData == null)
            {
                Debug.LogError("LoadGame: Player.instance is null!");
                return false;
            }
            // Восстанавливаем состояние
             if (LevelManager.instance != null) LevelManager.instance.currentSceneName = saveData.CurrentLevelSceneName;


            if (Player.instance != null)
            {
                Player.instance.hasShield = saveData.hasShield;
                Player.instance.hasLaser = saveData.hasLaser;
                Player.instance.isActiveCoin2xBuff = saveData.isActiveCoin2xBuff;
                Player.instance.isActiveDashBuff = saveData.isActiveDashBuff;
                Player.instance.currentHealth = saveData.currentHealth;
                Player.instance.currentCoins = saveData.currentCoins;
                Player.instance.currentShield = saveData.currentShield;
            }
            // if (HealthUI.instance != null)
            // {
            //     HealthUI.instance.UpdateHealthUI(saveData.currentHealth, Player.instance.maxHealth);
            //     if (saveData.hasShield && HealthUI.instance != null)
            //     {
            //         HealthUI.instance.SetupUIShield(Player.instance.maxShield);
            //         HealthUI.instance.UpdateShieldUI(Player.instance.currentShield);
            //     }
            // }
            if (BuffUIManager.Instance != null)
            {
                if (saveData.hasShield)
                    BuffUIManager.Instance.AddBuffIcon(Items.ItemType.Shield);
                if (saveData.isActiveCoin2xBuff)
                    BuffUIManager.Instance.AddBuffIcon(Items.ItemType.Coin2X);
                if (saveData.isActiveDashBuff)
                    BuffUIManager.Instance.AddBuffIcon(Items.ItemType.Dash);
            }
        }
        else
        {
            Sounds.Instance.PlaySoundEffect(Sounds.Instance.noSaveSoundEffectClip, volume: 0.15f, p1: 1.5f, p2: 1.5f);
            MainMenuManager.Instance.NoSavePanel.SetActive(true);
            MainMenuManager.Instance.NoSavePanelCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutBack).SetLink(MainMenuManager.Instance.NoSavePanel, LinkBehaviour.KillOnDisable);//Show the NoSavePanel if scene name is empty 
            return false;
        }
        Debug.Log("Game loaded: " + LevelManager.instance.currentSceneName);
        return true;
    }
    //    public void SaveGame()
    //    {
    //         Player player = Player.instance;
    //         if (LevelManager.instance == null)
    //         {
    //             Debug.LogError("SaveGame: LevelManager.instance is null!");
    //             return;
    //         }

    //         PlayerPrefs.SetString("CurrentLevelSceneName", SceneManager.GetActiveScene().name);

    //     if (player != null)
    //     {
    //         PlayerPrefs.SetInt("CurrentCoins", player.currentCoins);
    //         PlayerPrefs.SetInt("CurrentHealth", player.currentHealth);
    //         PlayerPrefs.SetInt("HasShield", player.hasShield ? 1 : 0);
    //         PlayerPrefs.SetInt("HasLaser", player.hasLaser ? 1 : 0);
    //         PlayerPrefs.SetInt("IsActiveCoin2xBuff", player.isActiveCoin2xBuff ? 1 : 0);
    //         }
    //     else
    //     {
    //         Debug.LogWarning("SaveGame: player is null, player values not saved.");
    //     }
    //     PlayerPrefs.Save();
    // }
    // public bool LoadGame()
    // {
    //     if (PlayerPrefs.HasKey("CurrentLevelSceneName"))
    //     {
    //         Player player = Player.instance;
    //         LevelManager.instance.currentSceneName = PlayerPrefs.GetString("CurrentLevelSceneName", "LVL1");
    //         //Player player = Player.instance;
    //         if (player != null)
    //         {
    //             //player.gameObject.SetActive(true);
    //             player.currentCoins = PlayerPrefs.GetInt("CurrentCoins", 0);
    //             player.currentHealth = PlayerPrefs.GetInt("CurrentHealth", 30);
    //             player.hasShield = PlayerPrefs.GetInt("HasShield", 0) == 1;
    //             player.hasLaser = PlayerPrefs.GetInt("HasLaser", 0) == 1;
    //             player.isActiveCoin2xBuff = PlayerPrefs.GetInt("IsActiveCoin2xBuff", 0) == 1;
    //             Debug.Log("Player health: " + player.currentHealth);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("LoadGame: player is null, cannot apply loaded values to player.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No saved game data found.");
    //         Sounds.Instance.PlaySoundEffect(Sounds.Instance.noSaveSoundEffectClip, volume: 0.15f, p1: 1.5f, p2: 1.5f);
    //         MainMenuManager.Instance.NoSavePanel.SetActive(true);
    //         MainMenuManager.Instance.NoSavePanelCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutBack).SetLink(MainMenuManager.Instance.NoSavePanel,LinkBehaviour.KillOnDisable);//Show the NoSavePanel if scene name is empty 
    //         return false;
    //     }
    //     Debug.Log("Game loaded: " + LevelManager.instance.currentSceneName);
    //     return true;
    // }

    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); 
        ResetPlayerToDefaults();
    }
     public void ResetPlayerToDefaults()
    {
        if (Player.instance != null)
        {
            Player.instance.hasShield = false;
            Player.instance.hasLaser = false;
            Player.instance.isActiveCoin2xBuff = false;
            Player.instance.isActiveDashBuff = false;
            Player.instance.currentHealth = Player.instance.maxHealth; // 30
            Player.instance.currentShield = 0;
            Player.instance.currentCoins = 0;
            BuffUIManager.Instance.activeBuffIcons.Clear();
            BuffUIManager.Instance.activeBuffs.Clear();
            if (Player.instance.Shield != null)
            {
                Player.instance.Shield.SetActive(false);
            }

            if (Player.instance.CoinsText != null)
            {
                Player.instance.CoinsText.text = "0";
            }

            // Обновляем HealthUI
            if (HealthUI.instance != null)
            {
                HealthUI.instance.UpdateHealthUI(Player.instance.currentHealth, Player.instance.maxHealth);
                HealthUI.instance.ShieldCheck(false);
            }

            Debug.Log("Player reset to default values");
        }
    }

}
