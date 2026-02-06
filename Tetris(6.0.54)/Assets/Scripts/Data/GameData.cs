using System.Collections.Generic;
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
            I, O, T, S, Z, L, J, None
        }
        public static Color ActiveColor(Type type = Type.None)
        {
            switch (type)
            {
                default: return Color.gray;
                case Type.I: return Color.cyan;
                case Type.O: return Color.blue;
                case Type.T: return Color.blue + Color.red;
                case Type.S: return Color.green;
                case Type.Z: return Color.red;
                case Type.L: return Color.yellow;
                case Type.J: return Color.red + Color.yellow;
            }
        }
        /// <summary>
        /// [字典]方塊形狀對應座標集合物件本體
        /// </summary>
        private static Dictionary<Type, List<Vector2Int[]>> _rotaTmp;
        /// <summary>
        /// [字典]方塊形狀對外公開存取接口
        /// </summary>
        public static Dictionary<Type, List<Vector2Int[]>> rotaTmp
        {
            get
            {
                if (_rotaTmp == null)
                {
                    InitialCellData();
                }
                return _rotaTmp;
            }
        }
        /// <summary>
        /// 初始化方塊資料
        /// </summary>
        private static void InitialCellData()
        {
            _rotaTmp = new Dictionary<Type, List<Vector2Int[]>>();
            //定義初始形狀(無旋轉)
            Dictionary<Type, Vector2Int[]> baseRota = new Dictionary<Type, Vector2Int[]>()
            {
                {//I型：軸點為底下算來第二格
                    Type.I, new Vector2Int[]
                    {
                        new Vector2Int(0,2),
                        new Vector2Int(0,1),
                        new Vector2Int(0,0),//軸點
                        new Vector2Int(0,-1)
                    }
                },
                {//O型：不轉動
                    Type.O, new Vector2Int[]
                    {
                        new Vector2Int(1,1),
                        new Vector2Int(0,1),
                        new Vector2Int(0,0),//軸點
                        new Vector2Int(1,0)
                    }
                },
                {//T型：
                    Type.T, new Vector2Int[]
                    {
                        new Vector2Int(0,1),
                        new Vector2Int(0,0),//軸點
                        new Vector2Int(-1,0),
                        new Vector2Int(1,0)
                    }
                },
                {//S型：
                    Type.S, new Vector2Int[]
                    {
                        new Vector2Int(0,1),
                        new Vector2Int(1,1),
                        new Vector2Int(-1,0),
                        new Vector2Int(0,0)//軸點
                    }
                },
                {//Z型：
                    Type.Z, new Vector2Int[]
                    {
                        new Vector2Int(-1,1),
                        new Vector2Int(0,1),
                        new Vector2Int(0,0),//軸點
                        new Vector2Int(1,0)
                    }
                },
                {//L型：
                    Type.L, new Vector2Int[]
                    {
                        new Vector2Int(0,2),
                        new Vector2Int(0,1),
                        new Vector2Int(0,0),//軸點
                        new Vector2Int(1,0)
                    }
                },
                {//J型：
                    Type.J, new Vector2Int[]
                    {
                        new Vector2Int(0,2),
                        new Vector2Int(0,1),
                        new Vector2Int(0,0),//軸點
                        new Vector2Int(-1,0)
                    }
                }
            };
            //為每個形狀產生一組四個轉向的模板
            foreach (var tmp in baseRota)
            {
                Type type = tmp.Key;//字典鍵值
                Vector2Int[] orgRota = tmp.Value;//字典資料
                //計算後的結果(4種轉向)
                List<Vector2Int[]> list = new List<Vector2Int[]>();
                //運算邏輯
                list.Add(orgRota);//原始數據
                for (int r = 1; r < 4; r++)
                {
                    Vector2Int[] nextRota = new Vector2Int[orgRota.Length];
                    for (int i = 0; i < orgRota.Length; i++)
                    {//旋轉公式 (y,-x)
                        nextRota[i].x = orgRota[i].y;
                        nextRota[i].y = -orgRota[i].x;
                    }
                    list.Add(nextRota);//加入清單內
                    orgRota = nextRota;//下一次轉動初始替換
                }
                _rotaTmp.Add(type, list);
            }
        }
        #endregion 規格訊息

        #region 公開資訊接口
        /// <summary>
        /// 棋盤寬
        /// </summary>
        public static int BoardWidth { get; private set; }
        /// <summary>
        /// 棋盤高
        /// </summary>
        public static int BoardHeight { get; private set; }
        /// <summary>
        /// 遊戲棋盤二維陣列(複數集合物件)
        /// </summary>
        public static Brick[,] Board { get; private set; }
        /// <summary>
        /// 預覽區寬
        /// </summary>
        public const int NextWidth = 3;
        /// <summary>
        /// 域覽區高
        /// </summary>
        public const int NextHeight = 4;
        /// <summary>
        /// 預覽區二維陣列
        /// </summary>
        public static Brick[,] NextUI { get; private set; }
        #endregion 公開資訊接口

        #region 建構式
        /// <summary>
        /// 建構式(初始化class用)
        /// </summary>
        public GameData()
        {
            BoardWidth = 10;
            BoardHeight = 20;
            Board = new Brick[BoardWidth, BoardHeight];
            NextUI = new Brick[NextWidth, NextHeight];
        }

        /// <summary>
        /// 建構式(可自訂初始值版本)
        /// </summary>
        /// <param name="width">寬</param>
        /// <param name="height">高</param>
        public GameData(int width, int height)
        {
            BoardWidth = width;
            BoardHeight = height;
            Board = new Brick[BoardWidth, BoardHeight];
            NextUI = new Brick[NextWidth, NextHeight];
        }
        #endregion 建構式

        #region 初始化遊戲資料
        /// <summary>
        /// 設定(建立)棋盤格上的磚
        /// </summary>
        /// <param name="x">座標X</param>
        /// <param name="y">座標Y</param>
        /// <param name="brick">磚塊實體</param>
        public void SetBrick(int x, int y, Brick brick)
        {
            Board[x, y] = brick;
            //為了辨識容易將每個Brick依座標命名
            brick.Initial($"Brick({x},{y})");
        }
        /// <summary>
        /// 設定(建立)棋盤格上的磚
        /// </summary>
        /// <param name="x">座標X</param>
        /// <param name="y">座標Y</param>
        /// <param name="brick">磚塊實體</param>
        public void SetNextUI(int x, int y, Brick brick)
        {
            NextUI[x, y] = brick;
            //為了辨識容易將每個Brick依座標命名
            brick.Initial($"Brick({x},{y})");
        }
        #endregion 初始化遊戲資料

        /// <summary>
        /// 隨機取得一個方塊形狀
        /// </summary>
        /// <returns>方塊形狀</returns>
        public Type RandomType()
        {
            return (Type)Random.Range(0, 7);
        }

        #region Brick狀態操作相關
        /// <summary>
        /// 取得特定位置磚塊的狀態
        /// </summary>
        /// <param name="pos">定位</param>
        /// <returns>磚塊的狀態</returns>
        public static Brick.State GetBrickState(Vector2Int pos)
        {
            return Board[pos.x, pos.y].state;
        }

        /// <summary>
        /// 清除Brick的佔用狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public static void SetBrickStateToNone(Vector2Int pos)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.None, ActiveColor());
        }
        /// <summary>
        /// 清除Brick的佔用狀態
        /// </summary>
        /// <param name="x">座標X</param>
        /// <param name="y">座標Y</param>
        public static void SetBrickStateToNone(int x, int y)
        {
            Board[x, y].ChangeState(Brick.State.None, ActiveColor());
        }

        /// <summary>
        /// 設定Brick的暫存狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public static void SetBrickStateToExist(Vector2Int pos, Type type)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.Exist, ActiveColor(type));
        }

        /// <summary>
        /// 設定Brick的佔用狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public static void SetBrickStateToOccupied(Vector2Int pos, Type type)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.Occupied, ActiveColor(type));
        }
        public static void SetBrickStateToDead(Vector2Int pos, Color color)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.Occupied, color);
        }
        #endregion Brick狀態操作相關

        #region NextUI狀態操作相關
        /// <summary>
        /// 設定NextUI的暫存狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public static void SetNextUIToExist(Vector2Int pos, Type type)
        {
            NextUI[pos.x, pos.y].ChangeState(Brick.State.Exist, ActiveColor(type));
        }
        /// <summary>
        /// 清除NextUI的暫存狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public static void SetNextUIToNone(Vector2Int pos)
        {
            NextUI[pos.x, pos.y].ChangeState(Brick.State.None, ActiveColor());
        }
        #endregion NextUI狀態操作相關


        #region 消除邏輯
        /// <summary>
        /// 確認磚塊連線消除
        /// </summary>
        public static void CheckClearLines()
        {
            for (int y = 0; y < BoardHeight;)
            {
                if (IsLineFull(y))
                {//該橫排是否填滿
                    //清除指定橫排
                    ClearLine(y);
                    //整體磚塊資料下降
                    ShiftRowsDown(y);
                }
                else y++;
            }
        }
        /// <summary>
        /// 檢查特定Y橫排是否滿線
        /// </summary>
        /// <param name="y">Y橫排值</param>
        /// <returns>是否滿線</returns>
        private static bool IsLineFull(int y)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                if (Board[x, y].state != Brick.State.Occupied) return false;
            }
            return true;
        }
        /// <summary>
        /// 清除特定Y橫排
        /// </summary>
        /// <param name="y">Y橫排值</param>
        private static void ClearLine(int y)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                SetBrickStateToNone(x, y);
            }
        }
        /// <summary>
        /// 從起始排Y以上磚塊資料下移
        /// </summary>
        /// <param name="startY">起始排Y值</param>
        private static void ShiftRowsDown(int startY)
        {
            for (int y = startY; y < BoardHeight - 1; y++)
            {//頂排不用做移動所以 -1
                for (int x = 0; x < BoardWidth; x++)
                {//將上一排狀態轉移至這排
                    Board[x, y].ChangeState(Board[x, y + 1]);
                }
            }
            ClearLine(BoardHeight - 1);//最後一排清除
        }
        #endregion 消除邏輯
    }
}

namespace Puzzle.Match3
{
    public class GameData
    {
        //不同類型的遊戲後端資料
    }

}
