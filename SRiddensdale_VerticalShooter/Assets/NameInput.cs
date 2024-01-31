using UnityEngine;
using TMPro;

public class NameInput : MonoBehaviour
{
    [SerializeField]
    private GameObject _resultsUI;
    [SerializeField]
    private TextMeshProUGUI _field;
    [SerializeField]
    private TextMeshProUGUI[] _alphabet;
    [SerializeField]
    private RectTransform _cursor;

    [Header("SFX")]
    [SerializeField]
    private AudioData _uiMove;
    [SerializeField]
    private AudioData _uiSelect;
    [SerializeField]
    private AudioData _uiBack;

    public delegate void PromptCompleted(string str);
    public PromptCompleted OnPromptCompleted;

    int index;
    bool prompting = false;
    string name = "";

    private void Update()
    {
        if (!prompting) return;

        int newIndex = index;

        // update index
        if (Input.GetKeyDown(KeyCode.LeftArrow)) newIndex--;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) newIndex++;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(newIndex + 7 <= 26) newIndex += 7;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {

            if (newIndex - 7 >= 0) newIndex -= 7;
        }

        // ensure index isn't out of range
        if (newIndex < 0) newIndex = 26;
        newIndex = newIndex % 26;

        if(newIndex != index) {
            index = newIndex;
            AudioHandler.instance.ProcessAudioData(_uiMove);
        }

        // set cursor position
        _cursor.anchoredPosition = _alphabet[index].rectTransform.anchoredPosition;

        // confirm
        if (Input.GetKeyDown(KeyCode.Z) && name.Length < 6) {
            AudioHandler.instance.ProcessAudioData(_uiSelect);
            name += _alphabet[index].text;
            _field.text = name;
        }
        // delete
        if (Input.GetKeyDown(KeyCode.X) && name.Length > 0) {
            AudioHandler.instance.ProcessAudioData(_uiBack);
            name = name.Substring(0, name.Length - 1);
            _field.text = name;
        }
        if (Input.GetKeyDown(KeyCode.C) && name.Length > 0)
        {
            EnterPrompt();
        }
    }

    public void Prompt()
    {
        _resultsUI.SetActive(true);
        prompting = true;
    }

    public void EnterPrompt()
    {
        OnPromptCompleted?.Invoke(_field.text);
    }
}
