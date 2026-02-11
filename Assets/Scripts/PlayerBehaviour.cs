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
using Unity.AI.Navigation;

public class PlayerBehaviour : MonoBehaviour
{

    private NavMeshAgent player;


    [SerializeField]
    private TargetSpawner targetSpawner;

    [SerializeField] private TextMeshProUGUI inventoryUI;
    [SerializeField] private GameObject panel;
    public Dictionary<NumberData, int> inventory;

    public Vector3 startPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new Dictionary<NumberData, int>();
        player = GetComponent<NavMeshAgent>();
        panel.SetActive(false);
        startPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && player.isActiveAndEnabled &&  targetSpawner != null && targetSpawner.currenttarget != null)
        {
            player.SetDestination(targetSpawner.currenttarget.position);

        }



        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowInventory();
        }
    }

    private void ShowInventory()
    {
        
        panel.SetActive(!panel.activeSelf);
    }


    public void UpdateInventoryUI()
    {
        inventoryUI.text = "";

        foreach (var entry in inventory)
        {
            NumberData data = entry.Key;
            int quantity = entry.Value;

            inventoryUI.text += $"{data.numberName} x{quantity}\n";
        }

    }

    public void EmptyInventory()
    {
        inventory = new Dictionary<NumberData, int>();
        inventoryUI.text = "";
    }

    public int PlayerAnswer()
    {
        int score = 0;
        foreach(KeyValuePair< NumberData, int> pair in inventory)
        {
            score += pair.Key.buildTime * pair.Value;
        }
        return score;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Target"))
        {
            targetSpawner.pile.Pop();
            Destroy(collision.gameObject);
        }

    }

    public void AddNumber(NumberData data)
    {
        if (inventory.ContainsKey(data))
            inventory[data]++;
        else
            inventory.Add(data, 1);

        UpdateInventoryUI();
    }


}
