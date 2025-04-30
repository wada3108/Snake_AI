using System;
using UnityEngine;

public static class DataManagerStatic
{
    static int score = 0;
    static int best = 0;
    static int speed = 1;
    static int playAreaHeight = 1;
    static int playAreaWidth = 1;

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

    public static void UpdateBest()
    {
        best = Math.Max(best, score);
    }

    public static int GetBest()
    {
        return best;
    }

    public static void SetPlayAreaHeight(int value)
    {
        playAreaHeight = value;
    }

    public static int GetPlayAreaHeight()
    {
        switch (playAreaHeight)
        {
            case 0: return 7;
            case 1: return 11;
            case 2: return 15;
            default: return -1;
        }
    }

    public static int GetPlayAreaHeightValue()
    {
        return playAreaHeight;
    }

    public static void SetPlayAreaWidth(int value)
    {
        playAreaWidth = value;
    }

    public static int GetPlayAreaWidth()
    {
        switch (playAreaWidth)
        {
            case 0: return 7;
            case 1: return 11;
            case 2: return 15;
            default: return -1;
        }
    }

    public static int GetPlayAreaWidthValue()
    {
        return playAreaWidth;
    }

    public static void SetSpeed(int value)
    {
        speed = value;
    }

    public static float GetSpeed()
    {
        return (float) (speed + 1);
    }

    public static int GetSpeedValue()
    {
        return speed;
    }

}
