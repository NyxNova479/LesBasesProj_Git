using UnityEngine;

public class Number : MonoBehaviour
{
   
    public float tempsConstruction = 0;

    public int collectedTimes = 0;

    [SerializeField]
    private Spawn spawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner = GameObject.Find("Spawner").GetComponent<Spawn>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float getTempsConstruction()
    {
        return gameObject.GetComponent<Number>().tempsConstruction;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            collectedTimes += 1;
        }
    }
}
