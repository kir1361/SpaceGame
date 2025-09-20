using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        GameOver();
        GameOverText();
        Invoke("MenuLoad", 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void MenuLoad()
    {
        SceneManager.LoadScene("MainMenu");
    }
    void GameOverText()
    {
        TextMeshProUGUI gameOverText = GameObject.Find("GameOver").GetComponent<TextMeshProUGUI>();
        gameOverText.DOFade(1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
    }
    void GameOver()
    {
        StopAllCoroutines();
        Sounds.Instance.StopAllMusic();
        Sounds.Instance.PlaySoundEffect(Sounds.Instance.GameOverMusic, volume: 0.2f);
        DOTween.KillAll();
        if (Player.IsPlayerDead) return;
        else
        {
            GameDataManager.instance.ResetSave();
            Player.IsPlayerDead = false;
        }

    }
}
