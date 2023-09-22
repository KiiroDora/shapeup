using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static float coins;
    public static GameObject[] enemies;
    public static GameObject winscr;
    public static GameObject pausscr;
    public GameObject winscr_ref; //non-static GameObjects to attach to the GameController script
    public GameObject pausscr_ref;
    bool escChecker = true;
    void Start()
    {
        Time.timeScale = 1; //resume time when level starts 
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); //find all enemies in scene
        winscr = winscr_ref; //non-static GameObjects for the win and pause screens are assigned to the static variables to use in static functions
        pausscr = pausscr_ref;
    }

    void Update()
    {
        if (coins < 0) { coins = 0; } //prevent coins from going to negative

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escChecker)
            {
                Time.timeScale = 0;
                ScreenActive(0, true);
            }

            else
            {
                ScreenActive(0, false);
                Time.timeScale = 1;
            }

            escChecker = !escChecker;
        }
    }

    public static void ScreenActive(int scr, bool active) //function to set a screen active or inactive
    {
        CameraFollow.ResetCamera(); //call static function from camera script to reset zoom level

        switch (scr)
        {
            case 0: //set pause screen active status 
                pausscr.SetActive(active);
                break;
            case 1: //set win screen active status
                winscr.SetActive(active);
                break;
        }
    }

    public static void EnemyRespawn() //sets all enemies to active
    {
        foreach (GameObject enemy in enemies) { //find all GameObjects with Enemy tag
            enemy.SetActive(true); //set each instance of an Enemy to active
        }
    }

    public static void RestartLevel()
    {
        coins = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void GoToLevel(string levelName)
    {
        coins = 0;
        SceneManager.LoadScene(levelName);
    }

    public static void Quit()
    {
        Application.Quit();
    }
    
}
