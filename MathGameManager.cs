using System.Collections;
using UnityEngine;

public class MathGameManager : MonoBehaviour
{
    [HideInInspector] public MathProblem currentProblem;
    public int firstDigit = -1;
    public int secondDigit = -1;
    public static MathGameManager Instance;
    public bool isCorrect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    void Start()
    {
        //InvokeRepeating(nameof(GenerateNewProblem),5f,30f);
        //StartCoroutine(NexProblemDelay(10f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string GenerateNewProblem()
    {
        currentProblem = MathProblem.GenerateRandomProblem(1, 10);
        return currentProblem.GetProblemString();
        firstDigit = -1;
        secondDigit = -1;
    }
    public void OnDigitsRecognized(int digit1, int digit2)
    {
        firstDigit = digit1;
        secondDigit = digit2;
        isCorrect = currentProblem.CheckSolution(digit1, digit2);
        Debug.Log($"{digit1} {currentProblem.GetOperatorSymbol()} {digit2} = {CalculateResult(digit1, digit2)}");
        Debug.Log($"Ожидается: {currentProblem.correctAnswer}, Верно: {isCorrect}");
    }
    public int CalculateResult(int digit1, int digit2)
    {
        switch (currentProblem.operation)
        {
            case MathProblem.MathOperation.Addition:
                return digit1 + digit2;
            case MathProblem.MathOperation.Subtraction:
                return digit1 - digit2;
            case MathProblem.MathOperation.Multiplication:
                return digit1 * digit2;
            case MathProblem.MathOperation.Division:
                if (digit2 != 0)
                    return digit1 / digit2;
                else
                {
                    Debug.LogWarning("Division by zero!");
                    return 0;
                }
            default:
                Debug.LogError("Unknown operation");
                return 0;
        }
    }
  
    // IEnumerator NexProblemDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     GenerateNewProblem();
    // }
}
