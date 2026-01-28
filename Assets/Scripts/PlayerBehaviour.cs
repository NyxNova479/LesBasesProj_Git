using UnityEngine;
using UnityEngine.AI;

public class PlayerBehaviour : MonoBehaviour
{

    private NavMeshAgent player;

    [SerializeField]
    private TargetSpawner targetSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<NavMeshAgent>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            player.SetDestination(targetSpawner.currenttarget.position);

        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        Destroy(collision.gameObject);

    }
}
