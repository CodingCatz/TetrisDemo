using UnityEngine;

namespace Puzzle.Tetris
{
    /// <summary>
    /// [結構]方塊資料組合
    /// </summary>
    public struct BrickData
    {
        /// <summary>
        /// 錨點X座標
        /// </summary>
        public int x;
        /// <summary>
        /// 錨點Y座標
        /// </summary>
        public int y;
        /// <summary>
        /// 形狀類型
        /// </summary>
        public GameData.Type type;

        /// <summary>
        /// 設定初始狀態
        /// </summary>
        /// <param name="x">起始X</param>
        /// <param name="y">起始Y</param>
        /// <param name="type">形狀</param>
        public void SetData(int x, int y, GameData.Type type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }

    public class GameData
    {
        #region 規格訊息
        /// <summary>
        /// 方塊種類(形狀)的列舉
        /// </summary>
        public enum Type
        {
            I, O, T, S, Z, L, J
        }
        /// <summary>
        /// 棋盤寬
        /// </summary>
        public int boardWidth;
        /// <summary>
        /// 棋盤高
        /// </summary>
        public int boardHeight;
        #endregion 規格訊息

        #region 建構式
        /// <summary>
        /// 建構式(初始化class用)
        /// </summary>
        public GameData() 
        {
            boardWidth = 10;
            boardHeight = 20;
        }

        /// <summary>
        /// 建構式(可自訂初始值版本)
        /// </summary>
        /// <param name="width">寬</param>
        /// <param name="height">高</param>
        public GameData(int width, int height)
        {
            boardWidth = width;
            boardHeight = height;
        }
        #endregion 建構式

        /// <summary>
        /// 隨機取得一個方塊形狀
        /// </summary>
        /// <returns>方塊形狀</returns>
        public Type RandomType()
        {
            return (Type)Random.Range(0, 7);
        }
    }

}

namespace Puzzle.Match3
{
    public class GameData
    {
        //不同類型的遊戲後端資料
    }

}
