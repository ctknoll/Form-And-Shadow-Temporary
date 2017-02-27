using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	public static bool resetting;
	public static int score;
    public static GameObject interactText;
    private GameObject scoreText;
    private GameObject shadowMeldResourceObject;
    
	private GameObject player;
    private GameObject playerShadow;

	void Start () 
	{
		player = GameObject.Find("Player_Character");
		playerShadow = GameObject.Find("Player_Shadow");
        interactText = GameObject.Find("Interact_Text");
        scoreText = GameObject.Find("Score_Text");
        shadowMeldResourceObject = GameObject.Find("Shadowmeld_Resource");
	}

    void Update()
    {
        scoreText.GetComponent<Text>().text = "Score: " + score;
        ShadowmeldUIControl();
    }

    void ShadowmeldUIControl()
    {
        shadowMeldResourceObject.SetActive(player.GetComponent<PlayerMovement>().shadowMeldAvailable);
        shadowMeldResourceObject.GetComponent<Image>().color = Color.magenta;
        shadowMeldResourceObject.GetComponent<Image>().fillAmount = player.GetComponent<PlayerMovement>().shadowMeldResource / 100;
    }

    public IEnumerator ResetLevel()
	{
		resetting = true;

		// Reset the player's position
		if(!PlayerMovement.in3DSpace)
		{
			PlayerMovement.in3DSpace = true;
			playerShadow.GetComponent<CharacterController>().enabled = false;
			player.GetComponent<CharacterController>().enabled = true;
			player.GetComponent<PlayerMovement>().controller = player.GetComponent<CharacterController>();
		}
		player.transform.position = PlayerMovement.playerStartPosition;

        //player.GetComponent<PlayerMovement>().ExitShadowMeld();
        player.GetComponent<PlayerMovement>().shadowMeldResource = 100;
        player.GetComponent<PlayerMovement>().shadowMeldVFX.SetActive(false);
        player.layer = LayerMask.NameToLayer("Form");
        yield return new WaitForSeconds(1.0f);
        resetting = false;
        PlayerMovement.shadowMelded = false;
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
