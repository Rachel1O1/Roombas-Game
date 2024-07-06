using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Points : MonoBehaviour
{
    public int highScore;
    public int points;
    public TMP_Text highScoreText;
    public int maxLevel;
    public string sceneName;

    public GameObject gameCanvas;

    public GameObject pauseCanvas;

    public GameObject winCanvas;

    public GameObject loseCanvas;

    public GameObject instructionsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore");
        highScoreText.text = "Score: " + points.ToString();
        maxLevel = PlayerPrefs.GetInt("maxLevel");
        sceneName = SceneManager.GetActiveScene().name;
        gameCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        instructionsCanvas.SetActive(false);
    }

    public void AddPoints()
    {
        points++;
        highScoreText.text = "Score: " + points.ToString();
        highScore++; // add to total high score points.
        PlayerPrefs.SetInt("highScore", highScore); //save new high score.
    }

    public void FailLevel() // don't take action just reload the previous scene.
    {
        SceneManager.LoadScene(0); // 0 will be the main menu screen.
    }

    public void PassLevel()
    {
       if (maxLevel.ToString() == sceneName) // if we are on the highest unlocked level...
        {
            maxLevel++;
            PlayerPrefs.SetInt("maxLevel", maxLevel);
            SceneManager.LoadScene(0);
        }
        else //if you're doing a stage you've already completed.
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Pause()
    {
        gameCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        instructionsCanvas.SetActive(false);
    }

    public void Play()
    {
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        instructionsCanvas.SetActive(false);
    }

    public void WinScreen()
    {
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        winCanvas.SetActive(true);
        loseCanvas.SetActive(false);
        instructionsCanvas.SetActive(false);
    }

    public void LoseScreen()
    {
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(true);
        instructionsCanvas.SetActive(false);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ShowInstructions()
    {
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        instructionsCanvas.SetActive(true);
    }
}

//https://www.youtube.com/watch?v=z2MuOtWYnPI for reference