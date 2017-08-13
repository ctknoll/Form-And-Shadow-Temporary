using UnityEngine;

/*

-- Downward Shadow Control --
Attached to a downward facing light in the scene, lighting the level.
It's active when the player is not in 2D.

*/

public class DownwardShadowControl : MonoBehaviour 
{
	void Update ()
    {
        switch(PlayerShadowInteraction.m_CurrentPlayerState)
        {
            case PlayerShadowInteraction.PlayerState.Shadow:
                gameObject.GetComponent<Light>().enabled = false;
                break;
            case PlayerShadowInteraction.PlayerState.Shadowmelded:
                gameObject.GetComponent<Light>().enabled = false;
                break;
            default:
                gameObject.GetComponent<Light>().enabled = true;
                break;
        }
	}
}
