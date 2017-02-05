using UnityEngine;
using UnityEngine.SceneManagement;

public class GameExit : MonoBehaviour {
	
	void Update () 
	{
		if(Input.GetButtonDown("Cancel"))
		{
			Application.Quit();
		}
	}
}
