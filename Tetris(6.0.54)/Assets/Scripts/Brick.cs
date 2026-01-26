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
            Occupied
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
        /// Brick狀態讀取
        /// </summary>
        public State state { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">名稱：座標描述</param>
        public void Initial(string name)
        {
            this.name = name;
            UpdateColor();
        }

        /// <summary>
        /// 切換磚塊狀態
        /// </summary>
        /// <param name="state">要切換的狀態</param>
        public void ChangeState(State state)
        {
            this.state = state;
            UpdateColor();//更新磚塊視覺
        }

        /// <summary>
        /// 刷新Brick的顏色
        /// </summary>
        public void UpdateColor()
        {
            switch (state)
            {
                default:
                    ClearColor();
                    break;
                case State.Exist:
                    ActiveColor();
                    break;
                case State.Occupied:
                    ActiveColor();
                    break;
            }
        }

        /// <summary>
        /// 預設顏色
        /// </summary>
        private void ClearColor()
        {
            image.color = GameData.orgColor;
        }
        /// <summary>
        /// 啟動顏色
        /// </summary>
        private void ActiveColor()
        {
            image.color = GameData.activeColor;
        }
    }
}
