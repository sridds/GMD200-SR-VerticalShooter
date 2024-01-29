using TMPro;
using UnityEngine;

public class SpecialText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    public void SetText(string txt) => _text.text = txt;
}
