using UnityEngine;

public class Number : MonoBehaviour
{
    [SerializeField] private NumberData data;

    private PlayerBehaviour player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerBehaviour>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (player != null && data != null)
        {
            player.AddNumber(data);
        }

        Destroy(gameObject);
    }
}
