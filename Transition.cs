using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using DG.Tweening;

public class Transition : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public static Transition Instance;
    public float fadeDuration = 1.25f;
    public TextMeshProUGUI introText;
    private string fullText = "in a universe where delivering valuable cargo has become increasingly dangerous, special militarized intergalactic delivery companies have been established to handle these high-stakes shipments. You are one such courier,tasked with delivering every parcel to its destination, no matter the cost.";
    public float typingSpeed = 0.5f;
    AsyncOperation asyncLoad;
    private bool gameStarted = false;

    private bool isWaitingForInput = false;
    private bool isTyping = false;
    private bool stopTyping = false;



    void Awake()
    {
        canvasGroup.blocksRaycasts = false;
        isTyping = false;
        stopTyping = false;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//transform.root.
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        //gameObject.SetActive(true);
        introText.gameObject.SetActive(false);
        StartCoroutine(FadeOut());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                stopTyping = true;
            }
            else if (asyncLoad != null && asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
        }
    }
    IEnumerator FadeOut()
    {
        canvasGroup.alpha = 1;
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    public IEnumerator FadeIn()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 0;
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
    public IEnumerator TypeText()
    {
        introText.text = " ";

        foreach (char letter in fullText)
        {
            if (stopTyping) 
            {
                introText.text = fullText;
                yield break;
            }

            introText.text += letter;
            isTyping = true;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
    public IEnumerator StartIntroSequence()
    {
        stopTyping = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        yield return new WaitForSeconds(1f);
        introText.gameObject.SetActive(true);

        asyncLoad = SceneManager.LoadSceneAsync("LVL1", LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        yield return StartCoroutine(TypeText());

        yield return new WaitForSeconds(2.5f);

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        if (stopTyping)
        {
             while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null;
            }
        }
       
        asyncLoad.allowSceneActivation = true;
        yield return StartCoroutine(LoadStartSceneRoutine());
    }
        public IEnumerator MyLoadSceneAsync(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty or null!");
            yield break;
        }
        asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
        yield return StartCoroutine(FadeOut());
    }
    public IEnumerator LoadStartSceneRoutine()
    {
        float timeText = 0;
        Color color = introText.color;
        while (timeText < 1f)
        {
            timeText += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timeText / 0.35f);
            introText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        introText.color = new Color(color.r, color.g, color.b, 0f);
        introText.gameObject.SetActive(false);

        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / 0.35f);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

   
    
}

