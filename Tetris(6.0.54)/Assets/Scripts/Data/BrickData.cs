using UnityEngine;

namespace Puzzle.Tetris
{
    /// <summary>
    /// [結構]方塊資料組合
    /// </summary>
    public struct BrickData
    {
        #region 基本屬性
        /// <summary>
        /// 形狀類型
        /// </summary>
        public Type type;
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
        /// 轉向方位的索引號碼
        /// </summary>
        private int _rotaIndex;
        /// <summary>
        /// 轉向方位的索引運算
        /// </summary>
        public int rotaIndex
        {
            get => _rotaIndex;
            set => _rotaIndex = value % 4;//四軸向取餘數
        }
        /// <summary>
        /// 當前操作中的對應座標組
        /// </summary>
        public Vector2Int[] Cells => CalRota();

        private Vector2Int[] CalRota()
        {
            //讀取模板
            Vector2Int[] tmp = TetrisConfig.RotaTmp[type][rotaIndex];
            Vector2Int[] result = new Vector2Int[tmp.Length];
            for (int i = 0; i < tmp.Length; i++)
            {
                result[i] = pos + tmp[i];
            }
            return result;
        }
        #endregion 基本屬性

        #region 初始化
        /// <summary>
        /// 設定初始狀態
        /// </summary>
        /// <param name="x">起始X</param>
        /// <param name="y">起始Y</param>
        /// <param name="type">形狀</param>
        public void SetData(int x, int y, Type type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
            rotaIndex = 0;//初始化旋轉
        }
        #endregion 初始化

        #region 移動旋轉相關功能

        /// <summary>
        /// 移動1個單位
        /// </summary>
        /// <param name="offset">指定方向</param>
        public void Move(Vector2Int direction)
        {
            x += direction.x;
            y += direction.y;
        }

        /// <summary>
        /// 順時針旋轉90度
        /// </summary>
        public void Rota()
        {
            //正方形不旋轉
            if (type == Type.O) return;
            //旋轉索引+1
            rotaIndex++;
        }
        #endregion 移動旋轉相關功能
    }
}