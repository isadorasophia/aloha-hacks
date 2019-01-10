using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateScriptInner : MonoBehaviour
{
    // Start is called before the first frame update

    private int score;
    public Text scoreText;

    void Start()
    {
        score = 0;
        ScoreUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter( Collider other )
    {
       if (other.gameObject.CompareTag("Ball"))
        {
            Debug.Log("WE GOT A HIT");
            score += 1;
            ScoreUpdate();
        }
    }

    void ScoreUpdate()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
