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

    private const string PUBLIC_KEY = "55dc3e31e7c73aac7ce42cfef7149d8a7e99be5ba055d05afbfd1f9ce727a58c";

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(PUBLIC_KEY, (msg) => {
            HandleLeaderboard(msg);
        });
    }

    private void HandleLeaderboard(Dan.Models.Entry[] msg)
    {
        for(int i = 0; i < _names.Count; i++)
        {
            _names[i].text = msg[i].Username;
            _scores[i].text = msg[i].Score.ToString();
        }
    }

    public static void SetLeaderboardEntry(string name, int score)
    {
        LeaderboardCreator.UploadNewEntry(PUBLIC_KEY, name, score, (msg) => {});
    }
}
