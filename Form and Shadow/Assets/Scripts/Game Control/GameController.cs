using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	public static bool resetting;
	public static int score;

    private static GameObject w_Tooltip;
    private static GameObject a_Tooltip;
    private static GameObject s_Tooltip;
    private static GameObject d_Tooltip;
    private static GameObject e_Tooltip;
	private static GameObject f_Tooltip;
    private static GameObject space_Tooltip;
    private static GameObject shift_Tooltip;

    private GameObject scoreText;
    private GameObject shadowMeldResourceObject;
    
	private GameObject player;
    private GameObject playerShadow;

	void Start () 
	{
		player = GameObject.Find("Player_Character");
		playerShadow = GameObject.Find("Player_Shadow");
        scoreText = GameObject.Find("Score_Text");
        w_Tooltip = GameObject.Find("W_Tooltip");
        a_Tooltip = GameObject.Find("A_Tooltip");
        s_Tooltip = GameObject.Find("S_Tooltip");
        d_Tooltip = GameObject.Find("D_Tooltip");
        e_Tooltip = GameObject.Find("E_Tooltip");
		f_Tooltip = GameObject.Find("F_Tooltip");
        e_Tooltip.SetActive(false);
		f_Tooltip.SetActive(false);
        space_Tooltip = GameObject.Find("Space_Tooltip");
        shift_Tooltip = GameObject.Find("Shift_Tooltip");
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

    public static void Toggle3DMovementTooltips(bool on)
    {
        w_Tooltip.SetActive(on);
        a_Tooltip.SetActive(on);
        s_Tooltip.SetActive(on);
        d_Tooltip.SetActive(on);
        space_Tooltip.SetActive(on);
        shift_Tooltip.SetActive(on);
    }

    public static void ToggleGrabbingTooltips(bool on)
    {
        w_Tooltip.SetActive(on);
        s_Tooltip.SetActive(on);
        a_Tooltip.SetActive(!on);
        d_Tooltip.SetActive(!on);
        e_Tooltip.SetActive(!on);
		f_Tooltip.SetActive(!on);
        space_Tooltip.SetActive(!on);
        shift_Tooltip.SetActive(!on);
    }

    public static void Toggle2DMovementTooltips(bool on)
    {
        w_Tooltip.SetActive(!on);
        s_Tooltip.SetActive(!on);
        a_Tooltip.SetActive(on);
        d_Tooltip.SetActive(on);
        space_Tooltip.SetActive(on);
    }

    public static void ToggleShadowShiftOutTooltips(bool on)
    {
        w_Tooltip.SetActive(on);
        s_Tooltip.SetActive(on);
        a_Tooltip.SetActive(!on);
        d_Tooltip.SetActive(!on);
        space_Tooltip.SetActive(!on);
    }

    public static void ToggleInteractTooltip(bool on)
    {
        e_Tooltip.SetActive(on);
    }

	public static void ToggleShadowMeldTooltip(bool on)
	{
		f_Tooltip.SetActive(on);
	}

    public static void ToggleShadowShiftInTooltip(bool on)
    {
        shift_Tooltip.SetActive(on);
    }

    public static void ScoreIncrement(int amount)
	{
		score += amount;
	}
}
