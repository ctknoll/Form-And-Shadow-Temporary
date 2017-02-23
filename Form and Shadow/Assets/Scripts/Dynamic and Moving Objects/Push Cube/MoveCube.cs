using UnityEngine;

public class MoveCube : MonoBehaviour {
	public bool canInteract;
	public bool grabbed;
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
                GameController.SetInteractText("Hold E to grab this cube from this side");
                if (Input.GetButtonDown("Grab"))
				{
                    GameController.ResetInteractText();
                    grabbed = true;
					PlayerMovement.isGrabbing = true;
					PlayerMovement.grabbedObject = gameObject;
				}
			}

            if (Input.GetButtonUp("Grab"))
            {
                grabbed = false;
                PlayerMovement.isGrabbing = false;
                PlayerMovement.grabbedObject = null;
            }
        }

		if(GameController.resetting)
		{
            GameController.ResetInteractText();
            transform.position = startPos;
			grabbed = false;
			PlayerMovement.isGrabbing = false;
			PlayerMovement.grabbedObject = null;
		}
	}
}
