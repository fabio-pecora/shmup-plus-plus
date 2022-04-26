using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighScore : MonoBehaviour
{

    static public int score = 1000;
    public TextMeshProUGUI highscore;

    private void Awake()
    {
        //If the PlayerPrefs HighScore already exists, read it!
        if (PlayerPrefs.HasKey("HighScore"))
        {
            score = PlayerPrefs.GetInt("HighScore");
        }
        //Assign the high score to HighScore
        PlayerPrefs.SetInt("HighScore", score);
    }

    // Update is called once per frame
    void Update()
    {
        highscore.text = "High Score: " + score;

        //Update the PlayerPrefs HighScore if necessary
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

    }
}
