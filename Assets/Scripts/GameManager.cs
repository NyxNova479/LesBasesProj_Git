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
    private float answTimeLim = 5f;
    private bool isBlinking = false;

    private Dictionary<string, int[]> questions = new Dictionary<string, int[]>();

    private string currentQuestion = "";
    private bool questionCreated = false;

    public int membreA = 0;
    public int membreB = 0;
    private int goodCount = 0;

    public Difficulty currentDifficulty = Difficulty.Easy;
    private int thresholdToTrigger;
    private bool difficultyMenuActive = false;

    void Start()
    {
        startPos = camPos.position;
        ground.GetComponent<Renderer>().material.color = BASECOLOR;

        questions.Add("Easy", new int[5]);
        questions.Add("Medium", new int[10]);
        questions.Add("Hard", new int[20]);

        SetThreshold();
        InitializeQuestions(currentDifficulty.ToString());
        CreateQuestion();
    }

    private void Update()
    {
        if (difficultyMenuActive) return;

        if (!questionCreated) CreateQuestion();

        if (camPos.position.y <= 10 && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(DecideResult(membreA, membreB));
        }
        else if (camPos.position.y <= 10)
        {
            delta = 0;
        }
        else
        {
            delta += Time.deltaTime;

            if (answTimeLim - delta >= 0)
                timeUI.text = (answTimeLim - delta).ToString("F0");
            else
                timeUI.text = "0";

            MoveCamera(delta);
        }
    }

    private void SetThreshold()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                thresholdToTrigger = 3;
                break;
            case Difficulty.Medium:
                thresholdToTrigger = 5;
                break;
            case Difficulty.Hard:
                thresholdToTrigger = 8;
                break;
        }
    }

    private void InitializeQuestions(string mode)
    {
        for (int i = 0; i < questions[mode].Length; i++)
        {
            questions[mode][i] = UnityEngine.Random.Range(1, spawnNumbers.partyMaxNumb());
        }
    }

    private void CreateQuestion()
    {
        questionsUI.text = "";

        string mode = currentDifficulty.ToString();

        membreA = questions[mode][UnityEngine.Random.Range(0, questions[mode].Length)];
        membreB = questions[mode][UnityEngine.Random.Range(0, questions[mode].Length)];

        currentQuestion = $"{membreA} + {membreB}";
        questionsUI.text = currentQuestion;

        questionCreated = true;
        isBlinking = false;
    }

    private void MoveCamera(float delta)
    {
        if (delta >= answTimeLim)
        {
            camPos.position = new Vector3(camPos.position.x,camPos.position.y - (delta / 150f),camPos.position.z);
        }
    }

    private int currentResult(int a, int b)
    {
        return a + b;
    }

    public bool isCorrect(int membreA, int membreB)
    {
        return currentResult(membreA, membreB) == player.GetComponent<PlayerBehaviour>().PlayerAnswer();
    }

    private IEnumerator DecideResult(int a, int b)
    {
        for (int i = 0; i <= 2; i++)
        {
            yield return new WaitForSeconds(0.5f);
            ground.GetComponent<Renderer>().material.color = INVISIBLE;
            yield return new WaitForSeconds(0.5f);
            ground.GetComponent<Renderer>().material.color = BASECOLOR;
        }

        if (!isCorrect(a, b))
        {
            yield return new WaitForSeconds(1f);
            ground.GetComponent<Renderer>().material.color = WRONGANSWER;
            yield return new WaitForSeconds(1f);

            player.GetComponent<NavMeshAgent>().enabled = false;
            ground.GetComponent<NavMeshSurface>().enabled = false;
            ground.SetActive(false);

            StartCoroutine(ShowGameOver());
        }
        else
        {
            StartCoroutine(ResetGame());
        }
    }

    private IEnumerator ResetGame()
    {
        spawnNumbers.ClearNumbers();
        yield return new WaitForSeconds(1f);
        ground.GetComponent<Renderer>().material.color = CORRECTANSWER;

        goodCount++;

        if (goodCount >= thresholdToTrigger)
        {
            TriggerDifficultyMenu();
            yield break;
        }

        yield return new WaitForSeconds(1f);

        camPos.position = startPos;
        questionCreated = false;

        yield return new WaitForSeconds(1f);

        ground.GetComponent<Renderer>().material.color = BASECOLOR;

        delta = 0;
        player.GetComponent<PlayerBehaviour>().EmptyInventory();
        player.transform.position = player.GetComponent<PlayerBehaviour>().startPos;
        targetSpawner.moveCount = 0;
        targetSpawner._movesUI.text = "" + targetSpawner.moveLimit;
    }

    private void TriggerDifficultyMenu()
    {
        difficultyMenuActive = true;
        Time.timeScale = 0f;
        difficultyUI.SetActive(true);

        decreaseButton.interactable = currentDifficulty != Difficulty.Easy;
        increaseButton.interactable = currentDifficulty != Difficulty.Hard;
    }

    public void ContinueSameDifficulty()
    {
        ResetGame();
        ResumeGame();
    }

    public void IncreaseDifficulty()
    {
        if (currentDifficulty != Difficulty.Hard)
            currentDifficulty++;
        ResetGame();
        ResumeGame();
    }

    public void DecreaseDifficulty()
    {
        if (currentDifficulty != Difficulty.Easy)
            currentDifficulty--;
        ResetGame();
        ResumeGame();
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        difficultyUI.SetActive(false);
        difficultyMenuActive = false;

        goodCount = 0;

        SetThreshold();
        InitializeQuestions(currentDifficulty.ToString());

        questionCreated = false;
        delta = 0;
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
