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
        /// 複製預設方塊類型資料
        /// </summary>
        /// <param name="type">方塊類型</param>
        /// <param name="pos">定位</param>
        /// <returns>方塊類型資料</returns>
        public static Vector2Int[] CloneCells(Type type, Vector2Int pos)
        {
            Vector2Int[] calCells = cells[type].Clone() as Vector2Int[];
            for (int i = 0; i < calCells.Length; i++)
            {
                calCells[i] += pos;
            }
            return calCells;
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
        private static Dictionary<Type, Vector2Int[]> _cells;
        /// <summary>
        /// [字典]方塊形狀對外公開存取接口
        /// </summary>
        public static Dictionary<Type, Vector2Int[]> cells
        {
            get
            {
                if (_cells == null)
                {
                    InitialCellData();
                }
                return _cells;
            }
        }
        /// <summary>
        /// 初始化方塊資料
        /// </summary>
        private static void InitialCellData()
        {
            _cells = new Dictionary<Type, Vector2Int[]>();
            //I型：軸點為底下算來第二格
            _cells.Add(Type.I, new Vector2Int[]
            {
                new Vector2Int(0,2),
                new Vector2Int(0,1),
                new Vector2Int(0,0),//軸點
                new Vector2Int(0,-1)
            });
            //O型：
            _cells.Add(Type.O, new Vector2Int[]
            {
                new Vector2Int(1,1),
                new Vector2Int(0,1),
                new Vector2Int(0,0),//軸點
                new Vector2Int(1,0)
            });
            //T型：
            _cells.Add(Type.T, new Vector2Int[]
            {
                new Vector2Int(0,1),
                new Vector2Int(0,0),//軸點
                new Vector2Int(-1,0),
                new Vector2Int(1,0)
            });
            //S型：
            _cells.Add(Type.S, new Vector2Int[]
            {
                new Vector2Int(0,1),
                new Vector2Int(1,1),
                new Vector2Int(-1,0),
                new Vector2Int(0,0)//軸點
            });
            //Z型：
            _cells.Add(Type.Z, new Vector2Int[]
            {
                new Vector2Int(-1,1),
                new Vector2Int(0,1),
                new Vector2Int(0,0),//軸點
                new Vector2Int(1,0)
            });
            //L型：
            _cells.Add(Type.L, new Vector2Int[]
            {
                new Vector2Int(0,2),
                new Vector2Int(0,1),
                new Vector2Int(0,0),//軸點
                new Vector2Int(1,0)
            });
            //J型：
            _cells.Add(Type.J, new Vector2Int[]
            {
                new Vector2Int(0,2),
                new Vector2Int(0,1),
                new Vector2Int(0,0),//軸點
                new Vector2Int(-1,0)
            });
        }
        /// <summary>
        /// [工具]計算方塊組對應CellPos
        /// </summary>
        /// <param name="data">方塊組資料</param>
        /// <returns>Cells座標陣列</returns>
        public static Vector2Int[] CalCells(BrickData data)
        {
            Vector2Int[] calCells = new Vector2Int[4];
            for (int i = 0; i < calCells.Length; i++)
            {
                calCells[i] = cells[data.type][i] + data.pos;
            }
            return calCells;
        }
        /// <summary>
        /// [工具]計算方塊組對應偏移量的CellPos
        /// </summary>
        /// <param name="data">方塊組資料</param>
        /// <param name="offset">偏移量</param>
        /// <returns>Cells座標陣列</returns>
        public static Vector2Int[] CalCells(BrickData data, Vector2Int offset)
        {
            Vector2Int[] calCells = new Vector2Int[4];
            for (int i = 0; i < calCells.Length; i++)
            {
                calCells[i] = cells[data.type][i] + data.pos + offset;
            }
            return calCells;
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
