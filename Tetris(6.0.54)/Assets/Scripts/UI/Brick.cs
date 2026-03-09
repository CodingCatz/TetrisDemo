using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Analytics.IAnalytic;

namespace Puzzle.Tetris
{
    /// <summary>
    /// 磚塊單元物件(顯示介面)
    /// </summary>
    public class Brick : MonoBehaviour
    {
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
        public Color color { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">名稱：座標描述</param>
        public void Initial(string name)
        {
            this.name = name;
            state = State.None;
            color = Type.None.ActiveColor();
        }

        /// <summary>
        /// 切換磚塊狀態
        /// </summary>
        /// <param name="state">要切換的狀態</param>
        public void ChangeState(CellData data)
        {
            state = data.state;
            color = data.color;
            //更新磚塊視覺
            image.color = color;
        }
    }
}
