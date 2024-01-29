using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    private float _waitTime = 0.5f;
    [SerializeField]
    private Animator _textAnimation;
    [SerializeField]
    private Animator _fade;
    [SerializeField]
    private float _menuHoldTime = 3.0f;
    [SerializeField]
    private GameObject _menuElements;
    [SerializeField]
    private AudioData _selectSound;
    [SerializeField]
    private GameObject _whiteFlash;

    private bool canSelect = false;

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
    }

    private void Update()
    {
        if (canSelect) {
            if (Input.anyKeyDown) {
                StartCoroutine(StartGame());
                canSelect = false;
                _whiteFlash.SetActive(true);
            }
        }
    }

    private IEnumerator StartGame()
    {
        // pause music and play select sound
        AudioHandler.instance.PauseMusic();
        Time.timeScale = 0.0f;
        AudioHandler.instance.ProcessAudioData(_selectSound);

        _textAnimation.SetBool("Select", true);
        yield return new WaitForSecondsRealtime(_menuHoldTime);
        // fade out 
        _fade.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(0.5f);
        // load game
        SceneManager.LoadScene(1);
    }
}
