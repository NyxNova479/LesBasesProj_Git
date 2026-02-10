using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform camPos;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI gameOverUI;
    [SerializeField] private TextMeshProUGUI questionsUI;

    private static Color32 BASECOLOR = new Color(0,0,0,255);
    private static Color32 INVISIBLE = new Color(0,0,0,0);
    private static Color32 CORRECTANSWER = new Color(0,50,0,255);
    private static Color32 WRONGANSWER = new Color(50,0,0,255);

    private float delta = 0;
    private bool isBlinking = false;

    private Dictionary<string, int[]> questions = new Dictionary<string, int[]>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ground.GetComponent<Renderer>().material.color = BASECOLOR;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (camPos.position.y <= 10 && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkAndDie());

        }
        else if (camPos.position.y <= 10) delta = 0;
        else
        {
            delta += Time.deltaTime / 40f;
            camPos.position = new Vector3(camPos.position.x, camPos.position.y - delta, camPos.position.z);
        }
    }

    private IEnumerator BlinkAndDie()
    {

        
        for (int i = 0; i <= 2; i++)
        {

            yield return new WaitForSeconds(0.5f);
            ground.GetComponent<Renderer>().material.color = INVISIBLE;
            yield return new WaitForSeconds(0.5f);
            ground.GetComponent<Renderer>().material.color = BASECOLOR;
        }
        player.GetComponent<NavMeshAgent>().enabled = false;
        ground.GetComponent<NavMeshSurface>().enabled = false;
        ground.SetActive(false);
        StartCoroutine(ShowGameOver());
        
       

    }

    private IEnumerator ShowGameOver()
    {
        for (int i = 0; i <= "Game Over".Length-1; i++)
        {
            gameOverUI.text += "Game Over"[i];
            yield return new WaitForSeconds(0.5f);
        }  
    } 
}
