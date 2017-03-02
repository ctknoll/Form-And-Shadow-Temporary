using UnityEngine;
using UnityEngine.SceneManagement;

/*
-- Game Exit --
A simple script. If an object with Game Exit exists,
the assigned Cancel key (ESC) will crash the game.

*/


public class GameExit : MonoBehaviour 
{
	
	void Update () 
	{
		if(Input.GetButtonDown("Cancel"))
		{
			Application.Quit();
		}
	}
}
