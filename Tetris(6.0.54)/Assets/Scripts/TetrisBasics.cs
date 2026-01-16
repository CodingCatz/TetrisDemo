using System;
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
        
        #endregion 遊戲核心資料結構

        #region 生命週期
        private void Start()
        {
            //初始化遊戲
            InitialGame();
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
                    _gameBoard[x, y].Initial($"Brick({x},{y})");
                    //委託清除顏色功能到Action
                    ClearAllBricks += _gameBoard[x, y].ClearColor;
                }
            }
        }

        /// <summary>
        /// 以每秒跳動50次的固定更新週期刷新畫面
        /// </summary>
        private void FixedUpdate()
        {
            _timeCounter++;//計算畫面更新
            //Debug.Log(_timeCounter);
            if (_timeCounter >= GameSpeed)
            {
                _timeCounter = 0;
                Debug.Log("畫面刷新");
                DropBrick();
            }
        }
        #endregion 生命週期

        #region 狀態數據
        /// <summary>
        /// 判定是否需要產生方塊組合
        /// </summary>
        private bool SpawnBrick => !_currentBrick.isAlive;
        /// <summary>
        /// 遊戲速率(共10級)
        /// </summary>
        private int GameSpeed => COUNTER_TH - speed * 5;
        #endregion 狀態數據

        #region 遊戲邏輯控制
        /// <summary>
        /// [常數]方塊出生座標X
        /// </summary>
        private const int SPAWN_X = 4;
        /// <summary>
        /// [常數]方塊出生座標Y
        /// </summary>
        private const int SPAWN_Y = 19;
        /// <summary>
        /// [常數]更新計數器閾值
        /// </summary>
        private const int COUNTER_TH = 50;
        /// <summary>
        /// [調速]速度等級(倍率：一個單位5)
        /// </summary>
        [Range(0,9)]
        public int speed = 0;
        /// <summary>
        /// 更新計數器
        /// </summary>
        private int _timeCounter;
        /// <summary>
        /// 下個出現的方塊形狀
        /// </summary>
        private GameData.Type _nextBrickType;
        /// <summary>
        /// 當前操作中的方塊資料
        /// </summary>
        private BrickData _currentBrick;
        /// <summary>
        /// 所有Brick的ClearColor功能集合
        /// </summary>
        private Action ClearAllBricks;

        /// <summary>
        /// 方塊下墜
        /// </summary>
        private void DropBrick()
        {
            if (SpawnBrick)
            {//產生新方塊組
                _currentBrick.SetData(SPAWN_X, SPAWN_Y, _nextBrickType);
                _nextBrickType = data.RandomType();
            }
            else
            {//原方塊組下落
                _currentBrick.Fall();
            }
            //視覺更新
            ValidCells();
        }
        /// <summary>
        /// 可視化棋盤Cells
        /// </summary>
        private void ValidCells()
        {
            //取得相對應的方塊Cells座標
            Vector2Int[] cells = GameData.CalCells(_currentBrick);
            bool valid = true;
            //先檢查是否有需要更新視覺
            foreach (Vector2Int cell in cells)
            {
                //1.左右超界檢查
                //2.觸底檢查
                if (cell.y < 0)
                {
                    valid = false;
                    break;
                }
            }
            //阻止更新
            if (!valid) return;
            //統一清除所有方塊顏色
            ClearAllBricks();
            //FOREACH迴圈 (單一類型 in 該類型的集合)
            foreach (Vector2Int cell in cells)
            {//計算對應錨點後所有Cell實際位置
                
                if (cell.y < data.boardHeight)
                {//避免超出的座標被渲染
                    _gameBoard[cell.x, cell.y].ActiveColor();
                }
            }
        }
        #endregion 遊戲邏輯控制
    }
}
