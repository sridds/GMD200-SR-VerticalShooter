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
    [SerializeField]
    private Transform _heartHolder;

    [Header("Pause Menu")]
    [SerializeField]
    private GameObject _pauseHolder;
    [SerializeField]
    private TextMeshProUGUI[] _pauseMenuSelections;
    [SerializeField]
    private Color _textSelectColor;
    [SerializeField]
    private Color _textDefaultColor;

    [Header("Sounds")]
    [SerializeField]
    private AudioData _pauseSound;
    [SerializeField]
    private AudioData _uiHoverSound;
    [SerializeField]
    private AudioData _uiSelectSound;

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
        _scoreText.text = $"{newPoints:D7}";
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

        if(GameManager.instance.CurrentGameState == GameManager.GameState.Paused)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                pauseMenuIndex++;
                pauseMenuIndex = (pauseMenuIndex % _pauseMenuSelections.Length + _pauseMenuSelections.Length) % _pauseMenuSelections.Length;
                AudioHandler.instance.ProcessAudioData(_uiHoverSound);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                pauseMenuIndex--;
                pauseMenuIndex = (pauseMenuIndex % _pauseMenuSelections.Length + _pauseMenuSelections.Length) % _pauseMenuSelections.Length;
                AudioHandler.instance.ProcessAudioData(_uiHoverSound);
            }

            // update selection if index changed
            if (pauseMenuIndex != index) UpdateSelection();

            // Select
            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (pauseMenuIndex)
                {
                    case 0:
                        GameManager.instance.UpdateGameState();
                        break;
                    case 1:
                        GameManager.instance.RestartLevel();
                        break;
                    case 2:
                        GameManager.instance.ReturnToMenu();
                        break;
                    default:
                        break;
                }

                AudioHandler.instance.ProcessAudioData(_uiSelectSound);
            }
        }
    }

    /// <summary>
    /// Called externally once whenever the game state is changed
    /// </summary>
    /// <param name="state"></param>
    private void UpdatePauseMenu(GameManager.GameState state)
    {
        if(state == GameManager.GameState.Paused)
        {
            _pauseHolder.SetActive(true);
            pauseMenuIndex = 0;
            UpdateSelection();

            AudioHandler.instance.ProcessAudioData(_pauseSound);
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
            GameObject go = Instantiate(_heart, _heartHolder);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, -493);
            healthObjects.Add(go);

            posX -= 64;
        }
    }
}
