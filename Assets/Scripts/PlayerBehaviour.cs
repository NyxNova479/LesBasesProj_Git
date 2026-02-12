using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBehaviour : MonoBehaviour
{
    private NavMeshAgent player;

    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private TextMeshProUGUI inventoryUI;
    [SerializeField] private GameObject panel;

    public Dictionary<NumberData, int> inventory = new Dictionary<NumberData, int>();

    // Stack utilisée pour le UNDO (respect structure imposée)
    private Stack<NumberData> inventoryHistory = new Stack<NumberData>();

    public Vector3 startPos;

    private int undoUsed;
    private int undoLimit;

    void Start()
    {
        player = GetComponent<NavMeshAgent>();
        panel.SetActive(false);
        startPos = transform.position;
        ConfigureUndo(gameManager.currentDifficulty);
    }

    void Update()
    {
        if (player != null &&
            player.isActiveAndEnabled &&
            targetSpawner != null &&
            targetSpawner.currenttarget != null)
        {
            player.SetDestination(targetSpawner.currenttarget.position);
        }

        // Inventaire visible sauf en Hard
        if (Input.GetKeyDown(KeyCode.I) &&
            gameManager.currentDifficulty != GameManager.Difficulty.Hard)
        {
            ToggleInventory();
        }

        // Undo avec Backspace
        if (Input.GetKeyDown(KeyCode.Backspace) && undoUsed<undoLimit)
        {
            UndoLastNumber();
        }
    }

    private void ToggleInventory()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void UpdateInventoryUI()
    {
        inventoryUI.text = "";

        foreach (var entry in inventory)
        {
            inventoryUI.text += $"{entry.Key.numberName} x{entry.Value}\n";
        }
    }

    public void EmptyInventory()
    {
        inventory.Clear();
        inventoryHistory.Clear();
        inventoryUI.text = "";
    }

    // Calcule la réponse du joueur
    public int PlayerAnswer()
    {
        int total = 0;

        foreach (var pair in inventory)
        {
            total += pair.Key.value * pair.Value; ;
        }

        return total;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Target"))
        {
            // Sécurité pile
            if (targetSpawner.pile.Count > 0)
                targetSpawner.pile.Pop();

            Destroy(collision.gameObject);
        }
    }

    public void AddNumber(NumberData data)
    {
        if (data == null) return;

        if (inventory.ContainsKey(data))
            inventory[data]++;
        else
            inventory.Add(data, 1);

        inventoryHistory.Push(data);

        UpdateInventoryUI();
    }

    public void UndoLastNumber()
    {
        if (inventoryHistory.Count == 0)
            return;

        NumberData last = inventoryHistory.Pop();

        if (inventory.ContainsKey(last))
        {
            inventory[last]--;

            if (inventory[last] <= 0)
                inventory.Remove(last);
        }

        UpdateInventoryUI();
    }

    public void ConfigureUndo(GameManager.Difficulty difficulty)
    {
        undoUsed = 0;

        switch (difficulty)
        {
            case GameManager.Difficulty.Easy:
                undoLimit = int.MaxValue;
                break;

            case GameManager.Difficulty.Medium:
                undoLimit = 2;
                break;

            case GameManager.Difficulty.Hard:
                undoLimit = 0;
                break;
        }
    }
}
