using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

/// <summary>
/// Thank you to https://www.youtube.com/watch?v=-O7zeq7xMLw&ab_channel=samyam for the amazing tutorial!
/// </summary>
public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> _names;
    [SerializeField]
    private List<TextMeshProUGUI> _scores;
    [SerializeField]
    private List<TextMeshProUGUI> _ranks;
    [SerializeField]
    private GameObject _leaderboardHolder;
    [SerializeField]
    private GameObject _fetchingText;
    [SerializeField]
    private GameObject _namesHolder;
    [SerializeField]
    private GameObject _scoresHolder;
    [SerializeField]
    private GameObject _rankHolder;

    private const string PUBLIC_KEY = "55dc3e31e7c73aac7ce42cfef7149d8a7e99be5ba055d05afbfd1f9ce727a58c";

    public void ShowLeaderboard()
    {
        _leaderboardHolder.SetActive(true);
        _fetchingText.SetActive(true);

        _scoresHolder.SetActive(false);
        _namesHolder.SetActive(false);
        _rankHolder.SetActive(false);

        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(PUBLIC_KEY, (msg) => {
            _fetchingText.SetActive(false);

            _scoresHolder.SetActive(true);
            _namesHolder.SetActive(true);
            _rankHolder.SetActive(true);

            HandleLeaderboard(msg);
        });
    }

    private void HandleLeaderboard(Dan.Models.Entry[] msg)
    {
        for(int i = 0; i < _names.Count; i++)
        {
            if(i < msg.Length)
            {
                _names[i].text = msg[i].Username;
                _scores[i].text = $"{msg[i].Score:D7}";
                
            }
            else {
                _names[i].text = "------";
                _scores[i].text = "-------";
            }

            _ranks[i].text = $"#{i + 1}";
        }
    }

    public static void SetLeaderboardEntry(string name, int score)
    {
        LeaderboardCreator.UploadNewEntry(PUBLIC_KEY, name, score, (msg) => {});
    }
}
