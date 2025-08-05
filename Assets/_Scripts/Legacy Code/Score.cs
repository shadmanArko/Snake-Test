﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score {

    public static event EventHandler OnHighscoreChanged;

    private static int score;

    public static void InitializeStatic() {
        OnHighscoreChanged = null;
        score = 0;
    }

    public static int GetScore() {
        return score;
    }

    public static void AddScore() {
        score += 100;
    }

    public static int GetHighscore() {
        return PlayerPrefs.GetInt("highscore", 0);
    }

    public static bool TrySetNewHighscore() {
        return TrySetNewHighscore(score);
    }

    public static bool TrySetNewHighscore(int score) {
        int highscore = GetHighscore();
        if (score > highscore) {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            if (OnHighscoreChanged != null) OnHighscoreChanged(null, EventArgs.Empty);
            return true;
        } else {
            return false;
        }
    }

}
