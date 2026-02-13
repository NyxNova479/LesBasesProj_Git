using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class SpawnNumbers : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NumberData[] allNumberPrefabs;

    private List<NumberData> numberPrefabs = new List<NumberData>();
    private List<GameObject> spawnedNumb = new List<GameObject>();

    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPositions;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _numbersUI;
    [SerializeField] private TextMeshProUGUI _waitingNumbUI;
    [SerializeField] private TextMeshProUGUI _spawningNumbUI;

    private Queue<NumberData> file = new Queue<NumberData>();

    private bool isSpawning = false;

    void Start()
    {

    }

    // --------------------------------------------------
    // 🔹 GÉNÉRATION DES NOMBRES DISPONIBLES
    // --------------------------------------------------

    public void GenerateAvailableNumbers(GameManager.Difficulty difficulty, int membreA, int membreB)
    {
        numberPrefabs.Clear();
        _numbersUI.text = "";

        List<NumberData> tempList = new List<NumberData>(allNumberPrefabs);

        int count = UnityEngine.Random.Range(4, 8);

        // 🔹 EASY → garantir présence
        if (difficulty == GameManager.Difficulty.Easy)
        {
            AddIfExists(membreA);
            AddIfExists(membreB);
        }

        // 🔹 HARD → retirer opérandes
        if (difficulty == GameManager.Difficulty.Hard)
        {
            tempList.RemoveAll(x =>
                x.buildTime == membreA ||
                x.buildTime == membreB);
        }

        // 🔹 Compléter aléatoirement
        while (numberPrefabs.Count < count && tempList.Count > 0)
        {
            int rng = UnityEngine.Random.Range(0, tempList.Count);

            if (!numberPrefabs.Contains(tempList[rng]))
                numberPrefabs.Add(tempList[rng]);

            tempList.RemoveAt(rng);
        }

        foreach (NumberData data in numberPrefabs)
        {
            _numbersUI.text += data.numberName + "\n";
        }
    }

    // --------------------------------------------------
    // 🔹 GARANTIT QUE LA SOLUTION EXISTE
    // --------------------------------------------------

    public void EnsureSolutionExists(int solution, GameManager.Difficulty difficulty)
    {
        if (difficulty == GameManager.Difficulty.Hard)
            return;
        else if (difficulty == GameManager.Difficulty.Medium)
            return;

        NumberData match = allNumberPrefabs.FirstOrDefault(x => x.buildTime == solution);

        if (match != null)
        {
            file.Enqueue(match);
            UpdateFileUI();
        }
    }

    // --------------------------------------------------
    // 🔹 UPDATE
    // --------------------------------------------------

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddRandomToQueue();
        }

        SpawnFromFile();
    }

    private void AddRandomToQueue()
    {
        if (numberPrefabs.Count == 0) return;

        NumberData random = numberPrefabs[UnityEngine.Random.Range(0, numberPrefabs.Count)];
        file.Enqueue(random);
        UpdateFileUI();
    }

    // --------------------------------------------------
    // 🔹 SPAWN LOGIC (QUEUE)
    // --------------------------------------------------

    private void SpawnFromFile()
    {
        if (file.Count == 0 || isSpawning)
            return;

        NumberData toSpawn = file.Dequeue();
        UpdateFileUI();

        _spawningNumbUI.text = toSpawn.numberName;
        StartCoroutine(SpawnAfterDelay(toSpawn));
    }

    private IEnumerator SpawnAfterDelay(NumberData data)
    {
        isSpawning = true;

        yield return new WaitForSeconds(data.buildTime);

        SpawnGOFromOriginal(data);

        isSpawning = false;

        if (file.Count == 0)
            _spawningNumbUI.text = "";
    }

    private void SpawnGOFromOriginal(NumberData original)
    {
        Transform rngPos = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)];

        GameObject number = Instantiate(original.prefab,
            rngPos.position,
            Quaternion.Euler(90, 0, 0));

        number.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        spawnedNumb.Add(number);
    }

    private void AddIfExists(int value)
    {
        NumberData match = allNumberPrefabs
            .FirstOrDefault(x => x.buildTime == value);

        if (match != null && !numberPrefabs.Contains(match))
        {
            numberPrefabs.Add(match);
        }
    }





    // --------------------------------------------------
    // 🔹 CLEAR ENTRE QUESTIONS
    // --------------------------------------------------

    public void ClearNumbers()
    {
        foreach (GameObject obj in spawnedNumb)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedNumb.Clear();
        file.Clear();
        UpdateFileUI();
        _spawningNumbUI.text = "";


    }

    // --------------------------------------------------
    // 🔹 UTILS
    // --------------------------------------------------

    private void UpdateFileUI()
    {
        if (file.Count == 0)
        {
            _waitingNumbUI.text = "Rien";
            return;
        }

        _waitingNumbUI.text = "";

        foreach (NumberData number in file)
        {
            _waitingNumbUI.text += number.numberName + "\n";
        }
    }

    public int partyMaxNumb()
    {
        int max = 0;

        foreach (NumberData data in numberPrefabs)
        {
            if (data.value > max)
                max = data.value;
        }

        return max;
    }
}
