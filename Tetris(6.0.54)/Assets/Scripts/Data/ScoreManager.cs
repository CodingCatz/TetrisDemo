using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Tetris
{
    #region 排行榜資料封裝結構
    /// <summary>
    /// [序列化]排行榜資料封裝結構
    /// </summary>
    [Serializable]
    public class LeaderBoardData
    {
        public List<int> topScores = new List<int>();
    }
    #endregion 排行榜資料封裝結構

    /// <summary>
    /// 本地排行榜管理
    /// </summary>
    public static class ScoreManager
    {
        #region 基本常數
        /// <summary>
        /// 存檔 Key 值
        /// </summary>
        private const string SCORE_DATA_KEY = "LeaderBoard";
        /// <summary>
        /// 最大分數儲存數量(筆數)
        /// </summary>
        private const int MAX_RECORDS = 10;
        #endregion 基本常數

        public static Action OnLeaderBoardUpdated;

        /// <summary>
        /// 讀取排行榜資料
        /// </summary>
        /// <returns>排行榜資料</returns>
        public static LeaderBoardData LoadLeaderBoard()
        {
            if (PlayerPrefs.HasKey(SCORE_DATA_KEY))
            {//資料復原
                string json = PlayerPrefs.GetString(SCORE_DATA_KEY);
                //Debug.Log(json);
                return JsonUtility.FromJson<LeaderBoardData>(json);
            }
            return new LeaderBoardData();
        }
        /// <summary>
        /// 送出儲存分數
        /// </summary>
        /// <param name="score">分數</param>
        public static void SubmitScore(int score)
        {
            if (score <= 0) return;//避免無效紀錄
            //先嘗試取得舊的資料
            LeaderBoardData data = LoadLeaderBoard();
            data.topScores.Add(score);//加入成績
            data.topScores.Sort((a, b) => b.CompareTo(a));//排序1~11筆
            if (data.topScores.Count > MAX_RECORDS)
            {//移除超出上限數量的分數
                data.topScores.RemoveRange(MAX_RECORDS, data.topScores.Count - MAX_RECORDS);
            }
            //轉回JSON儲存
            string json = JsonUtility.ToJson(data);
            //Debug.Log(json);
            PlayerPrefs.SetString(SCORE_DATA_KEY, json);
            PlayerPrefs.Save();//確保儲存完成
            //發報廣播給訂閱者
            OnLeaderBoardUpdated?.Invoke();
        }
    }
}