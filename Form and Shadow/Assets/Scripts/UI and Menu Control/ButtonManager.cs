using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public bool hasPlayedTutorial;

    public void Start()
    {
        hasPlayedTutorial = false;
    }

    public void LoadLevelByName(string newGameLevel)
    {
        //if (newGameLevel == "Level_1") hasPlayedTutorial = true;
        //if (newGameLevel == "Level_Select" && !hasPlayedTutorial) return;
        SceneManager.LoadScene(newGameLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
