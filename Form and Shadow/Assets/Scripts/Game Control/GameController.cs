using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*

    Written by: Daniel Colina and Chris Knoll
    --GameController--
    A master control script for handling UI, resetting the level, and score.
    Attached to the GameController prefab in each scene.

*/
public class GameController : MonoBehaviour
{
    public static GameController m_Instance;
	public static bool m_Resetting;
    public static bool m_Paused;
    public static float m_MasterTimer;

    GameObject shadowmeldResourceBar;

	void Start () 
	{
        m_Instance = this;
        m_Resetting = false;
        m_Paused = false;
        m_MasterTimer = 0f;
        shadowmeldResourceBar = GameObject.Find("Shadowmeld_Resource_Bar");
    }

    void Update()
    {
        if(!m_Paused && !m_Resetting)
        {
            switch (PlayerShadowInteraction.m_CurrentPlayerState)
            {
                case PlayerShadowInteraction.PlayerState.Form:
                    break;
                case PlayerShadowInteraction.PlayerState.Shadow:
                    break;
                case PlayerShadowInteraction.PlayerState.Grabbing:
                    break;
                case PlayerShadowInteraction.PlayerState.Shifting:
                    break;
                case PlayerShadowInteraction.PlayerState.Shadowmelded:
                    break;
                default:
                    break;
            }
            m_MasterTimer += Time.deltaTime;
        }
        ShadowmeldUIControl();
    }

    #region UI Control
    void ShadowmeldUIControl()
    {
        shadowmeldResourceBar.GetComponent<Slider>().value = PlayerShadowInteraction.m_Instance.m_CurrentShadowmeldResource / PlayerShadowInteraction.m_Instance.m_MaxShadowmeldResource;
    }
    #endregion

    public void ResetLevel()
	{
        m_Resetting = true;

        // Check if the player is in 2D space or shadowmelded
        switch (PlayerShadowInteraction.m_CurrentPlayerState)
        {
            case PlayerShadowInteraction.PlayerState.Shadow:
                PlayerShadowInteraction.m_Instance.FinishShiftingOut();
                break;
            case PlayerShadowInteraction.PlayerState.Shadowmelded:
                PlayerShadowInteraction.m_Instance.ExitShadowmeld();
                break;
            case PlayerShadowInteraction.PlayerState.Grabbing:
                Debug.Log("releasing");
                PlayerMotor.m_Instance.m_GrabbedObjectTransform.gameObject.transform.parent = null;
                PlayerMotor.m_Instance.m_GrabbedObjectTransform.gameObject.GetComponent<PushCube>().m_Grabbed = false;
                PlayerMotor.m_Instance.m_GrabbedObjectTransform = null;
                PlayerShadowInteraction.m_CurrentPlayerState = PlayerShadowInteraction.PlayerState.Form;
                break;
        }

        PlayerShadowInteraction.m_Instance.transform.position = PlayerShadowInteraction.m_PlayerRespawnPosition;
        PlayerShadowInteraction.m_Instance.m_CurrentShadowmeldResource = PlayerShadowInteraction.m_Instance.m_MaxShadowmeldResource;
        m_Resetting = false;
	}
}
