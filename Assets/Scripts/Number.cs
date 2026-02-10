using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Number : MonoBehaviour
{
    [SerializeField] private NumberData data;


    public int collectedTimes = 0;

    [SerializeField]
    private Spawn spawner;


    private PlayerBehaviour player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner = GameObject.Find("Spawner").GetComponent<Spawn>();
        player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
    }

    public float getTempsConstruction()
    {
        return gameObject.GetComponent<Number>().tempsConstruction;
        
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
