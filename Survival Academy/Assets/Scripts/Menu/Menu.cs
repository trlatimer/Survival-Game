using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Button continueButton;

    private void Start()
    {
        continueButton.interactable = PlayerPrefs.HasKey("Save");
    }

    public void OnContinueButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnNewGameButton()
    {0
        PlayerPrefs.DeleteKey("Save");
        SceneManager.LoadScene("Game");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
