using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Playing,
        Paused
    }

    // Accessors
    public int Points { get; private set; }
    public float TimePlaying { get; private set; }
    public GameState CurrentGameState { get; private set; }
    public Player ActivePlayer { get { if (cachedPlayer == null) cachedPlayer = FindAnyObjectByType<Player>(); return cachedPlayer; } }

    // Delegates
    public delegate void PointUpdate(int oldPoints, int newPoints);
    public PointUpdate OnPointUpdate;

    public delegate void GameStateChanged(GameState state);
    public GameStateChanged OnGameStateChanged;

    // fields
    private float timeScaleBeforePause;
    private Player cachedPlayer;

    private void Awake() => instance = this;

    private void Start() => Time.timeScale = 1;

    private void Update()
    {
        // increment time playing variable as long as game is not paused
        if(CurrentGameState != GameState.Paused)
            TimePlaying += Time.unscaledDeltaTime;

        if (Input.GetKeyDown(KeyCode.Escape)) UpdateGameState();
    }

    /// <summary>
    /// Handles the updating of the game  state
    /// </summary>
    private void UpdateGameState()
    {
        if (CurrentGameState == GameState.Playing) PauseGame();
        else if (CurrentGameState == GameState.Paused) UnpauseGame();

        // invoke the event if any are subscribed
        OnGameStateChanged?.Invoke(CurrentGameState);
    }

    /// <summary>
    /// Handles pausing the game
    /// </summary>
    private void PauseGame()
    {
        CurrentGameState = GameState.Paused;

        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Handles unpausing the game
    /// </summary>
    private void UnpauseGame()
    {
        CurrentGameState = GameState.Playing;
        Time.timeScale = timeScaleBeforePause;
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

    /// <summary>
    /// calls the lerp time scale coroutine to lerp timeScale over a period lerpTime
    /// </summary>
    /// <param name="timeScale"></param>
    /// <param name="lerpTime"></param>
    public void SetTimeScale(float timeScale, float lerpTime)
    {
        StopAllCoroutines();
        StartCoroutine(LerpTimeScale(timeScale, lerpTime));
    }

    /// <summary>
    /// Instantly sets time scale. This may seem redundant but I want the game manager to handle this in case pausing ever gets implemented
    /// </summary>
    /// <param name="timeScale"></param>
    public void SetTimeScaleInstant(float timeScale)
    {
        if (CurrentGameState == GameState.Paused) return;
        Time.timeScale = timeScale;
    }

    /// <summary>
    /// Responsible for lerping the time scale to the passed timeScale variable over a period defined by lerpTime
    /// </summary>
    /// <param name="timeScale"></param>
    /// <param name="lerpTime"></param>
    /// <returns></returns>
    private IEnumerator LerpTimeScale(float timeScale, float lerpTime)
    {
        float elapsedTime = 0.0f;
        float initialTimeScale = Time.timeScale;

        while(elapsedTime < lerpTime)
        {
            if(CurrentGameState == GameState.Paused)
            {
                // ensure the timescale is 0 while paused
                Time.timeScale = 0.0f;
                yield return null;
            }
            else
            {
                // adjust the time scale, ensure that elapsed time uses unscaled time since we are directly modifying the time scale
                Time.timeScale = Mathf.Lerp(initialTimeScale, timeScale, elapsedTime / lerpTime);
                elapsedTime += Time.unscaledDeltaTime;

                yield return null;
            }
        }

        // just in case it overshoots or undershoots
        Time.timeScale = timeScale;
    }
}
