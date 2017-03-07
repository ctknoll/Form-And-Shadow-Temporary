using UnityEngine;
using UnityEngine.SceneManagement;

/*
    Written by: SpeedTutor on Youtube, see https://www.youtube.com/watch?v=FrJogRBSzFo&t=698s
                Partially tweaked and edited for game by Daniel Colina
    --ButtonManager--
    Handles logic of buttons in various menu screens, not actually instanced.
*/
public class ButtonManager : MonoBehaviour
{
    public bool hasPlayedTutorial;

    public void Start()
    {
        hasPlayedTutorial = false;
    }

    public void LoadLevelByName(string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
