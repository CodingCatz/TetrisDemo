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
            I, O, T, S, Z, L, J
        }
        /// <summary>
        /// 當前正在操作的方塊組類型
        /// </summary>
        public static Type currentType { get; private set; }
        /// <summary>
        /// 設定當前操作方塊類型
        /// </summary>
        /// <param name="type">方塊類型</param>
        public static void SetCurrentType(Type type)
        {
            currentType = type;
        }
        /// <summary>
        /// 預設顏色
        /// </summary>
        public static Color orgColor = Color.gray;
        /// <summary>
        /// 使用中的方塊顏色
        /// </summary>
        public static Color activeColor
        {
            get
            {
                return ActiveColor(currentType);
            }
        }
        public static Color ActiveColor(Type type)
        {
            switch (type)
            {
                default: return orgColor;
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
            Board[pos.x, pos.y].ChangeState(Brick.State.None);
        }

        /// <summary>
        /// 設定Brick的暫存狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public static void SetBrickStateToExist(Vector2Int pos)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.Exist);
        }

        /// <summary>
        /// 設定Brick的佔用狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public static void SetBrickStateToOccupied(Vector2Int pos)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.None);
        }
        #endregion Brick狀態操作相關
    }
}

namespace Puzzle.Match3
{
    public class GameData
    {
        //不同類型的遊戲後端資料
    }

}
