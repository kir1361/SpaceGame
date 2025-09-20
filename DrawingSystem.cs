using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Color = UnityEngine.Color;
using System;
using DG.Tweening;
using System.Collections;
using NUnit.Framework;


public class DrawingSystem : MonoBehaviour
{
    // private bool isDrawing = false;
    // private bool isSpacePressed = false;
    // private DigitInputZone currentDrawingZone;
    // private bool isInValidZone = false;
    // public bool isDrawingComplete { get; private set; }
    //public Texture2D baseTexture;
    public static DrawingSystem instance;
    Color color;
    public Texture2D digit1Texture; // Для первой цифры
    public Texture2D digit2Texture; // Для второй цифры
    public MeshRenderer firstDigitRenderer;
    public MeshRenderer secondDigitRenderer;
    public Texture2D currentTexture;
    public static System.Action OnReminderNeeded;

    private bool _isWaitingForConfirmation = false;
    private bool _hasShownReminder = false; // Флаг однократного показа
    private Coroutine _reminderCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        currentTexture = digit1Texture;
        color = currentTexture.GetPixel(currentTexture.width / 2, currentTexture.height / 2);
        firstDigitRenderer.material.mainTexture = digit1Texture;
        secondDigitRenderer.material.mainTexture = digit2Texture;
    }

    // void Update()
    // {
    //     HandleInput();
    //     if (isSpacePressed && currentDrawingZone != null)
    //     {
    //         DrawAtSpaceshipPosition();
    //     }
    // }

    // void HandleInput()
    // {
    //     // Начало рисования - нажали пробел
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         DigitInputZone zone = GetCurrentZone();
    //         if (zone != null)
    //         {
    //             currentDrawingZone = zone;
    //             isSpacePressed = true;
    //             ClearDrawingTexture(); // Очищаем при начале нового рисунка
    //         }
    //     }

    //     // Конец рисования - отпустили пробел
    //     if (Input.GetKeyUp(KeyCode.Space))
    //     {
    //         isSpacePressed = false;
    //         currentDrawingZone = null;
    //     }
    // }

    // DigitInputZone GetCurrentZone()
    // {
    //     foreach (var zone in FindObjectsOfType<DigitInputZone>())
    //     {
    //         if (zone.isPlayerInZone) return zone;
    //     }
    //     return null;
    // }

    // void DrawAtSpaceshipPosition()
    // {
    //     if (currentDrawingZone == null) return;

    //     Vector2 pixelUV = GetTextureCoordinate(Player.instance.transform.position);

    //     baseTexture.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.white);
    //     baseTexture.Apply();
    // }

    // Vector2 GetTextureCoordinate(Vector3 worldPosition)
    // {
    //     BoxCollider2D collider = currentDrawingZone.GetComponent<BoxCollider2D>();
    //     if (collider != null)
    //     {
    //         Vector3 localPos = currentDrawingZone.transform.InverseTransformPoint(worldPosition);

    //         float u = (localPos.x / collider.size.x) + 0.5f;
    //         float v = (localPos.y / collider.size.y) + 0.5f;

    //         int pixelX = Mathf.Clamp((int)(u * baseTexture.width), 0, baseTexture.width - 1);
    //         int pixelY = Mathf.Clamp((int)(v * baseTexture.height), 0, baseTexture.height - 1);

    //         return new Vector2(pixelX, pixelY);
    //     }

    //     return Vector2.zero;
    // }

    // void ClearDrawingTexture()
    // {
    //     // Очищаем текстуру (заполняем черным)
    //     for (int x = 0; x < baseTexture.width; x++)
    //     {
    //         for (int y = 0; y < baseTexture.height; y++)
    //         {
    //             baseTexture.SetPixel(x, y, Color.black);
    //         }
    //     }
    //     baseTexture.Apply();
    // }

    //////////////////////////////
    //Ниже код рисования мышкой.//
    //////////////////////////////

    //   private void DoMouseDrawing()
    // {
    //     if (Camera.main == null)
    //     {
    //         Debug.LogError("Main camera not found!");
    //         return;
    //     }

    //     // Используем назначенную камеру или main camera
    //     Camera cam =  Camera.main;

    //     if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;

    //     Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;

    //     if (!Physics.Raycast(mouseRay, out hit)) 
    //     {
    //         Debug.Log("Raycast не попал ни в один объект");
    //         return;
    //     }

    //     if (hit.collider.transform != transform) 
    //     {
    //         Debug.Log("Попали в объект: " + hit.collider.name + ", но нужен: " + transform.name);
    //         return;
    //     }

    //     if (hit.collider == null)
    //     {
    //         Debug.LogWarning("Нет коллайдера на объекте!");
    //         return;
    //     }

    //     Vector2 pixelUV = hit.textureCoord;
    //     pixelUV.x *= baseTexture.width;
    //     pixelUV.y *= baseTexture.height;

    //     // Добавляем проверку границ
    //     int pixelX = Mathf.Clamp((int)pixelUV.x, 0, baseTexture.width - 1);
    //     int pixelY = Mathf.Clamp((int)pixelUV.y, 0, baseTexture.height - 1);

    //     Color colorToSet = Input.GetMouseButton(0) ? Color.white : Color.black;

    //     baseTexture.SetPixel(pixelX, pixelY, colorToSet);
    //     baseTexture.Apply();

    //     Debug.Log($"Нарисован пиксель на: {pixelX}, {pixelY}");
    // }

    // Update is called once per frame
    void Update()
    {
        DoMouseDrawing();
        UpdateDrawingState();
    }
    private void UpdateDrawingState()
    {
        bool bothDrawn = CheckIfBothDigitsDrawn();
    
        if (bothDrawn && !_isWaitingForConfirmation && !_hasShownReminder)
        {
            _isWaitingForConfirmation = true;
            _reminderCoroutine = StartCoroutine(ReminderCountdown());
        }
        // Если нажали E - отменяем ожидание
        if (Input.GetKeyDown(KeyCode.E) && _isWaitingForConfirmation)
        {
            _isWaitingForConfirmation = false;
            _hasShownReminder = true;
            if (_reminderCoroutine != null)
            {
                StopCoroutine(_reminderCoroutine);
                _reminderCoroutine = null;
            }
        }
    }
    private IEnumerator ReminderCountdown()
    {
        yield return new WaitForSeconds(10f);
        if (_isWaitingForConfirmation && CheckIfBothDigitsDrawn() && !_hasShownReminder)
        {
            OnReminderNeeded?.Invoke();
            _hasShownReminder = true;
        }
        _isWaitingForConfirmation = false;
        _reminderCoroutine = null;
    }
    
    private bool CheckIfBothDigitsDrawn()
    {
        return HasDrawing(digit1Texture) && HasDrawing(digit2Texture);
    }
    
    private bool HasDrawing(Texture2D texture)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color pixel = texture.GetPixel(x, y);
                if (pixel.r > 0.1f) 
                    return true;
            }
        }
        return false;
    }
    public void FadeAlpha(float targetAlpha, float duration)
    {
        firstDigitRenderer.material.DOFade(targetAlpha, duration);
        secondDigitRenderer.material.DOFade(targetAlpha, duration);
    }
    private void DoMouseDrawing()
    {
        // Don't bother trying to run if we can't find the main camera.
        if (Camera.main == null)
        {
            throw new Exception("Cannot find main camera");
        }

        // Is the mouse being pressed?
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;
        // Cast a ray into the scene from screenspace where the mouse is.
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Do nothing if we aren't hitting anything.
        if (!Physics.Raycast(mouseRay, out hit)) return;

        if (!hit.collider.transform.IsChildOf(transform)) return;

        // Do nothing if we didn't get hit.
        //if (hit.collider.transform != transform) return;
        if (hit.collider.CompareTag("FirstDigitZone"))
        {
            DrawToTexture(hit, digit1Texture);
        }
        else if (hit.collider.CompareTag("SecondDigitZone"))
        {
            DrawToTexture(hit, digit2Texture);
        }
    }
    private void DrawToTexture(RaycastHit hit, Texture2D texture)
    {
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= texture.width;
        pixelUV.y *= texture.height;

        // Set the color as white if the lmb is being pressed, black if rmb.
        Color colorToSet = Input.GetMouseButton(0) ? Color.white : Color.black;//color

        // Update the texture and apply.
        texture.SetPixel((int)pixelUV.x, (int)pixelUV.y, colorToSet);
        texture.Apply();
    }
    public void ClearDrawingTexture(Texture2D texture)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, Color.black);//Color.black//colorS
            }
        }

        texture.Apply();
    }

    public List<Texture2D> GetAllDigitTextures()
    {
        return new List<Texture2D> { digit1Texture, digit2Texture };
    }
    //Конвертация в формат для нейросети, ей нужен grayscale.
    public static Texture2D ConvertToR8(Texture2D texture)
    {
        Texture2D r8Texture = new Texture2D(texture.width, texture.height, TextureFormat.R8, false);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = texture.GetPixel(x, y);
                float grayscale = color.grayscale;
                r8Texture.SetPixel(x, y, new Color(grayscale, 0, 0));
            }
        }
        r8Texture.Apply();
        return r8Texture;
    }
    void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}