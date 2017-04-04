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
    public static bool paused;
	public int score;

    public float playerDeathAnimationDuration;
    private float playerDeathTimerStart;
    private float deathTimer;

    private static GameObject w_Tooltip;
    private static GameObject a_Tooltip;
    private static GameObject s_Tooltip;
    private static GameObject d_Tooltip;
    private static GameObject e_Tooltip;
	private static GameObject f_Tooltip;
    private static GameObject space_Tooltip;
    private static GameObject shift_Tooltip;
    private static GameObject interact_Tutorial_Tooltip;
    private static GameObject shadowInteract_Tutorial_Tooltip;
    private GameObject pause_Menu_Panel;

    public string interactGrabTutorialText;
    public string interactSwitchTutorialText;
    public string shadowMeldTutorialText;
    public string shadowShiftTutorialText;
    public AudioSource ambientAudioSource;
    public AudioSource playerDeathAudioSource;
    public AudioClip playerDeathAudioClip;
    public AudioClip ambientGearsAudioClip;
    public AudioClip ambientShadowmeldAudioClip;

    public static bool e_Switch_First_Time_Used;
    public static bool e_Grab_First_Time_Used;
    public static bool shadowMeld_First_Time_Used;
    public static bool shadowShift_First_Time_Used;

    private GameObject scoreText;
    private GameObject shadowMeldResourceObject;
    
	private GameObject player;
    private GameObject playerMesh;
    private GameObject playerShadow;
    [HideInInspector]
    public static GameController instance;

	void Start () 
	{
		player = GameObject.Find("Player_Character");
		playerShadow = GameObject.Find("Player_Shadow");
        playerMesh = player.transform.FindChild("Player_Mesh").gameObject;

        instance = GetComponent<GameController>();
        scoreText = GameObject.Find("Score_Text");
        w_Tooltip = GameObject.Find("W_Tooltip");
        a_Tooltip = GameObject.Find("A_Tooltip");
        s_Tooltip = GameObject.Find("S_Tooltip");
        d_Tooltip = GameObject.Find("D_Tooltip");
        e_Tooltip = GameObject.Find("E_Tooltip");
        e_Tooltip.GetComponent<Image>().color = Color.yellow;
        f_Tooltip = GameObject.Find("F_Tooltip");
        f_Tooltip.GetComponent<Image>().color = Color.magenta;
        shadowInteract_Tutorial_Tooltip = GameObject.Find("Shadow_Interact_Tutorial_Tooltip");
        interact_Tutorial_Tooltip = GameObject.Find("Interact_Tutorial_Tooltip");
        pause_Menu_Panel = GameObject.Find("Pause_Menu_Panel");
        e_Switch_First_Time_Used = false;
        e_Grab_First_Time_Used = false;
        shadowMeld_First_Time_Used = false;
        shadowShift_First_Time_Used = false;

        e_Tooltip.SetActive(false);
		f_Tooltip.SetActive(false);
        pause_Menu_Panel.SetActive(false);
        space_Tooltip = GameObject.Find("Space_Tooltip");
        shift_Tooltip = GameObject.Find("Shift_Tooltip");
        shadowMeldResourceObject = GameObject.Find("Shadowmeld_Resource");
        ambientAudioSource.clip = ambientGearsAudioClip;
        ambientAudioSource.Play();

        Cursor.visible = false;
    }

    void Update()
    {
        ScoreUIControl();
        ShadowmeldUIControl();

        if(Input.GetButtonDown("Quit") && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
        {
            ToggleGamePause();
        }
        ControlAmbientAudio();
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

    public static void CheckInteractToolip(bool on, bool isGrabObject)
    {
        instance.ToggleInteractTooltip(on, isGrabObject);
    }

    public void ToggleInteractTooltip(bool on, bool isGrabObject)
    {
        // Check if the tooltip is actually being turned on. If so, continue.
        if (on)
        {
            // Check if the interactable object being passed in is a cube or
            // a switch, and call the ChangeTutorialTooltip method based on 
            // which, passing in the string defined by the player on the master
            // GameController object.
            if (isGrabObject)
            {
                if (!e_Grab_First_Time_Used)
                {
                    ChangeInteractTutorialTooltip(interactGrabTutorialText);
                }
                else
                    ChangeInteractTutorialTooltip("");
            }
            else
            {
                if (!e_Switch_First_Time_Used)
                {
                    ChangeInteractTutorialTooltip(interactSwitchTutorialText);
                }
                else
                    ChangeInteractTutorialTooltip("");
            }
        }
        interact_Tutorial_Tooltip.SetActive(on);
        e_Tooltip.SetActive(on);
    }
        
    // Shadow interaction tooltip toggling and tutorial methods
    public static void CheckShadowMeldTooltip(bool on)
    {
        instance.ToggleShadowMeldTooltip(on);
    }

    public void ToggleShadowMeldTooltip(bool on)
    {
        if(on)
        {
            if (!shadowMeld_First_Time_Used)
            {
                ChangeShadowInteractTutorialTooltip(shadowMeldTutorialText);
            }
            else
                shadowInteract_Tutorial_Tooltip.GetComponent<Text>().text = "";
        }
        shadowInteract_Tutorial_Tooltip.SetActive(on);
        f_Tooltip.SetActive(on);
    }
    
    public static void CheckShadowShiftTooltip(bool on)
    {
        instance.ToggleShadowShiftInTooltip(on);
    }

    public void ToggleShadowShiftInTooltip(bool on)
    {
        if(on)
        {
            if (!shadowShift_First_Time_Used)
            {
                ChangeShadowInteractTutorialTooltip(shadowShiftTutorialText);
            }
            else
                shadowInteract_Tutorial_Tooltip.GetComponent<Text>().text = "";
        }
        shadowInteract_Tutorial_Tooltip.SetActive(on);
        shift_Tooltip.SetActive(on);
    }

    public static void ChangeInteractTutorialTooltip(string tutorialText)
    {
        interact_Tutorial_Tooltip.GetComponent<Text>().text = tutorialText;
    }

    public static void ChangeShadowInteractTutorialTooltip(string tutorialText)
    {
        shadowInteract_Tutorial_Tooltip.GetComponent<Text>().text = tutorialText;
    }
    #endregion

    public void ToggleGamePause()
    {
        if (!paused)
        {
            Time.timeScale = 0;
            ambientAudioSource.Pause();
        }
        else
        {
            Time.timeScale = 1;
            ambientAudioSource.UnPause();
        }
        if (!pause_Menu_Panel.activeSelf)
            pause_Menu_Panel.SetActive(true);
        else
            pause_Menu_Panel.SetActive(false);
        Cursor.visible = !Cursor.visible;
        paused = !paused;
        ChangeInteractTutorialTooltip("");
        ChangeShadowInteractTutorialTooltip("");
    }

    public void ControlAmbientAudio()
    {
        if(PlayerMovement.shadowMelded)
        {
            ambientAudioSource.Pause();
            ambientAudioSource.clip = ambientShadowmeldAudioClip;
            ambientAudioSource.Play();
        }
        else
        {
            ambientAudioSource.Pause();
            ambientAudioSource.clip = ambientGearsAudioClip;
            ambientAudioSource.Play();
        }
    }

    public void QuitToMainMenu()
    {
        paused = false;
        SceneManager.LoadScene("Menu_Title");
    }

    // Method invoked by other classes that resets the level
    // as a whole by resetting player position to the 3D start
    // position (in the case that the player is in 2D, manually
    // removes player from 2D and resets, and flips a global static
    // boolean called 'Resetting' on that all dynamic objects (push cube)
    // check in update to see if they need to reset to their start position
    public IEnumerator ResetLevel(bool resetWithK)
	{
        // Turn resetting on
        resetting = true;
        playerDeathAudioSource.Play();
        // Plays the player's death animation
        if(!resetWithK)
        {
            StartCoroutine(PlayerDeathAnimation());
            yield return new WaitForSeconds(playerDeathAnimationDuration);
        }
        
        // Check if the player is in 2D space
        if (!PlayerMovement.in3DSpace)
		{
            // If so, remove them from 2D space first
			PlayerMovement.in3DSpace = true;
			playerShadow.GetComponent<CharacterController>().enabled = false;
			player.GetComponent<CharacterController>().enabled = true;
			player.GetComponent<PlayerMovement>().controller = player.GetComponent<CharacterController>();
		}
        // Then, reset the player's position to the start position
		player.transform.position = PlayerMovement.playerStartPosition;
        playerMesh.transform.localScale = new Vector3(1, 1, 1);

        player.GetComponent<PlayerMovement>().shadowMeldResource = 100;
        player.GetComponent<PlayerMovement>().shadowMeldVFX.SetActive(false);
        player.layer = LayerMask.NameToLayer("Form");
        yield return new WaitForSeconds(0.5f);
        resetting = false;
        PlayerMovement.shadowMelded = false;
	}

    public IEnumerator PlayerDeathAnimation()
    {
        Vector3 startScale = playerMesh.transform.localScale;
        Vector3 endScale = new Vector3(0f, 0f, 0f);
        playerDeathTimerStart = Time.time;
        deathTimer = playerDeathTimerStart;

        while (deathTimer < playerDeathTimerStart + playerDeathAnimationDuration)
        {
            deathTimer += Time.deltaTime;
            playerMesh.transform.localScale = Vector3.Lerp(startScale, endScale, (deathTimer - playerDeathTimerStart) / playerDeathAnimationDuration);
            yield return null;
        }
    }
    
    public void ScoreIncrement(int amount)
	{
		score += amount;
	}
}
