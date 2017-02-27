using UnityEngine;

public class MoveCube : MonoBehaviour {
    public bool canInteract;
    public bool grabbed;
    public bool blockedAhead;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Vector3 directionAwayFromPlayer;
	private Vector3 startPos;

	void Start()
	{
		player = GameObject.Find("Player_Character");
		startPos = transform.position;
	}

	void Update()
	{
		if(canInteract)
		{
            if (!grabbed)
			{
                GameController.ToggleInteractTooltip(true);
                if (Input.GetButton("Grab"))
                {
                    GameController.ToggleInteractTooltip(false);
                    grabbed = true;
                    player.transform.rotation = Quaternion.LookRotation(directionAwayFromPlayer, Vector3.up);
                    transform.parent = player.transform;
					PlayerMovement.isGrabbing = true;
					PlayerMovement.grabbedObject = gameObject;
				}
			}

            if (Input.GetButtonUp("Grab"))
            {
                grabbed = false;
                transform.parent = null;
                PlayerMovement.isGrabbing = false;
                PlayerMovement.grabbedObject = null;
            }
        }

		if(GameController.resetting)
		{
            GameController.ToggleInteractTooltip(false);
            transform.position = startPos;
			grabbed = false;
            transform.parent = null;
			PlayerMovement.isGrabbing = false;
			PlayerMovement.grabbedObject = null;
		}
	}
}
