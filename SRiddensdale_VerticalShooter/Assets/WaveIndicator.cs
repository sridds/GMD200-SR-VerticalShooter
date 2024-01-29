using TMPro;
using UnityEngine;
using System.Collections;

public class WaveIndicator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private GameObject _contents;

    [SerializeField]
    private Animator _boxAnim;

    private void Start()
    {
        _boxAnim.gameObject.SetActive(false);
        _contents.SetActive(false);
    }

    public void CallWaveIndicator(int wave)
    {
        // set to wave text
        StartCoroutine(ShowIndicator(wave));
    }

    public IEnumerator ShowIndicator(int wave)
    {
        _boxAnim.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        _contents.SetActive(true);
        _text.text = "- WAVE " + wave + " -";

        yield return new WaitForSeconds(2f);

        _contents.SetActive(false);
        _boxAnim.SetTrigger("Exit");
        yield return new WaitForSeconds(0.4f);
        _boxAnim.gameObject.SetActive(false);
    }
}
