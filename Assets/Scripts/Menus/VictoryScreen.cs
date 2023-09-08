using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _victoryText;
    [SerializeField] private int _score;

    void Awake()
    {
        _victoryText = GetComponentInChildren<TMP_Text>();
        if (PlayerPrefs.HasKey("Score"))
        {
            _score = PlayerPrefs.GetInt("Score");
        }
        else
        {
            _score = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _victoryText.text = "Victory!\nYour score was:\n" + _score;
        _score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayAgainButton()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
}
