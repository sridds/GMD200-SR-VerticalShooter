using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameManager;

public class GameHUD : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private GameObject _heart;

    [Header("Pause Menu")]
    [SerializeField]
    private GameObject _pauseHolder;
    [SerializeField]
    private TextMeshProUGUI[] _pauseMenuSelections;
    [SerializeField]
    private Color _textSelectColor;
    [SerializeField]
    private Color _textDefaultColor;

    int pauseMenuIndex;

    private List<GameObject> healthObjects = new List<GameObject>();

    private void Start()
    {
        // subscribe to events
        GameManager.instance.OnPointUpdate += UpdateScoreText;
        GameManager.instance.ActivePlayer.PlayerHealth.OnHealthUpdate += UpdateHearts;
        GameManager.instance.ActivePlayer.PlayerHealth.OnHealthDepleted += DeactivateHearts;
        GameManager.instance.OnGameStateChanged += UpdatePauseMenu;

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

    private void Update()
    {
        int index = pauseMenuIndex;

        if(GameManager.instance.CurrentGameState == GameState.Paused)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                pauseMenuIndex++;
                pauseMenuIndex = (pauseMenuIndex % _pauseMenuSelections.Length + _pauseMenuSelections.Length) % _pauseMenuSelections.Length;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                pauseMenuIndex--;
                pauseMenuIndex = (pauseMenuIndex % _pauseMenuSelections.Length + _pauseMenuSelections.Length) % _pauseMenuSelections.Length;
            }

            // update selection if index changed
            if (pauseMenuIndex != index) UpdateSelection();
        }
    }

    /// <summary>
    /// Called externally once whenever the game state is changed
    /// </summary>
    /// <param name="state"></param>
    private void UpdatePauseMenu(GameState state)
    {
        if(state == GameState.Paused)
        {
            _pauseHolder.SetActive(true);
            pauseMenuIndex = 0;
            UpdateSelection();
        }
        else
        {
            _pauseHolder.SetActive(false);
        }
    }

    /// <summary>
    /// Updates whichever option is selected
    /// </summary>
    private void UpdateSelection()
    {
        for(int i = 0; i < _pauseMenuSelections.Length; i++)
        {
            // update color of selection
            if (i == pauseMenuIndex) _pauseMenuSelections[i].color = _textSelectColor;
            else _pauseMenuSelections[i].color = _textDefaultColor;
        }
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
