using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed;
    [SerializeField] private RawImage image;
    [SerializeField] private bool unscaledTime;

    private void Update()
    {
        // image.material.mainTextureOffset = new Vector2(image.material.mainTextureOffset.x + scrollSpeed.x * Time.deltaTime, image.material.mainTextureOffset.y + scrollSpeed.y * Time.deltaTime);

        float timeFactor = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        image.uvRect = new Rect(image.uvRect.x + scrollSpeed.x * timeFactor, image.uvRect.y + scrollSpeed.y * timeFactor, image.uvRect.width, image.uvRect.height);
    }
}
