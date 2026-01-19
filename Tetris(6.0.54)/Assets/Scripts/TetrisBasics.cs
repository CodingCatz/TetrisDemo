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
                    UpdateBricks += _gameBoard[x, y].UpdateColor;
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
        /// 當前操作中方塊組合是否存活
        /// </summary>
        private bool BrickAlive => _currentBrick.isAlive;
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
        private const int SPAWN_Y = 20;
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
        /// 所有Brick的UpdateColor功能集合
        /// </summary>
        private Action UpdateBricks;

        /// <summary>
        /// 方塊下墜
        /// </summary>
        private void DropBrick()
        {
            if (!BrickAlive)
            {//產生新方塊組
                _currentBrick.SetData(SPAWN_X, SPAWN_Y, _nextBrickType);
                _nextBrickType = data.RandomType();
            }
            else if (CheckCells(GameData.CalCells(_currentBrick, Vector2Int.down)))
            {//原方塊組下落：先清除原本位置狀態
                ClearCells(GameData.CalCells(_currentBrick));
                _currentBrick.Fall();
            }
            //視覺更新
            ValidCells(GameData.CalCells(_currentBrick));
        }
        /// <summary>
        /// 撞擊確認
        /// </summary>
        /// <param name="cells">方塊組座標陣列</param>
        /// <returns>是否可以通過</returns>
        private bool CheckCells(Vector2Int[] cells)
        {
            bool pass = true;
            //先檢查是否能更新視覺
            foreach (Vector2Int cell in cells)
            {
                if (cell.y >= data.boardHeight) continue;
                //0.左右超界檢查
                //1.觸底檢查(預判) or 2.觸碰堆疊
                if (cell.y < 0 || _gameBoard[cell.x, cell.y].state == Brick.State.Occupied)
                {
                    _currentBrick.Lock();
                    pass = false;
                    break;
                }
            }
            return pass;
        }
        /// <summary>
        /// 清除狀態
        /// </summary>
        /// <param name="cells">方塊組座標陣列</param>
        private void ClearCells(Vector2Int[] cells)
        {
            foreach (Vector2Int cell in cells)
            {//continue；略過超出範圍的cell
                if (cell.y >= data.boardHeight) continue;
                _gameBoard[cell.x, cell.y].ChangeState(Brick.State.None);
            }
        }

        /// <summary>
        /// 可視化棋盤Cells
        /// </summary>
        private void ValidCells(Vector2Int[] cells)
        {
            //更新磚塊狀態
            foreach (Vector2Int cell in cells)
            {
                if (cell.y >= data.boardHeight) continue;
                //三元運算：if => ?，else => :
                _gameBoard[cell.x, cell.y].ChangeState(BrickAlive ? Brick.State.Exist : Brick.State.Occupied);
            }
            //統一更新所有方塊顏色
            UpdateBricks();
        }
        #endregion 遊戲邏輯控制
    }
}
