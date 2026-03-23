using Puzzle.Tetris;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    #region 音樂庫
    public AudioClip bgmGamePlay;
    #endregion 音樂庫

    #region UI元件
    /// <summary>
    /// 總排分數文字元件(顯示1~10名)
    /// </summary>
    public Text leaderBoardText;
    #endregion UI元件

    private LeaderBoardData data => ScoreManager.LoadLeaderBoard();

    #region 更新邏輯
    private void OnEnable()
    {
        AudioManager.Instance.PlayBGM(bgmGamePlay, 0.5f);
        RefreshUI();
        ScoreManager.OnLeaderBoardUpdated += RefreshUI;
    }

    private void OnDisable()
    {
        ScoreManager.OnLeaderBoardUpdated -= RefreshUI;
    }

    public void RefreshUI(int highlightIndex = -1)
    {
        if (leaderBoardText)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("---- TOP 10 ----");//加入第一行(標頭)

            for (int i = 0; i < 10; i++)
            {
                if (i < data.topScores.Count)
                {//有分數紀錄
                    if(highlightIndex == i) 
                        sb.AppendLine($"<color=red>No.{i + 1:00} {data.topScores[i]:00000000}</color>");
                    else sb.AppendLine($"No.{i + 1:00} {data.topScores[i]:00000000}");
                }
                else
                {//無分填補
                    sb.AppendLine($"No.{i + 1:00} 00000000");
                }
            }

            leaderBoardText.text = sb.ToString();
        }
    }
    #endregion 更新邏輯
}
