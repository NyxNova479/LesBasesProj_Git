using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TargetSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject targetPrefab;

    private Transform mousePosition;
    public Transform currenttarget;

    public Stack<Target> pile = new Stack<Target>();

    [SerializeField]
    private Camera _mainCamera;

    private GameObject targetObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currenttarget = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
        try
        {

            currenttarget = pile.Peek().transform;

        }



        catch (InvalidOperationException)
        {
            Debug.Log("Aucune destination en vue!");
        }

    }

    private void HandleClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0))
        {
            targetObject = Instantiate(targetPrefab, hit.point, Quaternion.identity);
            AjouterALaPile(targetObject);
            
        }
    }

    private void AjouterALaPile(GameObject target)
    {

        pile.Push(target.GetComponent<Target>());

        
    }
}
