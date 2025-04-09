using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Button easyButton;
    public Button mediumButton;
    public Button impossibleButton;
    public TMP_Dropdown playerSignDropdown;

    void Start()
    {
        easyButton.onClick.AddListener(() => StartGame("Easy"));
        mediumButton.onClick.AddListener(() => StartGame("Medium"));
        impossibleButton.onClick.AddListener(() => StartGame("Impossible"));
    }

    void StartGame(string difficulty)
{
    string playerSign = playerSignDropdown.options[playerSignDropdown.value].text.Substring(12, 1); 

    PlayerPrefs.SetString("PlayerSign", playerSign); 
    PlayerPrefs.SetString("Difficulty", difficulty);

    SceneManager.LoadScene("Tic_tac_toe");
}

}
