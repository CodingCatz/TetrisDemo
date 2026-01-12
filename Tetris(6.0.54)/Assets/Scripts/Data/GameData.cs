namespace Puzzle.Tetris
{
    public class GameData
    {
        public int boardWidth;
        public int boardHeight;

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
    }

}

namespace Puzzle.Match3
{
    public class GameData
    {
        //不同類型的遊戲後端資料
    }

}
