using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBehaviour : MonoBehaviour
{

    private NavMeshAgent player;

    [SerializeField]
    private TargetSpawner targetSpawner;

    Dictionary<Number, int> dico;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dico = new Dictionary<Number, int>();
        player = GetComponent<NavMeshAgent>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && targetSpawner.currenttarget != null)
        {
            player.SetDestination(targetSpawner.currenttarget.position);

        }
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
            if (dico.ContainsKey(collision.GetComponent<Number>()))
            {
                int numbCount = collision.GetComponent<Number>().collectedTimes++;
                dico[collision.GetComponent<Number>()] = numbCount;
            }
            else
            {
                dico.Add(collision.GetComponent<Number>(), collision.GetComponent<Number>().collectedTimes);
            }
            Destroy(collision.gameObject);
        }

    }
}
