using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using Unity.UIElements;
using UnityEngine.UIElements;
using UnityEditor.UI;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerBehaviour : MonoBehaviour
{

    private NavMeshAgent player;

    [SerializeField]
    private TargetSpawner targetSpawner;

    [SerializeField] private TextMeshProUGUI inventoryUI;
    Dictionary<Number, int> inventory;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new Dictionary<Number, int>();
        player = GetComponent<NavMeshAgent>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && targetSpawner.currenttarget != null)
        {
            player.SetDestination(targetSpawner.currenttarget.position);

        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }
    }

    private void OpenInventory()
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Target")
        {
            targetSpawner.pile.Pop();
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "Number")
        {
            StoreNumber(collision.GetComponent<Number>());
        }

    }

    private void StoreNumber(Number number)
    {
        if (inventory.ContainsKey(number))
        {
            int numbCount = number.collectedTimes++;
            inventory[number] = numbCount;
        }
        else
        {
            inventory.Add(number, number.collectedTimes);
        }
        Destroy(number.gameObject);
    }
}
