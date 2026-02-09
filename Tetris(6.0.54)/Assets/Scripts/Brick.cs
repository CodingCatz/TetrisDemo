using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Tetris
{
    /// <summary>
    /// 磚塊單元物件，包含資料&介面
    /// </summary>
    public class Brick : MonoBehaviour
    {
        #region 定義
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
            Ghost
        }
        #endregion 定義

        #region 基礎元件
        private Image _image;
        private Image image
        {
            get 
            { 
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                }
                return _image; 
            }
        }
        #endregion 基礎元件

        /// <summary>
        /// [唯讀]Brick狀態
        /// </summary>
        public State state { get; private set; }
        /// <summary>
        /// [唯讀]Brick的顏色
        /// </summary>
        public Color color
        {
            get
            {
                return image.color;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">名稱：座標描述</param>
        public void Initial(string name)
        {
            this.name = name;
            ChangeState(State.None, GameData.ActiveColor());
        }

        /// <summary>
        /// 切換磚塊狀態
        /// </summary>
        /// <param name="state">要切換的狀態</param>
        public void ChangeState(State state, Color color)
        {
            this.state = state;
            color.a = state == State.Ghost ? 0.2f : 1f;
            //更新磚塊視覺
            image.color = color;
        }
        /// <summary>
        /// 切換磚塊的狀態(拷貝別的磚塊)
        /// </summary>
        /// <param name="brick">目標磚塊</param>
        public void ChangeState(Brick brick)
        {
            state = brick.state;
            image.color = brick.color;
        }
    }
}
