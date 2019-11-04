using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager LM;
    public GameObject winPannel;
    public GameObject gameOverPanel;
    public GameObject startPannel;

    public bool isStarted;

    public int levelNumber;


    // Start is called before the first frame update
    void Start()
    {
        LevelManager.LM = this;
    }

   public void StartGame()
    {
        isStarted = true;
        startPannel.SetActive(false);
    }
    public void TryAgain()
    {
        SceneManager.LoadScene(levelNumber);
    }
    public void NextLevel()
    {
        levelNumber += 1;
        SceneManager.LoadScene(levelNumber);
    }
}
