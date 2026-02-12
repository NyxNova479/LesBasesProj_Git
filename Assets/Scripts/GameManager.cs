using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public enum Difficulty { Easy, Medium, Hard }
    public enum Operation { Add, Subtract, Multiply, Divide }

    [Header("Scene References")]
    [SerializeField] private Transform camPos;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject player;
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private SpawnNumbers spawnNumbers;
    [SerializeField] private TextMeshProUGUI gameOverUI;
    [SerializeField] private TextMeshProUGUI questionsUI;
    [SerializeField] private TextMeshProUGUI timeUI;

    [Header("Difficulty UI")]
    [SerializeField] private GameObject difficultyUI;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    private Vector3 startPos;

    private static Color32 BASECOLOR = new Color(0, 0, 0, 255);
    private static Color32 INVISIBLE = new Color(0, 0, 0, 0);
    private static Color32 CORRECTANSWER = new Color(0, 50, 0, 255);
    private static Color32 WRONGANSWER = new Color(50, 0, 0, 255);

    public float delta = 0;
    private float currentTimeLimit;
    private float minimumTime = 2f;

    private bool isBlinking = false;
    private bool questionCreated = false;
    private bool solutionInjected = false;

    public int membreA = 0;
    public int membreB = 0;
    private int correctResult = 0;
    private Operation currentOperation;

    private int goodCount = 0;

    public Difficulty currentDifficulty = Difficulty.Easy;
    private int thresholdToTrigger;
    private bool difficultyMenuActive = false;

    private List<Operation> allowedOperations = new List<Operation>();

    void Start()
    {
        startPos = camPos.position;
        ground.GetComponent<Renderer>().material.color = BASECOLOR;

        ApplyDifficultySettings();
        CreateQuestion();
    }

    private void Update()
    {
        if (difficultyMenuActive) return;

        if (!questionCreated)
            CreateQuestion();

        delta += Time.deltaTime;
        float remaining = currentTimeLimit - delta;

        timeUI.text = Mathf.Max(0, remaining).ToString("F0");

        if (currentDifficulty == Difficulty.Easy && !solutionInjected && delta >= currentTimeLimit - 30f)
        {
            spawnNumbers.EnsureSolutionExists(correctResult, currentDifficulty);
            solutionInjected = true;
        }

        if (remaining <= 0)
        {
            camPos.position = new Vector3(
                camPos.position.x,
                camPos.position.y - (delta / 500f),
                camPos.position.z);
        }

        if (camPos.position.y <= 10 && !isBlinking)
        {
            delta = 0;
            isBlinking = true;
            StartCoroutine(DecideResult());
        }
    }

    private void ApplyDifficultySettings()
    {
        allowedOperations.Clear();

        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                allowedOperations.Add(Operation.Add);
                currentTimeLimit = 50f;
                targetSpawner.SetMoveLimit(5);
                thresholdToTrigger = 3;
                break;

            case Difficulty.Medium:
                allowedOperations.Add(Operation.Add);
                allowedOperations.Add(Operation.Subtract);
                currentTimeLimit = 40f;
                targetSpawner.SetMoveLimit(4);
                thresholdToTrigger = 5;
                break;

            case Difficulty.Hard:
                allowedOperations.Add(Operation.Add);
                allowedOperations.Add(Operation.Subtract);
                allowedOperations.Add(Operation.Multiply);
                allowedOperations.Add(Operation.Divide);
                currentTimeLimit = 30f;
                targetSpawner.SetMoveLimit(3);
                thresholdToTrigger = 8;
                break;
        }

        delta = 0;
    }

    private void CreateQuestion()
    {
        delta = 0;
        isBlinking = false;

        currentOperation = allowedOperations[Random.Range(0, allowedOperations.Count)];

        bool validQuestion = false;

        while (!validQuestion)
        {
            switch (currentOperation)
            {
                case Operation.Add:
                    membreA = Random.Range(1, 10);
                    membreB = Random.Range(1, 10);
                    correctResult = membreA + membreB;
                    break;

                case Operation.Subtract:
                    membreA = Random.Range(1, 10);
                    membreB = Random.Range(1, 10);

                    if (membreB > membreA)
                    {
                        int temp = membreA;
                        membreA = membreB;
                        membreB = temp;
                    }

                    correctResult = membreA - membreB;
                    break;

                case Operation.Multiply:
                    membreA = Random.Range(1, 11);
                    membreB = Random.Range(1, 11);
                    correctResult = membreA * membreB;
                    break;

                case Operation.Divide:
                    membreB = Random.Range(1, 11);
                    correctResult = Random.Range(1, 11);
                    membreA = membreB * correctResult;
                    break;
            }

            // Vérifie que le résultat est constructible (1 à 10)
            if (correctResult >= 1 && correctResult <= 10)
            {
                validQuestion = true;
            }
            else
            {
                // sinon on regénère une nouvelle opération
                currentOperation = allowedOperations[Random.Range(0, allowedOperations.Count)];
            }
        }

        string symbol = "+";
        if (currentOperation == Operation.Subtract) symbol = "-";
        if (currentOperation == Operation.Multiply) symbol = "×";
        if (currentOperation == Operation.Divide) symbol = "÷";

        questionsUI.text = $"{membreA} {symbol} {membreB}";


        questionCreated = true;
        solutionInjected = false;
    }

    public bool IsCorrect()
    {
        return player.GetComponent<PlayerBehaviour>().PlayerAnswer() == correctResult;
    }

    private IEnumerator DecideResult()
    {

        // 🔹 Clignotement
        for (int i = 0; i < 3; i++)
        {
            ground.GetComponent<Renderer>().material.color = INVISIBLE;
            yield return new WaitForSeconds(0.3f);

            ground.GetComponent<Renderer>().material.color = BASECOLOR;
            yield return new WaitForSeconds(0.3f);
        }

        //  Si FAUX
        if (!IsCorrect())
        {
            ground.GetComponent<Renderer>().material.color = WRONGANSWER;
            yield return new WaitForSeconds(0.8f);

            //  Désactivation du déplacement
            player.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

            //  Désactivation du NavMesh
            ground.GetComponent<Unity.AI.Navigation.NavMeshSurface>().enabled = false;

            // Désactivation visuelle du sol
            ground.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(ShowGameOver());
        }
        else
        {
            //  Si BON
            ground.GetComponent<Renderer>().material.color = CORRECTANSWER;
            
            yield return new WaitForSeconds(0.8f);

            StartCoroutine(ResetGame());
        }
    }

    private IEnumerator ResetGame()
    {
        spawnNumbers.ClearNumbers();

        yield return new WaitForSeconds(1f);
        ground.GetComponent<Renderer>().material.color = CORRECTANSWER;

        goodCount++;

        currentTimeLimit = Mathf.Max(minimumTime, currentTimeLimit - 5f);

        if (goodCount >= thresholdToTrigger)
        {
            TriggerDifficultyMenu();
            yield break;
        }

        yield return new WaitForSeconds(1f);

        camPos.position = startPos;
        questionCreated = false;
        ground.GetComponent<Renderer>().material.color = BASECOLOR;

        player.GetComponent<PlayerBehaviour>().EmptyInventory();
        targetSpawner.ResetMoves();

        delta = 0;
    }

    private void TriggerDifficultyMenu()
    {
        difficultyMenuActive = true;
        Time.timeScale = 0f;
        difficultyUI.SetActive(true);

        decreaseButton.interactable = currentDifficulty != Difficulty.Easy;
        increaseButton.interactable = currentDifficulty != Difficulty.Hard;
    }

    public void IncreaseDifficulty()
    {
        if (currentDifficulty != Difficulty.Hard)
            currentDifficulty++;
        camPos.position = startPos;
        questionCreated = false;
        ground.GetComponent<Renderer>().material.color = BASECOLOR;

        player.GetComponent<PlayerBehaviour>().EmptyInventory();
        targetSpawner.ResetMoves();

        delta = 0;
        ResumeGame();
    }

    public void DecreaseDifficulty()
    {
        if (currentDifficulty != Difficulty.Easy)
            currentDifficulty--;
        camPos.position = startPos;
        questionCreated = false;
        ground.GetComponent<Renderer>().material.color = BASECOLOR;

        player.GetComponent<PlayerBehaviour>().EmptyInventory();
        targetSpawner.ResetMoves();

        delta = 0;
        ResumeGame();
    }

    public void ContinueSameDifficulty()
    {
        camPos.position = startPos;
        questionCreated = false;
        ground.GetComponent<Renderer>().material.color = BASECOLOR;

        player.GetComponent<PlayerBehaviour>().EmptyInventory();
        targetSpawner.ResetMoves();

        delta = 0;
        ResumeGame();
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        difficultyUI.SetActive(false);
        difficultyMenuActive = false;

        goodCount = 0;

        ApplyDifficultySettings();
        CreateQuestion();
    }



    private IEnumerator ShowGameOver()
    {
        for (int i = 0; i < "Game Over".Length; i++)
        {
            gameOverUI.text += "Game Over"[i];
            yield return new WaitForSeconds(0.5f);
        }
    }
}
