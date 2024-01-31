using System.Collections;
using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField]
    private TextMeshProUGUI _scoreLabel;
    [SerializeField]
    private TextMeshProUGUI _timeLabel;
    [SerializeField]
    private TextMeshProUGUI _killsLabel;
    [SerializeField]
    private TextMeshProUGUI _continueLabel;
    [SerializeField]
    private TextMeshProUGUI _wavesLabel;

    [Header("Counters")]
    [SerializeField]
    private TextMeshProUGUI _scoreCounter;
    [SerializeField]
    private TextMeshProUGUI _timeCounter;
    [SerializeField]
    private TextMeshProUGUI _killsCounter;
    [SerializeField]
    private TextMeshProUGUI _wavesCounter;

    [Header("Preferences")]
    [SerializeField]
    private float _showInterval = 0.4f;
    [SerializeField]
    private float _scoreCountInterval = 0.03f;
    [SerializeField]
    private AudioData _labelShowSound;
    [SerializeField]
    private AudioData _continueSound;
    [SerializeField]
    private AudioData _tickSound;
    [SerializeField]
    private AudioData _reachSound;

    private bool promptName = false;
    private bool canContinue = false;

    private void Start() => StartCoroutine(ShowLabels());

    private void Update()
    {
        if (promptName && Input.anyKeyDown)
        {
            AudioHandler.instance.ProcessAudioData(_continueSound);
            StartCoroutine(PromptName());
            promptName = false;
        }
    }

    private IEnumerator ShowLabels()
    {
        #region Points
        yield return new WaitForSecondsRealtime(_showInterval);

        // enable score
        _scoreLabel.gameObject.SetActive(true);
        _scoreCounter.gameObject.SetActive(true);
        AudioHandler.instance.ProcessAudioData(_labelShowSound);

        int points = 0;
        int targetScore = GameManager.instance.Points;

        while(points < targetScore)
        {
            yield return new WaitForSecondsRealtime(_scoreCountInterval);
            points += 150;
            _scoreCounter.text = $"{points:D7}";
            AudioHandler.instance.ProcessAudioData(_tickSound);
        }

        points = targetScore;
        _scoreCounter.text = $"{points:D7}";
        AudioHandler.instance.ProcessAudioData(_reachSound);
        #endregion

        #region Time
        yield return new WaitForSecondsRealtime(_showInterval);

        // enable score
        _timeLabel.gameObject.SetActive(true);
        _timeCounter.gameObject.SetActive(true);
        AudioHandler.instance.ProcessAudioData(_labelShowSound);

        int time = 0;
        int targetTime = (int)GameManager.instance.TimePlaying;

        while (time < targetTime)
        {
            yield return new WaitForSecondsRealtime(_scoreCountInterval);
            time += 15;
            _timeCounter.text = $"{time:D3}";
            AudioHandler.instance.ProcessAudioData(_tickSound);
        }

        time = targetTime;
        _timeCounter.text = $"{time:D3}";
        AudioHandler.instance.ProcessAudioData(_reachSound);

        #endregion

        #region Kills
        yield return new WaitForSecondsRealtime(_showInterval);

        // enable score
        _killsLabel.gameObject.SetActive(true);
        _killsCounter.gameObject.SetActive(true);
        AudioHandler.instance.ProcessAudioData(_labelShowSound);

        int kills = 0;
        int targetKills = GameManager.instance.Kills;

        while (kills < targetKills)
        {
            yield return new WaitForSecondsRealtime(_scoreCountInterval);
            kills += 5;
            _killsCounter.text = $"{kills:D3}";
            AudioHandler.instance.ProcessAudioData(_tickSound);
        }

        kills = targetKills;
        _killsCounter.text = $"{kills:D3}";
        AudioHandler.instance.ProcessAudioData(_reachSound);

        #endregion

        #region Waves
        yield return new WaitForSecondsRealtime(_showInterval);

        // enable score
        _wavesLabel.gameObject.SetActive(true);
        _wavesCounter.gameObject.SetActive(true);
        AudioHandler.instance.ProcessAudioData(_labelShowSound);

        int waves = 0;
        int targetWaves = GameManager.instance.Waves;

        while (waves < targetWaves)
        {
            yield return new WaitForSecondsRealtime(_scoreCountInterval);
            waves += 3;
            _wavesCounter.text = $"{waves:D3}";
            AudioHandler.instance.ProcessAudioData(_tickSound);
        }

        waves = targetWaves;
        _wavesCounter.text = $"{waves:D3}";
        AudioHandler.instance.ProcessAudioData(_reachSound);
        #endregion

        yield return new WaitForSecondsRealtime(_showInterval);
        _continueLabel.gameObject.SetActive(true);

        promptName = true;
    }

    private IEnumerator PromptName()
    {
        GameManager.instance.FadeInOut();
        yield return new WaitForSecondsRealtime(0.5f);
        PromptForName();
    }

    private void PromptForName()
    {
        if (PersistentData.NamePrompted) {
            UpdateLeaderboard();
            GameManager.instance.RestartLevel();
            return;
        }

        // get the name input class and let the input handle it
        NameInput input = FindObjectOfType<NameInput>();
        input.Prompt();
        input.OnPromptCompleted += PromptComplete;

        // ensure the name doesn't get prompted more than once
        PersistentData.NamePrompted = true;
    }

    /// <summary>
    /// This is called externally from the NameInput class. When the name input is successful, the leaderboard entry will be recorded
    /// </summary>
    /// <param name="name"></param>
    private void PromptComplete(string name)
    {
        PersistentData.Name = name;
        UpdateLeaderboard();

        GameManager.instance.RestartLevel();
        AudioHandler.instance.ProcessAudioData(_continueSound);
    }

    private void UpdateLeaderboard()
    {
        PersistentData.LastScoreValue = GameManager.instance.Points;

        Leaderboard.SetLeaderboardEntry(PersistentData.Name, PersistentData.LastScoreValue);
    }
}
