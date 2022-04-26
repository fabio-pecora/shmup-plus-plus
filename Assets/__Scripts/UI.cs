using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI shield;
    public float level = Hero._shieldLevel;
    // Start is called before the first frame update
    void StartLevel()
    {
        UpdateGUI();
    }

    // Update is called once per frame
    void UpdateGUI()
    {
        
        score.text = "Score: " + Enemy.score;
        shield.text = "Shield Level = " + Hero._shieldLevel;
    }

    void Update()
    {
        UpdateGUI();
    }
}
