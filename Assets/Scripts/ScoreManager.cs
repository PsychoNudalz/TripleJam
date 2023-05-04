using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private int score = 0;
    [SerializeField]
    


    public static ScoreManager current;

    public static int Score => current.score;

    private void Awake()
    {
        current = this;
    }

    public static void AddScore(int s)
    {
        if (!current)
        {
            return;
        }
        current.score += s;
        UIManager.current.UpdateScore(current.score);
    }
}
