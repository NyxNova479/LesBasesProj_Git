using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UIElements;
using UnityEngine.UIElements;


public class Spawn : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> numberPrefabs;

    [SerializeField]
    GameObject[] allNumberPrefabs;

    private int numbLengthLimit;
    private int rng;

    [SerializeField]
    private Transform spawnerPoint;

    public bool isSpawning = false;

    Queue<Number> file;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //numbLengthLimit = UnityEngine.Random.Range(3, 9);
       //numberPrefabs = new List<GameObject>(new GameObject[numbLengthLimit]);
       file = new Queue<Number>();
       //for (int i = 0; i <= numbLengthLimit; i++)
       //{
       //
       //    rng = UnityEngine.Random.Range(0, allNumberPrefabs.Length - 1);
       //    try
       //    {
       //        if (!numberPrefabs[rng])
       //        {
       //            numberPrefabs[i] = allNumberPrefabs[rng];
       //        }
       //        else
       //        {
       //            i--;
       //            
       //        }
       //        
       //    }
       //    catch (ArgumentOutOfRangeException)
       //    {
       //        
       //        for(int index = 0; index <= numberPrefabs.Count-1; index++)
       //        {
       //            if (numberPrefabs[index] == allNumberPrefabs[rng].gameObject)
       //            {
       //                numberPrefabs[index] = allNumberPrefabs[rng];
       //                continue;
       //
       //            }
       //            else
       //            {
       //                numberPrefabs[i] = allNumberPrefabs[rng];
       //            }        
       //        }
       //        
       //    }
       //
       //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void SpawnNumber(GameObject number)
    {
        StartCoroutine(SpawnAfterDelay(number));
    }

    private void SpawnGOFromOriginal(GameObject goOriginal)
    {
        Instantiate(goOriginal, spawnerPoint.position, Quaternion.identity);
    }

    public void AjouterALaFile(GameObject number)
    {
        file.Enqueue(number.GetComponent<Number>());
    }

    private IEnumerator SpawnAfterDelay(GameObject number)
    {
        isSpawning= true;
        yield return new WaitForSeconds(number.GetComponent<Number>().getTempsConstruction());
        SpawnGOFromOriginal(number.gameObject);
        isSpawning = false;

    }

    public void OnClick()
    {
        GameObject willSpawn = allNumberPrefabs[UnityEngine.Random.Range(0, allNumberPrefabs.Length - 1)];
        AjouterALaFile(willSpawn);
        GameObject hasToSpawn = file.Dequeue().gameObject;
        SpawnNumber(hasToSpawn);
    }
}
