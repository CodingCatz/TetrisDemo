using UnityEngine;//使用 XXXXXX命名空間

//命名空間(程式資料夾的概念) 第一層名稱(.的)次一層名稱
namespace Puzzle.Tetris
{
    //公開權限 類別 名稱 (:繼承) Unity基礎類別
    public class TetrisBasics : MonoBehaviour
    {
        //數值宣告(測試用)
        public int numberInt;//整數
        public float numberFloat;//小數

        //啟動時運行一次(初始化)
        private void Start()
        {
            Debug.Log(numberInt);
        }

        //每一幀(FPS)運行一次，用來更新
        private void Update()
        {
            Debug.Log(numberInt);
        }
    }
}
