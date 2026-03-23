using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Puzzle.Tetris
{
    #region ENUM列舉
    /// <summary>
    /// 方塊種類(形狀)的列舉
    /// </summary>
    public enum Type
    {
        I, O, T, S, Z, L, J, None
    }
    /// <summary>
    /// [定義]Brick的基本狀態
    /// </summary>
    public enum State
    {
        /// <summary>
        /// 無磚塊
        /// </summary>
        None,
        /// <summary>
        /// 有磚塊於此
        /// </summary>
        Exist,
        /// <summary>
        /// 磚塊佔據
        /// </summary>
        Occupied,
        /// <summary>
        /// 預判磚塊組鬼影
        /// </summary>
        Ghost,
        /// <summary>
        /// 死亡鎖定
        /// </summary>
        Dead
    }
    #endregion ENUM列舉

    #region STRUCT資料
    /// <summary>
    /// 磚塊細胞資料結構
    /// </summary>
    public struct CellData
    {
        /// <summary>
        /// 磚塊細胞(最小單位)的狀態
        /// </summary>
        public State state;
        /// <summary>
        /// 磚塊細胞(最小單位)的所屬類型
        /// </summary>
        public Type type;

        public Color color => type.ActiveColor(state);
        /// <summary>
        /// 設定磚塊細胞數據
        /// </summary>
        /// <param name="state"></param>
        /// <param name="type"></param>
        public void SetData(State state, Type type)
        {
            this.state = state; 
            this.type = type;
        }
        /// <summary>
        /// 清除磚塊細胞數據
        /// </summary>
        public void Clear()
        {
            state = State.None;
            type = Type.None;
        }
    }
    #endregion STRUCT資料

    public static class TetrisConfig
    {
        #region 常數
        /// <summary>
        /// 棋盤寬
        /// </summary>
        public const int BoardWidth = 10;
        /// <summary>
        /// 棋盤高
        /// </summary>
        public const int BoardHeight = 20;
        /// <summary>
        /// 預覽區寬
        /// </summary>
        public const int UIWidth = 3;
        /// <summary>
        /// 域覽區高
        /// </summary>
        public const int UIHeight = 4;
        #endregion 常數

        /// <summary>
        /// [字典]方塊形狀對應座標集合物件本體
        /// </summary>
        private static Dictionary<Type, List<Vector2Int[]>> _rotaTmp;
        /// <summary>
        /// [字典]方塊形狀對外公開存取接口
        /// </summary>
        public static readonly Dictionary<Type, List<Vector2Int[]>> RotaTmp;

        /// <summary>
        /// [唯讀常數]踢牆機制位移
        /// </summary>
        public static readonly Vector2Int[] WallKickOffests
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
        /// [建構]初始化靜態資料
        /// </summary>
        static TetrisConfig()
        {
            RotaTmp = new Dictionary<Type, List<Vector2Int[]>>();
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
                RotaTmp.Add(type, list);
            }
        }

        #region 擴充功能
        /// <summary>
        /// [擴充功能]BrickCell顏色控制
        /// </summary>
        /// <param name="type">類型</param>
        /// <param name="state">狀態</param>
        /// <returns>對應的顏色</returns>
        public static Color ActiveColor(this Type type, State state = State.None)
        {
            Color color = Color.black;
            if (state == State.Dead)
            {//死亡滅頂
                color.a = 0.7f;
                return color;
            }

            switch (type)
            {
                default: 
                    color = Color.gray; break;
                case Type.I: 
                    color = Color.cyan; break;
                case Type.O: 
                    color = Color.blue; break;
                case Type.T: 
                    color = Color.blue + Color.red; break;
                case Type.S: 
                    color = Color.green; break;
                case Type.Z: 
                    color = Color.red; break;
                case Type.L: 
                    color = Color.yellow; break;
                case Type.J: 
                    color = Color.red + Color.yellow; break;
            }
            color.a = state == State.Ghost ? 0.2f : 1f;
            return color;
        }
        #endregion 擴充功能
    }
}

