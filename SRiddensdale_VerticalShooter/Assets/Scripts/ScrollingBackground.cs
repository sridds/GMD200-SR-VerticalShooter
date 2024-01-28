using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    public enum MoveMode
    {
        Scroll,
        Circle
    }
    [Header("Move Settings")]
    [SerializeField] private MoveMode mode;
    [SerializeField] private bool unscaledTime;

    [ShowIf(nameof(mode), MoveMode.Scroll)]
    [SerializeField] private Vector2 scrollSpeed;

    [ShowIf(nameof(mode), MoveMode.Circle)]
    [SerializeField] private float sinAmt;
    [ShowIf(nameof(mode), MoveMode.Circle)]
    [SerializeField] private float sinSpeed;
    [ShowIf(nameof(mode), MoveMode.Circle)]
    [SerializeField] private float cosAmt;
    [ShowIf(nameof(mode), MoveMode.Circle)]
    [SerializeField] private float cosSpeed;

    [Header("References")]
    [SerializeField] private RawImage image;

    private void Update()
    {
        // image.material.mainTextureOffset = new Vector2(image.material.mainTextureOffset.x + scrollSpeed.x * Time.deltaTime, image.material.mainTextureOffset.y + scrollSpeed.y * Time.deltaTime);

        float timeFactor = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (mode == MoveMode.Scroll)
        {
            image.uvRect = new Rect(image.uvRect.x + scrollSpeed.x * timeFactor, image.uvRect.y + scrollSpeed.y * timeFactor, image.uvRect.width, image.uvRect.height);
        }
        else if (mode == MoveMode.Circle)
        {
            float sinValue = Mathf.Sin(Time.time * sinSpeed) * sinAmt;
            float cosValue = Mathf.Cos(Time.time * cosSpeed) * cosAmt;

            image.uvRect = new Rect(image.uvRect.x + cosValue * timeFactor, image.uvRect.y + sinValue * timeFactor, image.uvRect.width, image.uvRect.height);
        }
    }
}
