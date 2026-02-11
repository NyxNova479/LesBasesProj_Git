using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Net;
using System;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform camPos;
    private UnityEngine.Vector3 startPos;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject player;
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private SpawnNumbers spawnNumbers;
    [SerializeField] private TextMeshProUGUI gameOverUI;
    [SerializeField] private TextMeshProUGUI questionsUI;
    [SerializeField] private TextMeshProUGUI timeUI;

    private static Color32 BASECOLOR = new Color(0,0,0,255);
    private static Color32 INVISIBLE = new Color(0,0,0,0);
    private static Color32 CORRECTANSWER = new Color(0,50,0,255);
    private static Color32 WRONGANSWER = new Color(50,0,0,255);

    public float delta = 0;
    private float answTimeLim = 50f;
    private bool isBlinking = false;

    private Dictionary<string, int[]> questions = new Dictionary<string, int[]>();
    private string currentQuestion = "";
    private bool questionCreated = false;

    private int membreA = 0;
    private int membreB = 0;
    private int goodCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = camPos.position;
        ground.GetComponent<Renderer>().material.color = BASECOLOR;
        questions.Add("Easy", new int[5]);
        questions.Add("Medium", new int[10]);
        questions.Add("Hard", new int[20]);
        InitializeQuestions();
        CreateQuestion();
    }

    private void InitializeQuestions()
    {
        for (int i = 0; i <= questions["Easy"].Length-1; i++)
        {
            questions["Easy"][i] = UnityEngine.Random.Range(1, spawnNumbers.partyMaxNumb());
        }
        for (int i = 0; i <= questions["Medium"].Length-1; i++)
        {
            questions["Medium"][i] = UnityEngine.Random.Range(1, spawnNumbers.partyMaxNumb()+1);
        }
        for (int i = 0; i <= questions["Hard"].Length - 1; i++)
        {
            questions["Hard"][i] = UnityEngine.Random.Range(1, spawnNumbers.partyMaxNumb()+2);
        }
    }

    private void CreateQuestion()
    {
        questionsUI.text = "";
        membreA = questions["Easy"][UnityEngine.Random.Range(0, questions["Easy"].Length)];
        membreB = questions["Easy"][UnityEngine.Random.Range(0, questions["Easy"].Length)];
        currentQuestion = $"{membreA} + {membreB}";
        questionsUI.text += currentQuestion;
        questionCreated = true;
    }

    // Update is called once per frame
    void Update()
    {
         

        if (!questionCreated && goodCount <= 1) CreateQuestion();

        if (camPos.position.y <= 10 && !isBlinking && goodCount <= 1)
        {
            isBlinking = true;
            StartCoroutine(DecideResult(membreA,membreB));

        }
        else if (camPos.position.y <= 10 && goodCount <= 1) delta = 0;
        
        else
        {
            delta += Time.deltaTime;
            if (answTimeLim - delta >= 0) timeUI.text = "" + (answTimeLim - delta).ToString();
            else timeUI.text = "0";
            MoveCamera(delta);
        }

        
    }

    private void MoveCamera(float delta)
    {
        if (delta >= answTimeLim)
        {
            for (int i = 0; i <= delta; i++)
            {
                camPos.position = new UnityEngine.Vector3(camPos.position.x, camPos.position.y - i / 5000f, camPos.position.z);
            }
        }
        else return;
    }

    private int currentResult(int a, int b)
    {
        return a + b;
    }

    private bool isCorrect(int membreA, int membreB)
    {
        if(currentResult(membreA, membreB) == player.GetComponent<PlayerBehaviour>().PlayerAnswer())
        {
            return true;
        }
        return false;
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
        yield return new WaitForSeconds(1f);
        ground.GetComponent<Renderer>().material.color = CORRECTANSWER;
        goodCount++;
        yield return new WaitForSeconds(1f);
        camPos.position = startPos;
        questionCreated = false;
        yield return new WaitForSeconds(2f);
        ground.GetComponent<Renderer>().material.color = BASECOLOR;
        delta = 0;
        player.GetComponent<PlayerBehaviour>().EmptyInventory();
        player.transform.position = player.GetComponent<PlayerBehaviour>().startPos;
        targetSpawner.moveCount = 0;
    }

    private IEnumerator ShowGameOver()
    {
        for (int i = 0; i <= "Game Over".Length-1; i++)
        {
            gameOverUI.text += "Game Over"[i];
            yield return new WaitForSeconds(0.5f);
        }  
    } 
}
