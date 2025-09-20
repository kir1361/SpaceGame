using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class MathProblem
{
    public int numA;
    public int numB;
    public MathOperation operation;
    public int correctAnswer;
    public enum MathOperation {Addition, Subtraction, Multiplication, Division}

    public static MathProblem GenerateRandomProblem(int minNum = 1, int maxNum = 10)
    {
        MathProblem problem = new MathProblem();
        problem.operation = (MathOperation)Random.Range(0, 4);

        switch (problem.operation)
        {
            case MathOperation.Addition:
                problem.numA = Random.Range(minNum, maxNum);
                problem.numB = Random.Range(minNum, maxNum);
                problem.correctAnswer = problem.numA + problem.numB;
                break;

            case MathOperation.Subtraction:
                problem.correctAnswer = Random.Range(minNum, maxNum);
                problem.numB = Random.Range(minNum, maxNum - problem.correctAnswer);
                problem.numA = problem.correctAnswer + problem.numB;
                break;

            case MathOperation.Multiplication:
                problem.numA = Random.Range(minNum, maxNum);
                problem.numB = Random.Range(minNum, maxNum);
                problem.correctAnswer = problem.numA * problem.numB;
                break;

            case MathOperation.Division:
                problem.correctAnswer = Random.Range(minNum, maxNum);
                problem.numB = Random.Range(minNum, maxNum);
                problem.numA = problem.correctAnswer * problem.numB;
                break;
        }
        return problem;
    }
    public string GetProblemString()
    {
        return $"? {GetOperatorSymbol()} ? = {correctAnswer}";
    }
    public char GetOperatorSymbol()
    {
        switch (operation)
        {
            case MathOperation.Addition: return '+';
            case MathOperation.Subtraction: return '-';
            case MathOperation.Multiplication: return '×';
            case MathOperation.Division: return '÷';
            default: return '?';
        }
    }
    public bool CheckSolution(int digit1, int digit2)
    {
        switch (operation)
        {
            case MathOperation.Addition: return (digit1 + digit2) == correctAnswer;
            case MathOperation.Subtraction: return (digit1 - digit2) == correctAnswer;
            case MathOperation.Multiplication: return (digit1 * digit2) == correctAnswer;
            case MathOperation.Division: return (digit2 != 0) && ((digit1 / digit2) == correctAnswer);
            default: return false;
        }
    }
}
