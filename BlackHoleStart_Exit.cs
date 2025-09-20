using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BlackHoleStart : MonoBehaviour
{
    public static BlackHoleStart instance;
    public GameObject blackHolePrefab;
    [HideInInspector] public GameObject blackHoleStart_Exit;
    public Transform blackHoleStartPoint;
    public Transform blackHoleExitPoint;
    [HideInInspector] public ParticleSystem ps;

    // public ParticleSystem blackHoleParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    }

    // Update is called once per frame

    public void StartBlackHoleSequence()
    {
        StopAllCoroutines();
        Player player = Player.instance;
        // GameManager.Manager.SetBackgroundScrolling(false);
        PauseManager.Instance.canBePaused = false;

        blackHoleStart_Exit = Instantiate(blackHolePrefab, blackHoleStartPoint.position, Quaternion.identity);
        Sounds.Instance.PlayLoopEffect(Sounds.Instance.BlackHoleSound, volume: 0.05f);

        SpriteRenderer sr = blackHoleStart_Exit.GetComponent<SpriteRenderer>();

        sr.DOColor(new Color(1f, 1f, 1f, 1.5f), 1f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine).SetLink(blackHoleStart_Exit, LinkBehaviour.KillOnDestroy);


        blackHoleStart_Exit.transform.DOScale(8f, 2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            blackHoleStart_Exit.transform.DOScale(8.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(blackHoleStart_Exit, LinkBehaviour.KillOnDestroy);
        });

       player.gameObject.SetActive(true);
       player.transform.position = blackHoleStart_Exit.transform.position;
       player.transform.localScale = Vector3.zero;
       if (HealthUI.instance.Shield != null)
       {
           HealthUI.instance.Shield.SetActive(false);
       }

        Sequence seq = DOTween.Sequence();
        seq.Append(player.transform.DOMove(gameObject.transform.position, 2f).SetEase(Ease.OutCubic));
        seq.Join(player.transform.DOScale(1f, 1f).SetEase(Ease.OutBack));

        seq.InsertCallback(0f,() => Player.instance.RestoreShield());

        Sounds.Instance.PlaySoundEffect(Sounds.Instance.PlayerEnter_ExitBlackHole, volume: 0.05f);
        seq.OnComplete(() =>
        {
            if (DirDialogue.Instance != null && DirDialogue.Instance.dialogGroup.gameObject != null)
            {
                DirDialogue.Instance.ShowDialog();
            }
        }).SetLink(player.gameObject, LinkBehaviour.KillOnDestroy);;

    }
    public void ExitBlackHoleSequence()
    {
        // if (blackHoleStart_Exit != null)
        //     Destroy(blackHoleStart_Exit);

        blackHoleStart_Exit = Instantiate(blackHolePrefab, blackHoleExitPoint.position, Quaternion.identity);
        Sounds.Instance.PlayLoopEffect(Sounds.Instance.BlackHoleSound,volume: 0.05f);

        SpriteRenderer sr = blackHoleStart_Exit.GetComponent<SpriteRenderer>();
        blackHoleStart_Exit.transform.localScale = Vector3.zero;
        if (blackHoleStart_Exit != null && blackHoleStart_Exit.gameObject.activeInHierarchy)
        {
            blackHoleStart_Exit.transform.DOScale(8f, 2f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                blackHoleStart_Exit.transform.DOScale(8.5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(blackHoleStart_Exit, LinkBehaviour.KillOnDestroy);
                if (DirDialogue.Instance != null && DirDialogue.Instance.dialogGroup.gameObject != null)
                {
                    DirDialogue.Instance.ShowDialog();
                }    
            });
        }
        sr.DOColor(new Color(1f, 1f, 1f, 1f), 1f)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine).SetLink(blackHoleStart_Exit, LinkBehaviour.KillOnDestroy);

        ps = blackHoleStart_Exit.GetComponentInChildren<ParticleSystem>();
        ps.transform.localPosition = Vector3.zero;
        ps.Play();

    }
    public IEnumerator FadeOutBlackHole()
    {
        float time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            blackHoleStart_Exit.GetComponent<SpriteRenderer>().color = Color.Lerp(blackHoleStart_Exit.GetComponent<SpriteRenderer>().color, new Color(1f, 1f, 1f, 0f), time / 0.5f);
            yield return null;
        }
        Destroy(blackHoleStart_Exit);
        //ps.Stop();
    }

   
    

}


