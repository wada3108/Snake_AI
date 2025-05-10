using System;
using System.Text;
using UnityEngine;

public static class DataManagerStatic
{
    static int score = 0;
    static int playAreaHeight = 15;
    static int playAreaWidth = 15;
    static StringBuilder playAreaState = new StringBuilder(new string('0', (playAreaHeight + 2) * (playAreaWidth + 2)));

    public static void AddScore(int value)
    {
        score += value;
    }

    public static int GetScore()
    {
        return score;
    }

    public static void ResetScore()
    {
        score = 0;
    }

    public static void SetPlayAreaHeight(int value)
    {
        playAreaHeight = value;
    }

    public static int GetPlayAreaHeight()
    {
        return playAreaHeight;
    }

    public static void SetPlayAreaWidth(int value)
    {
        playAreaWidth = value;
    }

    public static int GetPlayAreaWidth()
    {
        return playAreaWidth;
    }

    public static void ChangeState(Vector3 posv, char obj)
    {
        int pos = (int) (posv.x - 0.5f) + playAreaWidth / 2 + 1 + 
                ((int) (posv.y - 0.5f) + playAreaHeight / 2 + 1) * (playAreaWidth + 2);
        if (pos < 0 || (playAreaWidth + 2) * (playAreaHeight + 2) <= pos)
        {
            Console.WriteLine("Invalid pos for ChangeState");
        }
        playAreaState[pos] = obj;
    }

    public static void InitState()
    {
        for (int i = 0; i < playAreaHeight + 2; i++) {
            for (int j = 0; j < playAreaWidth + 2; j++) {
                int pos = i * (playAreaWidth + 2) + j;
                if (i == 0 || i == playAreaHeight + 1) playAreaState[pos] = '0';
                else if (j == 0 || j == playAreaWidth + 1) playAreaState[pos] = '0';
                else playAreaState[pos] = '1';
            }
        }
    }

    public static StringBuilder GetAreaState()
    {
        return playAreaState;
    }

}
