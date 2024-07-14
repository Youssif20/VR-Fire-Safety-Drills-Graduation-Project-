using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreGetter : MonoBehaviour
{
    public TMP_Text text;
    private void OnEnable()
    {
        var Score = PlayerPrefs.GetFloat("Score", 0);
        if (Score == 0)
        {
            text.text = "";
        }
        else
        {
            PlayerPrefs.SetFloat("Score", 0f);
            text.text = "Your Score is: " + Score;
        }
    }
}
