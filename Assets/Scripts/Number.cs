using Unity.VisualScripting;
using UnityEngine;

public class Number : MonoBehaviour
{
   
    public float tempsConstruction = 0;

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

        if(collision.gameObject.tag == "Player")
        {
            collectedTimes += 1;
            player.UpdateInventoryUI(gameObject.GetComponent<Number>());
            StoreNumber(gameObject.GetComponent<Number>());
            player.UpdateInventoryUI(gameObject.GetComponent<Number>());
            gameObject.SetActive(false);
        }
    }

    private void StoreNumber(Number number)
    {
        if (player.inventory.ContainsKey(number))
        {
            player.inventory[number] += 1 ;
            collectedTimes = player.inventory[number];
        }
        else
        {
            player.inventory.Add(number, number.collectedTimes);
        }

    }
}
