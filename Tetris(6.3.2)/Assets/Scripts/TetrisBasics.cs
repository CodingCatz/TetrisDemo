using UnityEngine;//使用 XXXXXX命名空間

//命名空間(程式資料夾的概念) 第一層名稱(.的)次一層名稱
namespace Puzzle.Tetris
{
    //公開權限 類別 名稱 (:繼承) Unity基礎類別
    public class TetrisBasics : MonoBehaviour
    {
        //物件建立 NEW 相當於記憶體空間的規劃
        GameData data = new GameData(5,10);

        private void Start()
        {
            Debug.Log(data.boardWidth);
            Debug.Log(data.boardHeight);
        }
    }
}
