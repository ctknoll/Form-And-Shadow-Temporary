using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	public static bool resetting;
	public static int score;
    public static GameObject interactText;
    public GameObject scoreText;

    public static bool playerShadowMelded;

	private GameObject player;
	private GameObject playerShadow;

	void Start () 
	{
		player = GameObject.Find("Player_Character");
		playerShadow = GameObject.Find("Player_Shadow");
        interactText = GameObject.Find("Interact_Text");
	}

    void Update()
    {
        scoreText.GetComponent<Text>().text = "Score: " + score;
    }

    public IEnumerator ResetLevel()
	{
		resetting = true;
		yield return new WaitForSeconds(0.01f);

		// Reset the player's position
		if(!PlayerMovement.in3DSpace)
		{
			PlayerMovement.in3DSpace = true;
			playerShadow.GetComponent<CharacterController>().enabled = false;
			player.GetComponent<CharacterController>().enabled = true;
			player.GetComponent<PlayerMovement>().controller = player.GetComponent<CharacterController>();
		}
		player.transform.position = PlayerMovement.playerStartPosition;
		resetting = false;

	}

    public static void SetInteractText(string intText)
    {
        interactText.GetComponent<Text>().text = intText;
    }

    public static void ResetInteractText()
    {
        interactText.GetComponent<Text>().text = "";
    }

	public static void ScoreIncrement(int amount)
	{
		score += amount;
	}
}
