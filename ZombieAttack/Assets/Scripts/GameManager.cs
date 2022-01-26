using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Player ps = null;
    public static int killCount = 0;
    private static GameManager instance = null;
    public static GameManager Instance { get { return instance; } }
    private TMP_Text healthTxt;
    private TMP_Text killCountTxt;

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(instance);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("Player").GetComponent<Player>();
        healthTxt = GameObject.Find("HealthTxt").GetComponent<TMP_Text>();
        killCountTxt = GameObject.Find("KillCountTxt").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level1":
            case "Level2":
            case "Level3":
                if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
                break;
        }

        if (healthTxt) healthTxt.text = "Health: " + ps.Health;
        if (killCountTxt) killCountTxt.text = "Kill Count: " + killCount;
    }

    public void PauseGame()
    {

    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("GameLost");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().name == "Level1") SceneManager.LoadScene("Level2");
        else if (SceneManager.GetActiveScene().name == "Level2") SceneManager.LoadScene("Level3");
        else if (SceneManager.GetActiveScene().name == "Level3")
        {
            killCount = 0;
            ps = null;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("GameWon");
        }
    }

    public void GoToMainMenu()
    {
        killCount = 0;
        SceneManager.LoadScene("MainMenu");
    }

    public void StartLevelOne()
    {
        SceneManager.LoadScene("Level1");
    }

    public void StartLevelTwo()
    {
        SceneManager.LoadScene("Level2");
    }

    public void StartLevelThree()
    {
        SceneManager.LoadScene("Level3");
    }

    public void GoToHowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
