using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Number : MonoBehaviour
{
    [SerializeField] private NumberData data;




    [SerializeField]
    private SpawnNumbers spawner;


    private PlayerBehaviour player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner = GameObject.Find("Spawner").GetComponent<SpawnNumbers>();
        player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
    }



    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.tag == "Player")
        {
            
            StoreNumber(gameObject.GetComponent<Number>());
            Destroy(gameObject);

        }
    }

    private void StoreNumber(Number number)
    {

        player.AddNumber(data);
    }
}
