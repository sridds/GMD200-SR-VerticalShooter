using UnityEngine;
using TMPro;

public class NameInput : MonoBehaviour
{
    [SerializeField]
    private GameObject _resultsUI;
    [SerializeField]
    private TMP_InputField _field;

    public delegate void PromptCompleted(string str);
    public PromptCompleted OnPromptCompleted;

    public void Prompt()
    {
        _resultsUI.SetActive(true);
    }

    public void EnterPrompt()
    {
        _resultsUI.SetActive(false);
        OnPromptCompleted?.Invoke(_field.text);
    }
}
