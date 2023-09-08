using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void OnPlayButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadSceneAsync("Level 1");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnPlayAgainButton()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
}
