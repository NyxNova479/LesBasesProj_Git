using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] public TextMeshProUGUI _movesUI;

    public int moveLimit = 4;
    public int moveCount = 0;

    public Transform currenttarget;

    // Structure imposée : STACK
    public Stack<Target> pile = new Stack<Target>();

    private GameObject targetObject;

    void Start()
    {
        currenttarget = transform;
        UpdateMovesUI();
    }

    void Update()
    {
        HandleInput();
        UpdateCurrentTarget();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && moveCount < moveLimit)
        {
            moveCount++;
            UpdateMovesUI();
            SpawnTargetAtClick();
        }
    }

    private void SpawnTargetAtClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetObject = Instantiate(targetPrefab, hit.point, Quaternion.identity);
            AddToStack(targetObject);
        }
    }

    private void AddToStack(GameObject target)
    {
        Target targetComponent = target.GetComponent<Target>();

        if (targetComponent != null)
        {
            pile.Push(targetComponent);
        }
    }

    private void UpdateCurrentTarget()
    {
        if (pile.Count > 0)
        {
            currenttarget = pile.Peek().transform;
        }
        else
        {
            currenttarget = transform;
        }
    }

    private void UpdateMovesUI()
    {
        _movesUI.text = (moveLimit - moveCount).ToString();
    }

    // 🔹 Appelé par GameManager quand difficulté change
    public void SetMoveLimit(int newLimit)
    {
        moveLimit = newLimit;
        moveCount = 0;
        UpdateMovesUI();
    }

    // 🔹 Reset après bonne réponse
    public void ResetMoves()
    {
        moveCount = 0;
        pile.Clear();
        UpdateMovesUI();
    }
}
