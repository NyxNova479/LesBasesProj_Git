using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform camPos;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject player;

    private float delta = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        delta += Time.deltaTime/40f;
        camPos.position = new Vector3(camPos.position.x, camPos.position.y - delta,camPos.position.z);
        if(camPos.position.y <= 10)
        {
            delta = 0;
            StartCoroutine(BlinkAndDie());
        }
    }

    private IEnumerator BlinkAndDie()
    {
        for (int i = 0; i <= 2; i++)
        {


            yield return new WaitForSeconds(1f);
            ground.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(1f);
            ground.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 255);

        }
        player.GetComponent<NavMeshAgent>().enabled = false;
        ground.GetComponent<NavMeshSurface>().enabled = false;
        ground.SetActive(false);
    }
}
