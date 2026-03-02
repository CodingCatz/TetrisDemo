using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Puzzle.Tetris
{
    public class GameData
    {
        #region 公開資訊接口

        /// <summary>
        /// 遊戲棋盤二維陣列(複數集合物件)
        /// </summary>
        public Brick[,] Board { get; private set; }
        
        /// <summary>
        /// 預覽區二維陣列
        /// </summary>
        public Brick[,] NextUI { get; private set; }
        #endregion 公開資訊接口

        #region 建構式
        /// <summary>
        /// 建構式(初始化class用)
        /// </summary>
        public GameData()
        {
            Board = new Brick[TetrisConfig.BoardWidth, TetrisConfig.BoardHeight];
            NextUI = new Brick[TetrisConfig.NextWidth, TetrisConfig.NextHeight];
        }

        /// <summary>
        /// 建構式(可自訂初始值版本)
        /// </summary>
        /// <param name="width">寬</param>
        /// <param name="height">高</param>
        public GameData(int width, int height)
        {
            Board = new Brick[width, height];
            NextUI = new Brick[TetrisConfig.NextWidth, TetrisConfig.NextHeight];
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

        #region Brick狀態操作相關
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
        public Brick.State GetBrickState(Vector2Int pos)
        {
            return Board[pos.x, pos.y].state;
        }

        /// <summary>
        /// 清除Brick的佔用狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public void SetBrickStateToNone(Vector2Int pos)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.None, TetrisConfig.ActiveColor());
        }
        /// <summary>
        /// 清除Brick的佔用狀態
        /// </summary>
        /// <param name="x">座標X</param>
        /// <param name="y">座標Y</param>
        public void SetBrickStateToNone(int x, int y)
        {
            Board[x, y].ChangeState(Brick.State.None, TetrisConfig.ActiveColor());
        }

        /// <summary>
        /// 設定Brick的暫存狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public void SetBrickStateToExist(Vector2Int pos, Type type)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.Exist, TetrisConfig.ActiveColor(type));
        }

        /// <summary>
        /// 設定Brick的佔用狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public void SetBrickStateToOccupied(Vector2Int pos, Type type)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.Occupied, TetrisConfig.ActiveColor(type));
        }
        public void SetBrickStateToDead(Vector2Int pos, Color color)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.Occupied, color);
        }

        public void SetBrickStateToGhost(Vector2Int pos, Type type)
        {
            Board[pos.x, pos.y].ChangeState(Brick.State.Ghost, Color.black);
        }
        #endregion Brick狀態操作相關

        #region NextUI狀態操作相關
        /// <summary>
        /// 設定NextUI的暫存狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public void SetNextUIToExist(Vector2Int pos, Type type)
        {
            NextUI[pos.x, pos.y].ChangeState(Brick.State.Exist, TetrisConfig.ActiveColor(type));
        }
        /// <summary>
        /// 清除NextUI的暫存狀態
        /// </summary>
        /// <param name="pos">定位</param>
        public void SetNextUIToNone(Vector2Int pos)
        {
            NextUI[pos.x, pos.y].ChangeState(Brick.State.None, TetrisConfig.ActiveColor());
        }
        #endregion NextUI狀態操作相關

        #region 消除邏輯
        /// <summary>
        /// 確認磚塊連線消除
        /// </summary>
        public void CheckClearLines(Action<int> ClearRows)
        {
            int count = 0;
            for (int y = 0; y < TetrisConfig.BoardHeight;)
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
            for (int x = 0; x < TetrisConfig.BoardWidth; x++)
            {
                if (Board[x, y].state != Brick.State.Occupied) return false;
            }
            return true;
        }
        /// <summary>
        /// 清除特定Y橫排
        /// </summary>
        /// <param name="y">Y橫排值</param>
        private void ClearLine(int y)
        {
            for (int x = 0; x < TetrisConfig.BoardWidth; x++)
            {
                SetBrickStateToNone(x, y);
            }
        }
        /// <summary>
        /// 從起始排Y以上磚塊資料下移
        /// </summary>
        /// <param name="startY">起始排Y值</param>
        private void ShiftRowsDown(int startY)
        {
            for (int y = startY; y < TetrisConfig.BoardHeight - 1; y++)
            {//頂排不用做移動所以 -1
                for (int x = 0; x < TetrisConfig.BoardWidth; x++)
                {//將上一排狀態轉移至這排
                    Board[x, y].ChangeState(Board[x, y + 1]);
                }
            }
            ClearLine(TetrisConfig.BoardHeight - 1);//最後一排清除
        }
        #endregion 消除邏輯
    }
}
