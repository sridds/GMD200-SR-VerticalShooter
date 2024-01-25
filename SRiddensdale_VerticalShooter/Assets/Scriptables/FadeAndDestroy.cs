using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float fadeTime = 0.4f;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0.0f;
        float alpha = 1.0f;
        float step = 0.0f;

        while(elapsed < fadeTime)
        {
            step = Mathf.Lerp(alpha, 0.0f, elapsed / fadeTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, step);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
