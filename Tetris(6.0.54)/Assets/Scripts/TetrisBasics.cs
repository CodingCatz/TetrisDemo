using UnityEngine;//使用 XXXXXX命名空間

//命名空間(程式資料夾的概念) 第一層名稱(.的)次一層名稱
namespace Puzzle.Tetris
{
    //公開權限 類別 名稱 (:繼承) Unity基礎類別
    public class TetrisBasics : MonoBehaviour
    {
        #region 基礎資料
        /// <summary>
        /// [靜態]data資料物件實體
        /// </summary>
        private static GameData _data;
        /// <summary>
        /// [靜態]公開取用的data物件(唯讀)
        /// </summary>
        public static GameData data
        {
            get
            {
                if (_data == null)
                {//如果(資料實體 不存在) 建立新的
                    _data = new GameData();
                }
                return _data;
            }
        }
        /// <summary>
        /// 級距常數
        /// </summary>
        private const int LV_RANGE = 1000;
        /// <summary>
        /// 經由分數計算出來的遊戲等級
        /// </summary>
        private int _level
        {
            get
            {//級距：1000
                return _score / LV_RANGE;
            }
        }
        /// <summary>
        /// 遊戲進行成績
        /// </summary>
        private int _score;
        /// <summary>
        /// 遊戲是否結束
        /// </summary>
        private bool _isGameOver;
        #endregion 基礎資料

        #region 遊戲核心資料結構
        /// <summary>
        /// 磚塊模板物件
        /// </summary>
        public Brick brickTMP;
        /// <summary>
        /// 棋盤載體UI
        /// </summary>
        public Transform boardUI;
        /// <summary>
        /// 遊戲棋盤二維陣列(複數集合物件)
        /// </summary>
        private Brick[,] _gameBoard;
        /// <summary>
        /// 下個出現的方塊形狀
        /// </summary>
        private GameData.Type _nextBrickType;
        /// <summary>
        /// 當前操作中的方塊形狀
        /// </summary>
        private GameData.Type _currentBrickType;
        #endregion 遊戲核心資料結構


        private void Start()
        {
            //初始化遊戲
            InitialGame();
            Debug.Log(_nextBrickType);
        }

        /// <summary>
        /// 初始化遊戲
        /// </summary>
        private void InitialGame()
        {
            _nextBrickType = data.RandomType();
            _score = 0;
            _isGameOver = false;
            _gameBoard = new Brick[data.boardWidth, data.boardHeight];

            //FOR迴圈：起始值;終點值;迭代值;
            for (int y = 0; y < data.boardHeight; y++)
            {//巢狀迴圈：10 * 20 次
                for (int x = 0; x < data.boardWidth; x++)
                {
                    //棋盤[指定的座標] = 具現化物件到特定目標
                    _gameBoard[x, y] = Instantiate(brickTMP, boardUI);
                    //為了辨識容易將每個Brick依座標命名
                    _gameBoard[x, y].name = $"Brick({x},{y})";
                }
            }
        }
    }
}
