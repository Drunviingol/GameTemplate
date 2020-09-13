using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

/*
 * Author: Dyllan Thomas
 * Date Created: September 9, 2020
 * Date Modified: September 9, 2020
 * Description: Game Manager that manages basic game resources
 * */

public class GameManager : MonoBehaviour
{

    /***** Public Variables *****/
    #region GM Singleton
    public static GameManager gm;
    private void Awake()
    {
        if (gm != null)
        {
            Debug.LogWarning("More than one instance of game manager found");
        }
        else
        {
            gm = this;
            DontDestroyOnLoad(gm);
        }
    }//end Awake()
    #endregion

    public static int playLives; //number of lives for player
    public static int score; //score value

    [Header("GENERAL SETTINGS")]
    public string gameTitle = "Untitled Game";
    public string gameCredits = "Made by Dyllan Thomas"; //creators
    public string copyrightDate = "Copyright " + thisDay;

    [Space(10)]

    public TMP_Text titleDisplay; //text display for title
    public TMP_Text creditsDisplay; //text display for credits
    public TMP_Text copyrightDisplay; //text display for cpoyright


    [Header("GAME SETTINGS")]
    public GameObject player; //player object
    public int defaultScore = 0; //default score
    public int defaultLives = 3; //default lives
    public bool canBeatLevel = false; //
    public int beatLevelScore;
    public bool timedLevel = false;
    public float startTime = 5.0f;

    public AudioSource backgroundMusicAudio; //game Background Music
    public AudioClip gameOverSFX; //game over sfx
    public AudioClip beatLevelSFX;

    [HideInInspector]public enum gameStates { Playing, Death, GameOver, BeatLevel};
    [HideInInspector]public gameStates gameState = gameStates.Playing;
    [HideInInspector]public bool gameIsOver = false;
    [HideInInspector]public bool playerIsDead = false;

    [Header("MENU SETTINGS")]
    public GameObject MenuCanvas;
    public GameObject HUDCanvas;
    public GameObject EndScreenCanvas;
    public GameObject FooterCanvas;

    public string scoreTitle = "Score: ";
    public TMP_Text scoreTitleDisplay;
    public TMP_Text scoreValueDisplay;

    public string livesTitle = "Lives: ";
    public TMP_Text livesTitleDisplay;
    public TMP_Text livesValueDisplay;

    public string timerTitle = "Timer: ";
    public TMP_Text timerTitleDisplay;
    public TMP_Text timerValueDisplay;

    public string gameOver = "Game Over";
    public TMP_Text gameOverDisplay;

    public string loseMessage = "You Lose";
    public string winMessage = "You Win";
    public TMP_Text gameMessage;

    public string playAgainLevelToLoad;
    //get current level and set it as reload level
    public string nextLevelToLoad; //next level to load


    /**** Private Variables ****/
    private float currentTime;
    private bool gameStarted = false; //tests if game has started
    private static bool replay = false;
    private static string thisDay = System.DateTime.Now.ToString("yyyy"); //todays date as a string



    // Start is called before the first frame update
    void Start()
    {
        MenuCanvas.SetActive(true);
        FooterCanvas.SetActive(false);
        EndScreenCanvas.SetActive(false);
        HUDCanvas.SetActive(false);
        Scene currentLevel = SceneManager.GetActiveScene();
        playAgainLevelToLoad = currentLevel.name;

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        playerIsDead = false;
        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentLevel);
        PlayGame();
    }

    public void StartNextLevel()
    {
        //backgroundMusicAudio.SetActive(true);
        playLives = defaultLives;
        Scene currentLevel = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentLevel);
        nextLevelToLoad = "SceneTemplate";
        PlayGame();


    }

    public void Reset()
    {
        playLives = defaultLives;
        score = defaultScore;
    }

    public void HideMenu()
    {
        EndScreenCanvas.SetActive(false);
        MenuCanvas.SetActive(false);
        FooterCanvas.SetActive(false);
    }

    public void MainMenu()
    {
        defaultScore = 0;
        defaultLives = 3;
        titleDisplay.text = gameTitle;
        creditsDisplay.text = gameCredits;

        MenuCanvas.SetActive(true);
        FooterCanvas.SetActive(true);
    }

    public void PlayGame()
    {
        HideMenu();
        HUDCanvas.SetActive(true);

        if (timedLevel)
        {
            currentTime = startTime;
            timerTitleDisplay.text = timerTitle;
        }
        
        if (beatLevelScore >= 0)
        {
            scoreValueDisplay.text = score.ToString();
            scoreTitleDisplay.text = scoreTitle;
        }

        if (defaultLives >= 0)
        {
            livesValueDisplay.text = defaultLives.ToString();
            livesTitleDisplay.text = livesTitle;
        }

        gameStarted = true;
        gameState = gameStates.Playing;

        playerIsDead = false;
        SceneManager.LoadScene(playAgainLevelToLoad, LoadSceneMode.Additive);

        Scene currentLevel = SceneManager.GetActiveScene();
        playAgainLevelToLoad = currentLevel.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
            QuitGame();

        if (Input.GetKey("end"))
            gameState = gameStates.GameOver;

        switch (gameState)
        {
            case gameStates.Playing:
                if (playerIsDead)
                {
                    if(playLives > 0)
                    {
                        playLives -= 1;
                        RestartLevel();
                    }
                    else
                    {
                        gameState = gameStates.Death;
                    }
                }
                break;

            case gameStates.Death:
                backgroundMusicAudio.volume -= 0.01f;
                if (backgroundMusicAudio.volume <= 0.0f)
                {
                    AudioSource.PlayClipAtPoint(gameOverSFX, gameObject.transform.position);
                    gameMessage.text = loseMessage;
                    gameState = gameStates.GameOver;
                }
                break;

            case gameStates.GameOver:
                if (player)
                    player.SetActive(false);

                if (HUDCanvas)
                    HUDCanvas.SetActive(false);

                if(EndScreenCanvas)
                {
                    EndScreenCanvas.SetActive(true);
                    gameOverDisplay.text = gameOver;
                }

                if (FooterCanvas)
                    FooterCanvas.SetActive(true);

                break;

            case gameStates.BeatLevel:
                backgroundMusicAudio.volume -= 0.01f;
                if (backgroundMusicAudio.volume <= 0.0f)
                {
                    AudioSource.PlayClipAtPoint(beatLevelSFX, gameObject.transform.position);
                }

                if (nextLevelToLoad != null)
                    StartNextLevel();

                else
                {
                    gameMessage.text = winMessage;
                    gameState = gameStates.GameOver;
                }

                break;
        }
        
    }
}
