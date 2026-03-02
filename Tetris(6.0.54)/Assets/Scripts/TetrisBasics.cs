using System;
using System.Threading;
using System.Threading.Tasks;
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
        private const int LV_RANGE = 10;
        /// <summary>
        /// 經由分數計算出來的遊戲等級
        /// </summary>
        private int _level
        {
            get
            {//級距：10
                return Math.Min(_clearRows / LV_RANGE, 9);
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
        /// 速降冷卻計時
        /// </summary>
        private float downTimer;
        /// <summary>
        /// 遊戲進行成績
        /// </summary>
        private int _score;
        /// <summary>
        /// 累積消除行數
        /// </summary>
        private int _clearRows;
        /// <summary>
        /// 下次落磚時間(間隔)
        /// </summary>
        private float _nextDropTime;
        /// <summary>
        /// 磚塊鎖死的時間(間隔)
        /// </summary>
        private float _lockTime;
        /// <summary>
        /// 遊戲是否初始完成
        /// </summary>
        private bool _isReady;
        /// <summary>
        /// 磚塊觸底
        /// </summary>
        private bool _isGrounded;
        /// <summary>
        /// 狀態鎖：遊戲(落磚)循環中
        /// </summary>
        private bool _isProcessing;
        /// <summary>
        /// 狀態鎖：遊戲是否結束
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
        /// 執行左右移動
        /// </summary>
        private bool LRMove => MoveDir != 0;
        /// <summary>
        /// 第一次按下移動操作
        /// </summary>
        private bool FirstMove => moveCount < 0;
        /// <summary>
        /// 移動是否可以被觸發
        /// </summary>
        private bool MoveTrigger => moveTimer >= MOVE_SWD + (FirstMove ? 0 : moveCount * MOVE_CD);
        /// <summary>
        /// 執行速降操作
        /// </summary>
        private bool FastDown => Math.Sign(Input.GetAxis("Vertical")) < 0;
        /// <summary>
        /// 快速下降是否可以被觸發
        /// </summary>
        private bool FastDownTrigger => downTimer >= MOVE_CD;
        /// <summary>
        /// 當前操作中方塊組合是否存活
        /// </summary>
        private bool BrickAlive => _currentBrick.isAlive;
        /// <summary>
        /// 遊戲速率(共10級)
        /// </summary>
        private int GameSpeed => M_SEC - (LV * 100);
        /// <summary>
        /// 落磚時間間隔
        /// </summary>
        private float DropTimeGap => GameSpeed / 1000f;
        /// <summary>
        /// 磚塊是否觸底鎖定
        /// </summary>
        private bool IsLocked => _isGrounded && Time.time >= _lockTime;
        /// <summary>
        /// [拒絕任何人為操作]遊戲是否正在處裡核心狀態操作
        /// </summary>
        private bool IsCoreLock => _isGameOver || _isProcessing;
        /// <summary>
        /// 是否到達下次落磚時間
        /// </summary>
        private bool IsNextDrop => Time.time >= _nextDropTime;
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
            GameStart();
        }

        /// <summary>
        /// 執行玩家操作偵測
        /// </summary>
        private void Update()
        {
            if (IsCoreLock) return;

            MoveInput();

            //旋轉
            if (Input.GetKeyDown(KeyCode.W))
            {
                TryRota();
            }

            //瞬降(硬降)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HardDrop();
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
        /// 1000毫秒(1秒)
        /// </summary>
        private const int M_SEC = 1000;
        /// <summary>
        /// [調速]速度等級(倍率：一個單位5)
        /// </summary>
        [Range(0,9)]
        public int LV = 0;

        /// <summary>
        /// 下一個的磚塊組資料
        /// </summary>
        private BrickData _nextBrick;
        /// <summary>
        /// 當前操作中的磚塊組資料
        /// </summary>
        private BrickData _currentBrick;

        /// <summary>
        /// 產生新方塊組
        /// </summary>
        private void SpawnBrick()
        {
            _currentBrick.SetData(SPAWN_X, SPAWN_Y, _nextBrick.type);
            RandomNextBrick();
            //滅頂邏輯
            if (!_currentBrick.IsValid())
            {
                _isGameOver = true;
            }
            else
            {
                _currentBrick.UpdateBrickState();
                CheckGrounded();
                ResetDropTimer();
            }
        }

        /// <summary>
        /// 觸底鎖定 & 產生新掉落
        /// </summary>
        private void LockNextDrop()
        {
            _isProcessing = true;
            //Lock
            _currentBrick.Lock();
            _currentBrick.UpdateBrickState();
            GameData.CheckClearLines(ClearRows);
            //NextDrop
            SpawnBrick();
            _isProcessing = false;
        }

        /// <summary>
        /// 清除行數累進(等級提升計算)
        /// </summary>
        /// <param name="rows">清除行數</param>
        private void ClearRows(int rows)
        {
            _clearRows += rows;
            if (LV < _level) LV = _level;
        }

        /// <summary>
        /// 隨機下一組磚塊資料
        /// </summary>
        private void RandomNextBrick()
        {
            if (_isReady) _nextBrick.ClearNextBrick();
            _nextBrick.SetData(NEXT_X, NEXT_Y, data.RandomType());
            _nextBrick.UpdateNextBrick();
        }

        /// <summary>
        /// 移動輸入
        /// </summary>
        private void MoveInput()
        {
            if (LRMove)
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

            if (FastDown)
            {//下降(加速)
                if (FastDownTrigger)
                {
                    if (TryMove(Vector2Int.down))
                    {//成功移動：強制完成一個間隔
                        ResetDropTimer();
                    }
                    downTimer = 0;
                }
                downTimer += Time.deltaTime;
            }
            else
            {//放開S
                downTimer = 0;
            }
        }
        /// <summary>
        /// 各種可能的踢牆位移
        /// </summary>
        private readonly Vector2Int[] WALL_KICK_OFFSETS
            = new Vector2Int[]
        {
            Vector2Int.zero,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.left * 2,
            Vector2Int.right * 2,
            Vector2Int.up * 2,
        };

        /// <summary>
        /// 嘗試旋轉方塊組合
        /// </summary>
        private void TryRota()
        {
            BrickData tmp = _currentBrick;//影Brick
            //模擬旋轉
            tmp.Rota();
            //踢牆偏移量遍歷(找到第一個合法位置)
            foreach (Vector2Int offest in WALL_KICK_OFFSETS)
            {
                BrickData tmpKick = tmp;//影tmpBrick
                tmpKick.Move(offest);
                //不穿牆不卡磚
                if (tmpKick.IsValid())
                {
                    _currentBrick.ClearBrickState();
                    _currentBrick = tmpKick;//套用影tmpBrick
                    _currentBrick.UpdateBrickState();
                    //觸底檢測
                    CheckGrounded();//任何的移動都要檢查接下來是否撞擊
                    break;
                }
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
                CheckGrounded();//任何的移動都要檢查接下來是否撞擊
                return true;
            }
            return false;
        }

        /// <summary>
        /// 方塊下墜
        /// </summary>
        private void DropBrick()
        { //自然下墜
            _isProcessing = true;
            if (TryMove(Vector2Int.down))
            {
                ResetDropTimer();
            }
            _isProcessing = false;
        }

        /// <summary>
        /// 硬降：瞬間到位
        /// </summary>
        private void HardDrop()
        {
            _isProcessing = true;//上鎖
            //硬降過程
            _currentBrick.ClearBrickState();//清除舊視覺
            _currentBrick = _currentBrick.GhostData;//落點偵測
            LockNextDrop();

            _isProcessing = false;//解鎖
        }

        /// <summary>
        /// 檢查磚塊是否觸底(計算可極限操作的時間)
        /// </summary>
        private void CheckGrounded()
        {
            BrickData tmp = _currentBrick;//影Brick
            tmp.Move(Vector2Int.down);//模擬下落
            if (tmp.IsValid())
            {//不穿牆不卡磚
                _isGrounded = false;
            }
            else
            {
                _isGrounded = true;
                _lockTime = Time.time + DropTimeGap;
            }
        }

        /// <summary>
        /// 執行滅頂動態
        /// </summary>
        private void DieOut()
        {
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
        #endregion 遊戲邏輯控制

        #region 遊戲流程控制
        /// <summary>
        /// 執行序的識別碼
        /// </summary>
        private CancellationTokenSource _CTS;
        /// <summary>
        /// 遊戲流程開始(產生CTS)
        /// </summary>
        private async void GameStart()
        {
            _CTS?.Cancel();//萬一已存在舊的先刪除
            _CTS = new CancellationTokenSource();
            //初始化遊戲
            await InitialGame(_CTS.Token);
            //開始遊戲迴圈
            await GameLoop(_CTS.Token);
        }

        /// <summary>
        /// 初始化遊戲
        /// </summary>
        private async Task InitialGame(CancellationToken token)
        {
            //準備預覽UI
            for (int y = 0; y < NextHeight; y++)
            {//巢狀迴圈：3 * 4 次
                for (int x = 0; x < NextWidth; x++)
                {
                    //棋盤[指定的座標] = 具現化物件到特定目標
                    data.SetNextUI(x, y, Instantiate(brickTMP, nextUI));
                }
            }
            //下一組磚塊資料
            RandomNextBrick();

            //FOR迴圈：起始值;終點值;迭代值;
            for (int y = 0; y < Height; y++)
            {//巢狀迴圈：10 * 20 次
                for (int x = 0; x < Width; x++)
                {
                    //棋盤[指定的座標] = 具現化物件到特定目標
                    data.SetBrick(x, y, Instantiate(brickTMP, boardUI));
                    await Task.Yield();
                }
            }
            _score = 0;
            _isGameOver = false;
            _isReady = true;
            SpawnBrick();//產生第一塊磚
        }

        /// <summary>
        /// [異步]遊戲迴圈
        /// </summary>
        /// <returns>執行任務</returns>
        private async Task GameLoop(CancellationToken token)
        {
            while (_isReady)
            {
                await Task.Yield();//逐幀運行

                if (IsGameOver)
                {
                    DieOut();
                    await Task.Delay(M_SEC / Height, token);
                    continue;//省略後續檢測
                }

                if (IsLocked)
                {//檢查：是否觸底鎖定
                    LockNextDrop();
                }
                else if (IsNextDrop) 
                {//檢查：落磚時間是否已到
                    DropBrick();
                }
            }
        }

        /// <summary>
        /// 重設落磚計時器
        /// </summary>
        private void ResetDropTimer()
        {
            _nextDropTime = Time.time + DropTimeGap;
        }

        /// <summary>
        /// 遊戲摧毀(關閉)
        /// </summary>
        private void OnDestroy()
        {
            _CTS?.Cancel();//取消任務
            _CTS?.Dispose();//釋放資源(記憶體)
            _CTS = null;
        }
        #endregion 遊戲流程控制
    }
}
