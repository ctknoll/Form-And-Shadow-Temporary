using UnityEngine;
using UnityEngine.SceneManagement;

/*
    Written by: Chris Knoll
    --LevelFinish--
    Attached to level finish flags, handles transferring to Game_Over
    scene upon finishing a level

*/
public class LevelFinish : MonoBehaviour 
{
	void OnTriggerEnter (Collider other) 
	{
		if(other.gameObject.tag == "Player")
		{
            Cursor.visible = true;
			SceneManager.LoadScene("Game_Over");
		}
	}
}
