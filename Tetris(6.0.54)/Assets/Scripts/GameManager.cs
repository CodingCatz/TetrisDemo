using Puzzle.Tetris;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TetrisBasics[] players = new TetrisBasics[2];
    public CountdownTimer cdTimer;
    public float gameTime = 300;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cdTimer?.StartTimer(gameTime, TimeUp);
    }

    void TimeUp()
    {

    }
}
