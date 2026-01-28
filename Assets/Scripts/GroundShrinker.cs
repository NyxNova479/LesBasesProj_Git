using UnityEngine;
using System.Collections;

public class GroundShrinker : MonoBehaviour
{

    private float delta;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        delta = Time.deltaTime*2f;
        
        if(gameObject.transform.localScale.x <= 5f) 
        {
            StartCoroutine(BlinkAndDie());
        }
        else
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - delta, gameObject.transform.localScale.y, gameObject.transform.localScale.z - delta);
        }
    }

    private IEnumerator BlinkAndDie()
    {
        for (int i = 0; i <= 2; i++)
        {
            
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 255);
            yield return new WaitForSeconds(0.5f);

        }
        Destroy(gameObject);
    }
}
