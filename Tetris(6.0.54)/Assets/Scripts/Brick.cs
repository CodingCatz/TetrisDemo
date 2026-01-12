using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Tetris
{
    /// <summary>
    /// 磚塊單元物件，包含資料&介面
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

        public Color orgColor;
        public Color activeColor;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">名稱：座標描述</param>
        public void Initial(string name)
        {
            this.name = name;
            ClearColor();
        }
        /// <summary>
        /// 預設顏色
        /// </summary>
        public void ClearColor()
        {
            image.color = orgColor;
        }
        /// <summary>
        /// 啟動顏色
        /// </summary>
        public void ActiveColor()
        {
            image.color = activeColor;
        }
    }
}
