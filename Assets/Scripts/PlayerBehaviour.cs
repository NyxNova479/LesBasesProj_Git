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
    [SerializeField] private GameObject panel;
    public Dictionary<Number, int> inventory;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new Dictionary<Number, int>();
        player = GetComponent<NavMeshAgent>();
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null &&  targetSpawner != null &&  targetSpawner.currenttarget != null)
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


    public void UpdateInventoryUI(Number number)
    {

        if (inventory.ContainsKey(number)) return;
        else
        {
            inventoryUI.text += "." + number.gameObject.name + " x" + number.collectedTimes + "\n";
            inventoryUI.text = inventoryUI.text.Replace("(Clone)", "");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Target")
        {
            targetSpawner.pile.Pop();
            Destroy(collision.gameObject);
        }

    }


}
