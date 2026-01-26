using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

namespace Puzzle.Tetris
{
    /// <summary>
    /// [結構]方塊資料組合
    /// </summary>
    public struct BrickData
    {
        #region 操作屬性
        /// <summary>
        /// 形狀類型
        /// </summary>
        public GameData.Type type;
        /// <summary>
        /// 錨點X座標
        /// </summary>
        public int x;
        /// <summary>
        /// 錨點Y座標
        /// </summary>
        public int y;
        /// <summary>
        /// 錨點座標
        /// </summary>
        private Vector2Int _pos;
        /// <summary>
        /// 錨點座標公開接口
        /// </summary>
        public Vector2Int pos
        {
            get
            {
                _pos.x = x;
                _pos.y = y;
                return _pos;
            }
        }
        /// <summary>
        /// 當前操作中的對應座標組
        /// </summary>
        public Vector2Int[] cells { get; private set; }
        /// <summary>
        /// [暫存]重新計算的旋轉位置
        /// </summary>
        private Vector2Int newRota;
        /// <summary>
        /// 方塊組是否處於可活動狀態
        /// </summary>
        public bool isAlive { get; private set; }
        #endregion 操作屬性

        private int W => GameData.BoardWidth;
        private int H => GameData.BoardHeight;

        /// <summary>
        /// 設定初始狀態
        /// </summary>
        /// <param name="x">起始X</param>
        /// <param name="y">起始Y</param>
        /// <param name="type">形狀</param>
        public void SetData(int x, int y, GameData.Type type)
        {
            isAlive = true;
            this.x = x;
            this.y = y;
            this.type = type;
            this.cells = GameData.CloneCells(type, pos);
        }
        /// <summary>
        /// 產生碰撞鎖定
        /// </summary>
        public void Lock()
        {
            isAlive = false;
        }
        /// <summary>
        /// 下墜1個單位
        /// </summary>
        public void Fall()
        {
            this.y -= 1;
        }

        /// <summary>
        /// 檢查方塊是否處於合法位置
        /// </summary>
        /// <returns>是否處於合法位置</returns>
        public bool IsValid()
        {
            foreach (var cell in cells)
            {
                //出界(左、下、右邊)超出
                if (cell.x < 0 || cell.y < 0 || cell.x >= W)
                    return false;
                //重疊(上邊以內)
                if (cell.y < H)
                {
                    if(GameData.GetBrickState(cell) == Brick.State.Occupied) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 移動(撞擊)確認
        /// </summary>
        /// <param name="direction">方向</param>
        /// <returns>是否</returns>
        public bool CheckMove(Vector2Int direction)
        {
            foreach (Vector2Int cell in cells)
            {
                if (cell.y >= GameData.BoardHeight) continue;
                //0.左右超界檢查
                if (cell.x < 0 || cell.x >= GameData.BoardWidth)
                {
                    return false;
                }
                //1.觸底檢查(預判) or 2.觸碰堆疊
                if (cell.y < 0 || GameData.Board[cell.x, cell.y].state == Brick.State.Occupied)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 移動1個單位
        /// </summary>
        /// <param name="offset">指定方向</param>
        public void Move(Vector2Int offset)
        {
            this.x += offset.x;
            this.y += offset.y;
        }
        /// <summary>
        /// 順時針旋轉90度
        /// </summary>
        public void Rota()
        {
            //正方形不旋轉
            if (type == GameData.Type.O) return;
            for (int i = 0; i < cells.Length; i++)
            {//旋轉公式 (y,-x)
                newRota.x = cells[i].y;
                newRota.y = -cells[i].x;
                cells[i] = newRota;//從暫存取代原本
            }
        }
    }

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
    }
}

namespace Puzzle.Match3
{
    public class GameData
    {
        //不同類型的遊戲後端資料
    }

}
