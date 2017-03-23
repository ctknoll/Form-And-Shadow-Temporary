using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


/*

    Written by: Daniel Colina and Chris Knoll
    --GameController--
    A master control script for handling UI, resetting the level, and score.
    Attached to the GameController prefab in each scene.

*/
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

    public static bool e_First_Time_Used;
    public static bool shift_First_Time_Used;
    public static bool f_First_Time_Used;

    private GameObject scoreText;
    private GameObject shadowMeldResourceObject;
    
	private GameObject player;
    private GameObject playerShadow;
    [HideInInspector]
    public static GameController instance;

	void Start () 
	{
		player = GameObject.Find("Player_Character");
		playerShadow = GameObject.Find("Player_Shadow");
        instance = GetComponent<GameController>();
        scoreText = GameObject.Find("Score_Text");
        w_Tooltip = GameObject.Find("W_Tooltip");
        a_Tooltip = GameObject.Find("A_Tooltip");
        s_Tooltip = GameObject.Find("S_Tooltip");
        d_Tooltip = GameObject.Find("D_Tooltip");
        e_Tooltip = GameObject.Find("E_Tooltip");
		f_Tooltip = GameObject.Find("F_Tooltip");
        e_First_Time_Used = false;
        f_First_Time_Used = false;
        shift_First_Time_Used = false;

        e_Tooltip.SetActive(false);
		f_Tooltip.SetActive(false);

        space_Tooltip = GameObject.Find("Space_Tooltip");
        shift_Tooltip = GameObject.Find("Shift_Tooltip");
        shadowMeldResourceObject = GameObject.Find("Shadowmeld_Resource");
	}

    void Update()
    {
        ScoreUIControl();
        ShadowmeldUIControl();

        if(Input.GetButtonDown("Quit"))
        {
            SceneManager.LoadScene("Menu_Title");
        }
    }

    #region UI Control
    void ScoreUIControl()
    {
        scoreText.GetComponent<Text>().text = "Score: " + score;

    }

    void ShadowmeldUIControl()
    {
        shadowMeldResourceObject.SetActive(player.GetComponent<PlayerMovement>().shadowMeldAvailable);
        shadowMeldResourceObject.GetComponent<Image>().color = Color.magenta;
        shadowMeldResourceObject.GetComponent<Image>().fillAmount = player.GetComponent<PlayerMovement>().shadowMeldResource / 100;
    }
    #endregion

    #region Tooltip Control
    public static void Toggle3DMovementTooltips(bool on)
    {
        w_Tooltip.SetActive(on);
        a_Tooltip.SetActive(on);
        s_Tooltip.SetActive(on);
        d_Tooltip.SetActive(on);
        space_Tooltip.SetActive(on);
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

    public static void CheckInteractToolip(bool on)
    {
        instance.StartCoroutine(instance.ToggleInteractTooltip(on));
    }

    public IEnumerator ToggleInteractTooltip(bool on)
    {
        if (!e_First_Time_Used)
        {
            e_Tooltip.GetComponent<Image>().color = Color.yellow;
            e_Tooltip.SetActive(on);
            e_First_Time_Used = true;
            yield return new WaitForSeconds(2f);
            e_Tooltip.GetComponent<Image>().color = Color.white;
        }
        else
        {
            e_Tooltip.SetActive(on);
        }
    }

    public static void CheckShadowMeldTooltip(bool on)
    {
        instance.StartCoroutine(instance.ToggleShadowMeldTooltip(on));
    }

    public IEnumerator ToggleShadowMeldTooltip(bool on)
    {
        if (!f_First_Time_Used)
        {
            f_Tooltip.GetComponent<Image>().color = Color.magenta;
            f_Tooltip.SetActive(on);
            f_First_Time_Used = true;
            yield return new WaitForSeconds(2f);
            f_Tooltip.GetComponent<Image>().color = Color.white;
        }
        else
        {
            f_Tooltip.SetActive(on);
        }
    }

    public static void ToggleShadowShiftInTooltip(bool on)
    {
        shift_Tooltip.SetActive(on);
    }
    #endregion

    // Method invoked by other classes that resets the level
    // as a whole by resetting player position to the 3D start
    // position (in the case that the player is in 2D, manually
    // removes player from 2D and resets, and flips a global static
    // boolean called 'Resetting' on that all dynamic objects (push cube)
    // check in update to see if they need to reset to their start position
    public IEnumerator ResetLevel()
	{
        // Turn resetting on
		resetting = true;
        // Check if the player is in 2D space
		if(!PlayerMovement.in3DSpace)
		{
            // If so, remove them from 2D space first
			PlayerMovement.in3DSpace = true;
			playerShadow.GetComponent<CharacterController>().enabled = false;
			player.GetComponent<CharacterController>().enabled = true;
			player.GetComponent<PlayerMovement>().controller = player.GetComponent<CharacterController>();
		}
        // Then, reset the player's position to the start position
		player.transform.position = PlayerMovement.playerStartPosition;

        player.GetComponent<PlayerMovement>().shadowMeldResource = 100;
        player.GetComponent<PlayerMovement>().shadowMeldVFX.SetActive(false);
        player.layer = LayerMask.NameToLayer("Form");
		Debug.Log ("Before wait");
        yield return new WaitForSeconds(0.5f);
		Debug.Log ("After wait");
        resetting = false;
        PlayerMovement.shadowMelded = false;
	}

    

    public static void ScoreIncrement(int amount)
	{
		score += amount;
	}
}
