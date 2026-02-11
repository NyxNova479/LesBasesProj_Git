using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UIElements;
using UnityEngine.UIElements;
using UnityEditor.UI;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class SpawnNumbers : MonoBehaviour
{

    [SerializeField]
    private List<NumberData> numberPrefabs;
    [SerializeField] private Transform[] spawnPositions = new Transform[5];
    [SerializeField] private TextMeshProUGUI _numbersUI;
    [SerializeField] private TextMeshProUGUI _waitingNumbUI;
    [SerializeField] private TextMeshProUGUI _spawningNumbUI;

    [SerializeField]
    NumberData[] allNumberPrefabs;

    private int numbLengthLimit;
    private int rng;
    

    [SerializeField]
    private Transform spawnerPoint;

    public bool isSpawning = false;

    Queue<NumberData> file;


    private NumberData willSpawn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool flowControl = StartGame();
        if (!flowControl)
        {
            return;
        }
    }



    private bool StartGame()
    {
        numbLengthLimit = UnityEngine.Random.Range(3, 9);
        numberPrefabs = new List<NumberData>(new NumberData[numbLengthLimit]);
        file = new Queue<NumberData>();
        for (int i = 0; i <= numbLengthLimit - 1; i++)
        {

            rng = UnityEngine.Random.Range(0, allNumberPrefabs.Length - 1);
            try
            {
                if (numberPrefabs.Contains(allNumberPrefabs[rng]))
                {
                    i--;
                    continue;
                }
                else
                {
                    numberPrefabs[i] = allNumberPrefabs[rng];

                }

            }
            catch (ArgumentOutOfRangeException)
            {
                if (i == numberPrefabs.Count())
                {
                    return false;

                }
                i--;
                continue;
            }

        }
        foreach (NumberData prefabs in numberPrefabs)
        {
            _numbersUI.text += "" + prefabs.name + "\n";
        }

        return true;
    }

    public int partyMaxNumb()
    {
        int maxNumb = 0;
        foreach (NumberData numberData in numberPrefabs)
        {
            if (maxNumb < numberData.buildTime)
            {
                maxNumb = numberData.buildTime;
            }
        }
        return maxNumb;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            willSpawn = numberPrefabs[UnityEngine.Random.Range(0, numberPrefabs.Count())];
            AjouterALaFile(willSpawn);

            UpdateFileUI();
            
        }
        SpawnFromFile();
    }

    // Update is called once per frame
    private void UpdateFileUI()
    {
        if (file.Count() == 0)
        {
            _waitingNumbUI.text = "Rien";
            return;
        }

        _waitingNumbUI.text = "";

        foreach (NumberData number in file)
        {
            _waitingNumbUI.text += number.name + "\n";
        }
        
    }


    private void SpawnNumber(GameObject number)
    {
        StartCoroutine(SpawnAfterDelay(number.GetComponent<NumberData>()));
    }

    private void SpawnGOFromOriginal(NumberData goOriginal)
    {
        Transform rngPos = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length)];
        GameObject number = Instantiate(goOriginal.prefab, rngPos.position, Quaternion.Euler(90,0,0));
        number.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
    }

    public void AjouterALaFile(NumberData number)
    {
        file.Enqueue(number);
    }

    private IEnumerator SpawnAfterDelay(NumberData numberData)
    {
        isSpawning = true;
        yield return new WaitForSeconds(numberData.buildTime);
        SpawnGOFromOriginal(numberData);
        isSpawning = false;
        if (file.Count() == 0) _spawningNumbUI.text = "";



    }

    private void SpawnFromFile()
    {
        
        if (file.Count() == 0) return;
        
        while (file.Count() > 0)
        {
            if (isSpawning) return;
            else
            {
                NumberData hasToSpawn = file.Dequeue();
                UpdateFileUI();
                _spawningNumbUI.text = "" + hasToSpawn.name;
                StartCoroutine(SpawnAfterDelay(hasToSpawn));

            }
        }

    }


}
