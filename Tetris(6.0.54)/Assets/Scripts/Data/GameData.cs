using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Puzzle.Tetris
{
    public class GameData
    {
        #region 公開資訊接口
        private int BoardWidth => TetrisConfig.BoardWidth;
        private int BoardHeight => TetrisConfig.BoardHeight;

        private int UIWidth => TetrisConfig.UIWidth;
        private int UIHeight => TetrisConfig.UIHeight;
        /// <summary>
        /// 遊戲棋盤二維陣列(複數集合物件)
        /// </summary>
        private CellData[,] Board;
        /// <summary>
        /// 取得遊戲棋盤單一細胞磚塊資料
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>單一細胞磚塊資料</returns>
        public CellData GetBoradCell(int x, int y)
        {
            return Board[x, y];
        }
        /// <summary>
        /// 預覽區二維陣列
        /// </summary>
        private CellData[,] NextUI;
        public CellData GetNextUICell(int x, int y)
        {
            return NextUI[x, y];
        }
        /// <summary>
        /// 預覽區二維陣列
        /// </summary>
        private CellData[,] HoldUI;
        public CellData GetHoldUICell(int x, int y)
        {
            return HoldUI[x, y];
        }
        #endregion 公開資訊接口

        #region 建構式
        /// <summary>
        /// 建構式(初始化class用)
        /// </summary>
        public GameData()
        {
            Board = new CellData[BoardWidth, BoardHeight];
            NextUI = new CellData[UIWidth, UIHeight];
            HoldUI = new CellData[UIWidth, UIHeight];
            ClearAllData();
        }

        /// <summary>
        /// 建構式(可自訂初始值版本)
        /// </summary>
        /// <param name="width">寬</param>
        /// <param name="height">高</param>
        public GameData(int width, int height)
        {
            Board = new CellData[width, height];
            NextUI = new CellData[UIWidth, UIHeight];
            HoldUI = new CellData[UIWidth, UIHeight];
            ClearAllData();
        }
        /// <summary>
        /// 刷新界面前的資料清理
        /// </summary>
        private void ClearAllData()
        {
            //清除面板UI
            for (int x = 0; x < BoardWidth; x++) 
                for (int y = 0; y < BoardHeight; y++)
                    Board[x, y].Clear();
            //清除預覽UI
            for (int x = 0; x < UIWidth; x++)
                for (int y = 0; y < UIHeight; y++)
                    NextUI[x, y].Clear();
            //清除保留UI
            for (int x = 0; x < UIWidth; x++)
                for (int y = 0; y < UIHeight; y++)
                    HoldUI[x, y].Clear();
        }
        #endregion 建構式

        #region Brick狀態驗證&碰撞相關
        /// <summary>
        /// 隨機取得一個方塊形狀
        /// </summary>
        /// <returns>方塊形狀</returns>
        public Type RandomType()
        {
            return (Type)Random.Range(0, 7);
        }

        /// <summary>
        /// 當前磚塊組的垂直著陸點
        /// </summary>
        /// <param name="orgBrick">當前磚塊組原始資料</param>
        /// <returns>著陸點的虛擬磚塊組</returns>
        public BrickData GetBrickShadow(BrickData brickData)
        {
            BrickData tmp = brickData;//影Brick
            do
            {//先模擬位移一次
                tmp.Move(Vector2Int.down);
            }//再判斷是否繼續循環 
            while (IsValid(tmp));
            tmp.Move(Vector2Int.up);//回彈處理
            return tmp;
        }

        /// <summary>
        /// 檢查方塊是否處於合法位置
        /// </summary>
        /// <returns>是否處於合法位置</returns>
        public bool IsValid(BrickData brickData)
        {
            foreach (var cell in brickData.Cells)
            {
                //出界(左、下、右邊)超出
                if (cell.x < 0 || cell.y < 0 || cell.x >= BoardWidth)
                    return false;
                //重疊(上邊以內)
                if (cell.y < BoardHeight)
                {
                    if (GetBrickState(cell) == State.Occupied) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 磚塊組撞擊鎖定
        /// </summary>
        /// <param name="brickData"></param>
        public void ImpactLock(BrickData brickData)
        {
            foreach (Vector2Int cell in brickData.Cells)
            {
                if (cell.y >= 0 && cell.y < BoardHeight && cell.x >= 0 && cell.x < BoardWidth)
                {
                    Board[cell.x, cell.y].SetData(State.Occupied, brickData.type);
                }
            }
        }

        /// <summary>
        /// 取得特定位置磚塊的狀態
        /// </summary>
        /// <param name="pos">定位</param>
        /// <returns>磚塊的狀態</returns>
        public State GetBrickState(Vector2Int pos)
        {
            return Board[pos.x, pos.y].state;
        }

        #endregion Brick狀態操作相關

        #region 消除邏輯
        /// <summary>
        /// 確認磚塊連線消除
        /// </summary>
        public void CheckClearLines(Action<int> ClearRows)
        {
            int count = 0;
            for (int y = 0; y < BoardHeight;)
            {
                if (IsLineFull(y))
                {//該橫排是否填滿
                    count++;
                    //清除指定橫排
                    ClearLine(y);
                    //整體磚塊資料下降
                    ShiftRowsDown(y);
                }
                else y++;
            }
            ClearRows(count);
        }
        /// <summary>
        /// 檢查特定Y橫排是否滿線
        /// </summary>
        /// <param name="y">Y橫排值</param>
        /// <returns>是否滿線</returns>
        private bool IsLineFull(int y)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                if (Board[x, y].state != State.Occupied) return false;
            }
            return true;
        }
        /// <summary>
        /// 清除特定Y橫排
        /// </summary>
        /// <param name="y">Y橫排值</param>
        private void ClearLine(int y)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                Board[x, y].Clear();
            }
        }
        /// <summary>
        /// 從起始排Y以上磚塊資料下移
        /// </summary>
        /// <param name="startY">起始排Y值</param>
        private void ShiftRowsDown(int startY)
        {
            for (int y = startY; y < BoardHeight - 1; y++)
            {//頂排不用做移動所以 -1
                for (int x = 0; x < BoardWidth; x++)
                {//將上一排狀態轉移至這排
                    Board[x, y] = Board[x, y + 1];
                }
            }
            ClearLine(BoardHeight - 1);//最後一排清除
        }
        #endregion 消除邏輯

        #region 戰鬥機制
        /// <summary>
        /// 受到攻擊(對手消除2行以上時)
        /// </summary>
        /// <param name="lines">逞罰行數</param>
        public void OnAttack(int lines)
        {
            if (lines <= 0) return;
            //所有磚塊上移 lines 格
            for (int y = BoardHeight - 1; y >= lines; y--)
                for (int x = 0; x < BoardWidth; x++)
                    Board[x, y] = Board[x, y - lines];
            //生成垃圾
            for (int y = 0; y < lines; y++)
            {
                int holeX = Random.Range(0, BoardWidth);//隨機決定缺口
                for (int x = 0; x < BoardWidth; x++)
                {
                    if (x == holeX) Board[x, y].Clear();
                    else Board[x, y].SetData(State.Occupied, RandomType());
                }
            }
        }
        #endregion 戰鬥機制
    }
}
