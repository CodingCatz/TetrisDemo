using Puzzle.Tetris;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject titleCover;
    public GameObject gameOver;
    public TetrisBasics[] players = new TetrisBasics[2];
    public CountdownTimer cdTimer;
    public float gameTime = 300;
    private int playerCount = 0;

    public void SetPlayer(int count)
    {//設定玩家數量
        playerCount = count;
    }

    public void Replay()
    {//場景重載
        SceneManager.LoadScene("GamePlay");
    }

    public void QuitGame()
    {//退出應用
        Application.Quit();
    }

    /// <summary>
    /// 開始遊戲
    /// </summary>
    public void GameStart()
    {
        cdTimer?.StartTimer(gameTime, TimeUp);
        for (int i = 0; i < playerCount; i++)
        {//玩家數量*N：啟動
            players[i].GameStart();
        }
        titleCover.SetActive(false);//標題面板物件：關閉
    }

    /// <summary>
    /// 時間到
    /// </summary>
    void TimeUp()
    {
        for (int i = 0; i < playerCount; i++)
        {//玩家數量*N：死亡
            players[i].GameOver();
        }
        gameOver.SetActive(true);//遊戲結束面板物件：開啟
    }


}
