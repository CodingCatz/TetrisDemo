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
        /// [操作]首次按住移動觸發的時間延遲(SWitchDelay)
        /// </summary>
        private const float MOVE_SWD = 0.2f;
        /// <summary>
        /// [操作]按住移動後持續觸發的時間間隔(CoolDown)
        /// </summary>
        private const float MOVE_CD = 0.05f;
        /// <summary>
        /// 移動冷卻計時
        /// </summary>
        private float moveTimer;
        /// <summary>
        /// 移動觸發次數
        /// </summary>
        private int moveCount;
        /// <summary>
        /// 遊戲進行成績
        /// </summary>
        private int _score;
        /// <summary>
        /// 遊戲是否初始完成
        /// </summary>
        private bool _isReady;
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
        /// 棋盤面板UI
        /// </summary>
        public Transform boardUI;
        /// <summary>
        /// 磚塊預覽面板UI
        /// </summary>
        public Transform nextUI;
        /// <summary>
        /// 磚塊陣亡
        /// </summary>
        public Color brickDead;
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
        /// 預覽區寬
        /// </summary>
        private int NextWidth => GameData.NextWidth;
        /// <summary>
        /// 預覽區高
        /// </summary>
        private int NextHeight => GameData.NextHeight;
        /// <summary>
        /// 左右(A/D)操作數值
        /// </summary>
        private int MoveDir => Math.Sign(Input.GetAxis("Horizontal"));
        /// <summary>
        /// 第一次按下移動操作
        /// </summary>
        private bool FirstMove => moveCount < 0;
        /// <summary>
        /// 移動是否可以被觸發
        /// </summary>
        private bool MoveTrigger => moveTimer >= MOVE_SWD + (FirstMove ? 0 : moveCount * MOVE_CD);
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

        /// <summary>
        /// 掃描線座標
        /// </summary>
        Vector2Int scanPos;

        #region 生命週期
        private void Start()
        {
            //初始化遊戲
            InitialGame();
        }

        /// <summary>
        /// 隨機下一組磚塊資料
        /// </summary>
        private void RandonNextBrick()
        {
            if (_isReady) _nextBrick.ClearNextBrick();
            _nextBrick.SetData(NEXT_X, NEXT_Y, data.RandomType());
            _nextBrick.UpdateNextBrick();
        }

        /// <summary>
        /// 初始化遊戲
        /// </summary>
        private void InitialGame()
        {
            _score = 0;
            _isGameOver = false;

            for (int y = 0; y < NextHeight; y++)
            {//巢狀迴圈：3 * 4 次
                for (int x = 0; x < NextWidth; x++)
                {
                    //棋盤[指定的座標] = 具現化物件到特定目標
                    data.SetNextUI(x, y, Instantiate(brickTMP, nextUI));
                }
            }
            //下一組磚塊資料
            RandonNextBrick();

            //FOR迴圈：起始值;終點值;迭代值;
            for (int y = 0; y < Height; y++)
            {//巢狀迴圈：10 * 20 次
                for (int x = 0; x < Width; x++)
                {
                    //棋盤[指定的座標] = 具現化物件到特定目標
                    data.SetBrick(x, y, Instantiate(brickTMP, boardUI));
                }
            }

            _isReady = true;
        }

        /// <summary>
        /// 移動輸入
        /// </summary>
        private void MoveInput()
        {
            if (MoveDir != 0)
            {//按下移動：A/D
                if (MoveTrigger)
                {//觸發移動
                    if (TryMove(Vector2Int.right * MoveDir))
                    {//檢查是否為按下A/D後第一次移動
                        if (FirstMove) moveTimer = 0;//贈送第一次移動
                        moveCount++;//正式計算連續移動次數
                    }
                }
                moveTimer += Time.deltaTime;
            }
            else
            {//放開A/D
                moveTimer = MOVE_SWD;//立刻冷卻連續移動延遲
                moveCount = -1;//立刻重置連續移動次數(避免每次冷卻重置)
            }
        }

        /// <summary>
        /// 執行滅頂動態
        /// </summary>
        private void DieOut()
        {
            if (_timeCounter < 5) return;
            _timeCounter = 0;//計時重置
            if (scanPos.y < Height)
            {//掃描線從最底往上淹沒
                for (int x = 0; x < Width; x++)
                {//Y相同的一橫排變死色
                    scanPos.x = x;
                    GameData.SetBrickStateToDead(scanPos, brickDead);
                }
                scanPos.y++;//跳至下一排
            }
        }

        /// <summary>
        /// 以每秒跳動50次的固定更新週期刷新畫面
        /// </summary>
        private void FixedUpdate()
        {
            _timeCounter++;//計算畫面更新
            if (IsGameOver)
            {
                DieOut();
            }
            else 
            {
                DropBrick();
            }
        }

        /// <summary>
        /// 執行玩家操作偵測
        /// </summary>
        private void Update()
        {
            if (IsGameOver) return;

            MoveInput();

            //下降(加速)
            if (Input.GetKeyDown(KeyCode.S))
            {

            }
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
        /// [常數]預覽中心點座標X
        /// </summary>
        private const int NEXT_X = 1;
        /// <summary>
        /// [常數]預覽中心點座標Y
        /// </summary>
        private const int NEXT_Y = 1;
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
        /// 下一個的方塊資料
        /// </summary>
        private BrickData _nextBrick;
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
            if (_timeCounter < GameSpeed) return;
            _timeCounter = 0;//計時重置
            if (!BrickAlive)
            {//產生新方塊組
                _currentBrick.SetData(SPAWN_X, SPAWN_Y, _nextBrick.type);
                RandonNextBrick();
                //滅頂邏輯
                if (!_currentBrick.IsValid())
                {
                    _isGameOver = true;
                }
            }
            else
            {//自然下墜
                if (!TryMove(Vector2Int.down))
                {//下墜移動失敗：產生撞擊
                    _currentBrick.Lock();
                }
            }
            _currentBrick.UpdateBrickState();
        }
        #endregion 遊戲邏輯控制
    }
}
