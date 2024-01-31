using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    private float _waitTime = 0.5f;
    [SerializeField]
    private Animator _startTextAnimation;
    [SerializeField]
    private Animator _leaderboardTextAnimation;
    [SerializeField]
    private RectTransform _shipSelector;
    [SerializeField]
    private Animator _fade;
    [SerializeField]
    private float _menuHoldTime = 3.0f;
    [SerializeField]
    private GameObject _menuElements;
    [SerializeField]
    private AudioData _selectSound;
    [SerializeField]
    private AudioData _menuSelectSound;
    [SerializeField]
    private AudioData _leaderboardSelect;
    [SerializeField]
    private AudioData _backSound;
    [SerializeField]
    private GameObject _whiteFlash;
    [SerializeField]
    private Leaderboard _leaderboard;

    private bool canSelect = false;
    private bool leaderboardOpen = false;
    int selection = 0;

    private void Start()
    {
        Time.timeScale = 1.0f;
        _menuElements.SetActive(false);
        Invoke(nameof(CanSelect), _waitTime);
    }

    private void CanSelect()
    {
        _menuElements.SetActive(true);
        canSelect = true;

        _startTextAnimation.SetBool("OnText", true);
        _leaderboardTextAnimation.SetBool("OnText", false);
    }

    private void Update()
    {
        int newSelection = selection;

        if (canSelect) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                if(selection == 0)
                {
                    StartCoroutine(StartGame());
                    canSelect = false;
                    _whiteFlash.SetActive(true);
                }
                if(selection == 1)
                {
                    StartCoroutine(LeaderboardOpen());
                    canSelect = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                newSelection = 1;

            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                newSelection = 0;
            }
        }

        if(leaderboardOpen && Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(LeaderboardClose());
        }


        if(selection != newSelection) {
            AudioHandler.instance.ProcessAudioData(_menuSelectSound);

            if(newSelection == 0) {
                _startTextAnimation.SetBool("OnText", true);
                _leaderboardTextAnimation.SetBool("OnText", false);
                _shipSelector.anchoredPosition = new Vector2(_shipSelector.anchoredPosition.x, _shipSelector.anchoredPosition.y + 64);

            }
            else {
                _startTextAnimation.SetBool("OnText", false);
                _leaderboardTextAnimation.SetBool("OnText", true);
                _shipSelector.anchoredPosition = new Vector2(_shipSelector.anchoredPosition.x, _shipSelector.anchoredPosition.y - 64);
            }
        }
        selection = newSelection;
    }

    private IEnumerator StartGame()
    {
        // pause music and play select sound
        AudioHandler.instance.PauseMusic();
        Time.timeScale = 0.0f;
        AudioHandler.instance.ProcessAudioData(_selectSound);

        _startTextAnimation.SetBool("Select", true);
        yield return new WaitForSecondsRealtime(_menuHoldTime);
        // fade out 
        _fade.SetBool("FadeOutBool", true);
        yield return new WaitForSecondsRealtime(0.5f);
        // load game
        SceneManager.LoadScene(1);
    }

    private IEnumerator LeaderboardOpen()
    {
        AudioHandler.instance.ProcessAudioData(_leaderboardSelect);
        _fade.SetBool("FadeOutBool", true);
        yield return new WaitForSecondsRealtime(0.5f);
        _leaderboard.ShowLeaderboard();
        _fade.SetBool("FadeOutBool", false);
        leaderboardOpen = true;
    }

    private IEnumerator LeaderboardClose()
    {
        AudioHandler.instance.ProcessAudioData(_backSound);
        _fade.SetBool("FadeOutBool", true);
        yield return new WaitForSecondsRealtime(0.5f);
        _leaderboard.HideLeaderboard();
        _fade.SetBool("FadeOutBool", false);
        leaderboardOpen = false;
        canSelect = true;
    }
}
