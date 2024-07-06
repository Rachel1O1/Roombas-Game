using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public LevelScriptableObject[] levelScriptableObjects;
    public GameObject[] levelPanelsGO;
    public LevelPanel[] levelPanels;
    public Button[] myLevelButtons;

    public GameObject defaultScreen;
    public GameObject instructionsScreen;

    public int maxLevel;
    public int highScore;

    public TMP_Text maxLevelText;
    public TMP_Text highScoreText;
    // Start is called before the first frame update

    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        if (PlayerPrefs.HasKey("maxLevel"))
            maxLevel = PlayerPrefs.GetInt("maxLevel");
        else
        {
            maxLevel = 1;
            PlayerPrefs.SetInt("maxLevel", maxLevel);
        }
        highScore = PlayerPrefs.GetInt("highScore");
        maxLevelText.text = "Max Level: " + maxLevel.ToString();
        highScoreText.text = "Total Score: " + highScore.ToString();

        defaultScreen.SetActive(true);
        instructionsScreen.SetActive(false);

        LoadPanels(); 
    }

    public void ShowInstructions()
    {
        defaultScreen.SetActive(false);
        instructionsScreen.SetActive(true);
    }

    public void ShowDefaultScreen()
    {
        defaultScreen.SetActive(true);
        instructionsScreen.SetActive(false);
    }

    public void LoadPanels() //loads the scriptable object information into the project. Done on scene load.
    {
        for (int i = 0; i < levelScriptableObjects.Length; i++) 
        {
            levelPanels[i].levelText.text = levelScriptableObjects[i].level.ToString();
            levelPanels[i].titleText.text = levelScriptableObjects[i].title;

            if (maxLevel >= levelScriptableObjects[i].level) //I am up to this level.
            {
                myLevelButtons[i].interactable = true;
            } else {
                myLevelButtons[i].interactable = false;
            }

        }
    }

    public void SelectLevel(int LevelNo) // Loads the scene when the button is clicked, attached to the onclick function of the buttons.
    {
        SceneManager.LoadScene(LevelNo);
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();

        Setup();
    }
}

//https://www.youtube.com/watch?v=z2MuOtWYnPI for reference