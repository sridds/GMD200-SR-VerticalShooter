using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private GameObject _heart;

    private List<GameObject> healthObjects = new List<GameObject>();

    private void Start()
    {
        // subscribe to events
        GameManager.instance.OnPointUpdate += UpdateScoreText;
        GameManager.instance.ActivePlayer.PlayerHealth.OnHealthUpdate += UpdateHearts;
        GameManager.instance.ActivePlayer.PlayerHealth.OnHealthDepleted += DeactivateHearts;

        CreateHearts();
    }

    private void UpdateScoreText(int oldPoints, int newPoints)
    {
        if (newPoints > 9999999) newPoints = 9999999;
        // update the score text accordingly
        _scoreText.text = $"SCORE:\n{newPoints:D7}";
    }

    /// <summary>
    /// update hearts based on health
    /// </summary>
    /// <param name="oldHealth"></param>
    /// <param name="newHealth"></param>
    private void UpdateHearts(int oldHealth, int newHealth)
    {
        healthObjects[newHealth].SetActive(false);
    }

    /// <summary>
    /// deactivates all hearts
    /// </summary>
    private void DeactivateHearts()
    {
        foreach(GameObject g in healthObjects)
        {
            g.SetActive(false);
        }
    }

    private void CreateHearts()
    {
        int posX = 544;

        // creates as many hearts the player has
        for(int i = 0; i < GameManager.instance.ActivePlayer.PlayerHealth.MaxHealth; i++)
        {
            GameObject go = Instantiate(_heart, transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, -493);
            healthObjects.Add(go);

            posX -= 64;
        }
    }
}
