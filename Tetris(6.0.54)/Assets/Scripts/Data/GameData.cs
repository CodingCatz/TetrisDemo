using UnityEngine;

namespace Puzzle.Tetris
{
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
