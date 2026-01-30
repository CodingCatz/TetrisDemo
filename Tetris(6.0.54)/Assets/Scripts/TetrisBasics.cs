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

        #region 遊戲核心介面
        /// <summary>
        /// 磚塊模板物件
        /// </summary>
        public Brick brickTMP;
        /// <summary>
        /// 棋盤載體UI
        /// </summary>
        public Transform boardUI;
        #endregion 遊戲核心介面

        #region 狀態數據
        /// <summary>
        /// 棋盤寬
        /// </summary>
        private int Width => GameData.BoardWidth;
        /// <summary>
        /// 棋盤高
        /// </summary>
        private int Height => GameData.BoardHeight;
        /// <summary>
        /// 當前操作中方塊組合是否存活
        /// </summary>
        private bool BrickAlive => _currentBrick.isAlive;
        /// <summary>
        /// 遊戲速率(共10級)
        /// </summary>
        private int GameSpeed => COUNTER_TH - speed * 5;
        /// <summary>
        /// 遊戲是否結束
        /// </summary>
        private bool IsGameOver => _isGameOver;
        #endregion 狀態數據

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

            //FOR迴圈：起始值;終點值;迭代值;
            for (int y = 0; y < Height; y++)
            {//巢狀迴圈：10 * 20 次
                for (int x = 0; x < Width; x++)
                {
                    //棋盤[指定的座標] = 具現化物件到特定目標
                    data.SetBrick(x, y, Instantiate(brickTMP, boardUI));
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

        /// <summary>
        /// 執行玩家操作偵測
        /// </summary>
        private void Update()
        {
            if (IsGameOver) return;
            //左移
            if (Input.GetKey(KeyCode.A))
            {
                TryMove(Vector2Int.left);
            }
            //右移
            if (Input.GetKey(KeyCode.D))
            {
                TryMove(Vector2Int.right);
            }
            //下降(加速)

            //旋轉
            if (Input.GetKeyDown(KeyCode.W))
            {
                TryRota();
            }
        }
        #endregion 生命週期

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
        /// 嘗試旋轉方塊組合
        /// </summary>
        private void TryRota()
        {
            BrickData tmp = _currentBrick;//影Brick
            //模擬位移
            tmp.Rota();
            //不穿牆不卡磚
            if (tmp.IsValid())
            {
                _currentBrick.ClearBrickState();
                _currentBrick = tmp;//套用影Brick
                _currentBrick.UpdateBrickState();
            }
        }

        /// <summary>
        /// 嘗試移動方塊組合
        /// </summary>
        /// <param name="offset">操作的偏移量</param>
        private bool TryMove(Vector2Int offset)
        {
            BrickData tmp = _currentBrick;//影Brick
            //模擬位移
            tmp.Move(offset);
            //不穿牆不卡磚
            if (tmp.IsValid())
            {
                _currentBrick.ClearBrickState();
                _currentBrick = tmp;//套用影Brick
                _currentBrick.UpdateBrickState();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 方塊下墜
        /// </summary>
        private void DropBrick()
        {
            if (!BrickAlive)
            {//產生新方塊組
                _currentBrick.SetData(SPAWN_X, SPAWN_Y, _nextBrickType);
                _nextBrickType = data.RandomType();
                //滅頂邏輯
            }
            else
            {//自然下墜
                if (!TryMove(Vector2Int.down))
                {//下墜移動失敗：產生撞擊
                    _currentBrick.Lock();
                    _currentBrick.UpdateBrickState();
                }
            }
        }
        #endregion 遊戲邏輯控制
    }
}
