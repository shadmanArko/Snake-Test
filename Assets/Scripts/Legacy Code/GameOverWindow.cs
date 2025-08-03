﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour {

    private static GameOverWindow instance;

    private void Awake() {
        instance = this;

        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc = () => { 
            Loader.Load(Loader.Scene.GameScene);
        };

        Hide();
    }

    private void Show(bool isNewHighscore) {
        gameObject.SetActive(true);

        transform.Find("newHighscoreText").gameObject.SetActive(isNewHighscore);

        transform.Find("scoreText").GetComponent<Text>().text = Score.GetScore().ToString();
        transform.Find("highscoreText").GetComponent<Text>().text = "HIGHSCORE " + Score.GetHighscore();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public static void ShowStatic(bool isNewHighscore) {
        instance.Show(isNewHighscore);
    }
}
