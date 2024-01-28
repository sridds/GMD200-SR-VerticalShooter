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
    private AudioData _selectSound;

    private bool canSelect = false;

    private void Start()
    {
        Time.timeScale = 1.0f;
        Invoke(nameof(CanSelect), _waitTime);
    }

    private void CanSelect() => canSelect = true;

    private void Update()
    {
        if (canSelect) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                StartCoroutine(StartGame());
                canSelect = false;
            }
        }
    }

    private IEnumerator StartGame()
    {
        // pause music and play select sound
        AudioHandler.instance.PauseMusic();
        AudioHandler.instance.ProcessAudioData(_selectSound);
        _textAnimation.SetBool("Select", true);
        yield return new WaitForSeconds(_menuHoldTime);
        // fade out 
        _fade.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        // load game
        SceneManager.LoadScene(1);
    }
}
