using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;


    private void Start()
    {
        // subscribe to events
        GameManager.instance.OnPointUpdate += UpdateScoreText;
    }

    private void UpdateScoreText(int oldPoints, int newPoints)
    {
        if (newPoints > 9999999) newPoints = 9999999;
        // update the score text accordingly
        _scoreText.text = $"SCORE:\n{newPoints:D7}";
    }
}
