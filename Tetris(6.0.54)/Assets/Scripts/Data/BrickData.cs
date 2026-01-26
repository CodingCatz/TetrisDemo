using UnityEngine;

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

        #region 初始化
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
        #endregion 初始化

        #region 移動旋轉相關功能
        /// <summary>
        /// 產生碰撞鎖定
        /// </summary>
        public void Lock()
        {
            isAlive = false;
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
                    if (GameData.GetBrickState(cell) == Brick.State.Occupied) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 移動1個單位
        /// </summary>
        /// <param name="offset">指定方向</param>
        public void Move(Vector2Int direction)
        {
            x += direction.x;
            y += direction.y;
            for (int i = 0; i < cells.Length; i++)
            {//移動
                cells[i] += direction;
            }
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
        #endregion 移動旋轉相關功能

        #region 視覺更新相關功能
        /// <summary>
        /// 清除磚塊組合狀態
        /// </summary>
        /// <param name="cells">方塊組座標陣列</param>
        public void ClearBrickState()
        {
            foreach (Vector2Int cell in cells)
            {//continue；略過超出範圍的cell
                if (cell.y >= H) continue;
                GameData.SetBrickStateToNone(cell);
            }
        }

        /// <summary>
        /// 更新磚塊組合狀態
        /// </summary>
        /// <param name="cells">方塊組座標陣列</param>
        public void UpdateBrickState()
        {
            foreach (Vector2Int cell in cells)
            {//continue；略過超出範圍的cell
                if (cell.y >= H) continue;
                if (isAlive)
                {
                    GameData.SetBrickStateToExist(cell);
                }
                else
                {
                    GameData.SetBrickStateToOccupied(cell);
                }
            }
        }
        #endregion 視覺更新相關功能
    }
}