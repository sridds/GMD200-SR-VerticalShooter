using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int Points { get; private set; }
    public float TimePlaying { get; private set; }

    public delegate void PointUpdate(int oldPoints, int newPoints);
    public PointUpdate OnPointUpdate;

    private void Update()
    {
        TimePlaying += Time.unscaledDeltaTime;
    }

    /// <summary>
    /// Adds a point amount to the current points
    /// </summary>
    /// <param name="pointAmt"></param>
    public void AddPoints(int pointAmt)
    {
        Points += pointAmt;
        OnPointUpdate?.Invoke(Points - pointAmt, Points);
    }
}
