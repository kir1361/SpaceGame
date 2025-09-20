using UnityEngine;
using System.Collections;
using Unity.Barracuda;
using System.Linq;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;
using DG.Tweening;


public class GettingInterference : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public Texture2D inputTexture;
    public static GettingInterference Instance;
    public NNModel modelAsset;
    private Model runtimeModel;
    private IWorker worker;
    public int TaskCount = 0;


    [Serializable]
    public struct Prediction
    {
        public int predictedValue;
        public float[] predicted;
        public void SetPrediction(Tensor t)
        {
            predicted = t.AsFloats();
            predictedValue = Array.IndexOf(predicted, predicted.Max());
        }
    }
    public Prediction prediction;
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
        runtimeModel = ModelLoader.Load(modelAsset);
        var device = SystemInfo.supportsComputeShaders ? WorkerFactory.Device.GPU : WorkerFactory.Device.CPU;
        worker = WorkerFactory.CreateWorker(runtimeModel, device);
        prediction = new Prediction();
    }

    // Update is called once per frame
    void Update()
    {
        DoInterfarence();
    }
    public void IncrementTaskCount()
    {
        TaskCount++;

        if (TaskCount >= 3)
        {
            DestroyTask_DrawingSystem();
        }
    }
    public void DestroyTask_DrawingSystem()
    {

        if (DrawingSystem.instance == null) return;
        DG.Tweening.Sequence seq = DOTween.Sequence();
                    seq.Append(DrawingSystem.instance.gameObject.transform.DOScale(0.1f, 0.5f).SetEase(Ease.InBack));
                    seq.InsertCallback(0f, () => TraderManager.instance.FadeOutTask());//1.5f
                    seq.OnComplete(() => {
                        if (DrawingSystem.instance != null)
                        {
                            Destroy(DrawingSystem.instance.gameObject);
                        }
                    }).SetLink(DrawingSystem.instance.gameObject, LinkBehaviour.KillOnDestroy);
    }
    bool IsTextureEmpty(Texture2D texture)
    {
        if (texture == null) return true;

        Color[] pixels = texture.GetPixels();
        foreach (Color pixel in pixels)
        {
            // Если хотя бы один пиксель не черный - текстура не пустая
            if (pixel.r > 0.1f || pixel.g > 0.1f || pixel.b > 0.1f)
            {
                return false;
            }
        }
        return true;
    }
    public void DoInterfarence()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (DrawingSystem.instance == null) return;
            Texture2D d1t = DrawingSystem.instance.digit1Texture;
            Texture2D d2t = DrawingSystem.instance.digit2Texture;
            if (IsTextureEmpty(d1t) || IsTextureEmpty(d2t))
            {
                return;
            }
            int firstDigit = RecognizeDigit(d1t);
            int secondDigit = RecognizeDigit(d2t);

            if (d1t != null && d2t != null)
            {
                DrawingSystem.instance.ClearDrawingTexture(d1t);
                DrawingSystem.instance.ClearDrawingTexture(d2t);
            }
            else
            {
                Debug.LogError("One or both textures are null!");
            }

            MathGameManager.Instance.OnDigitsRecognized(firstDigit, secondDigit);

            if (TraderManager.instance.WasTraderInvoked == true)
            {
                if (MathGameManager.Instance.isCorrect)
                {
                    TraderManager.instance.ApplyTraderDiscountToAllItems(25);
                    Sounds.Instance.PlaySoundEffect(Sounds.Instance.positiveAnswerSound, volume: 0.1f);
                    DestroyTask_DrawingSystem();
                }
                else if (MathGameManager.Instance.isCorrect == false && TaskCount <= 3)
                {
                    IncrementTaskCount();
                    Sounds.Instance.PlaySoundEffect(Sounds.Instance.negativeAnswerSound, volume: 0.1f);
                    DG.Tweening.Sequence seq = DOTween.Sequence();
                    seq.Append(TraderManager.instance.TaskText.transform.DOLocalMove(Vector3.up * 150, 2f).SetEase(Ease.OutCubic));
                    seq.Join(TraderManager.instance.TaskText.DOFade(0f, 2f));
                    seq.Append(TraderManager.instance.TaskText.transform.DOMove(TraderManager.instance._originalTextPosition, 0f));
                    seq.OnComplete(() =>
                    {
                        string taskText = MathGameManager.Instance.GenerateNewProblem();
                        TraderManager.instance.TaskText.text = taskText;
                        TraderManager.instance.TaskText.alpha = 1f;
                    }).SetLink(TraderManager.instance.TaskText.gameObject, LinkBehaviour.KillOnDestroy);
                    if (TaskCount >= 3) return;


                }
            }
        }
    }
    
    int RecognizeDigit(Texture2D texture)
    {
        if (IsTextureEmpty(texture))
        {
            return -1; 
        }
        Texture2D r8Texture = DrawingSystem.ConvertToR8(texture);//DrawingSystem.instance.baseTexture

        var channelCount = 1;//grayscale, 3 = color, 4 = color + alpha
        var inputX = new Tensor(r8Texture, channelCount);//inputTexture//r8Texture

        Tensor outputY = worker.Execute(inputX).PeekOutput();
        prediction.SetPrediction(outputY);
        
        inputX.Dispose();
        Destroy(r8Texture);//r8Texture//inputTexture
        return prediction.predictedValue;
    }
    void OnDestroy()
    {
        worker?.Dispose();
    }
}
