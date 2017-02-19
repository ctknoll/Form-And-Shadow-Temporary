using UnityEngine;

public class MoveCube : MonoBehaviour {
	public bool canInteract;
	public bool grabbed;
	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public Vector3 directionAwayFromPlayer;
	public bool blockedAhead;

	private Vector3 gravityDirection;
	private Vector3 velocity = new Vector3(0, 0, 0);
	private Vector3 acceleration;
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
            GameController.ResetInteractText();
            transform.position = startPos;
			grabbed = false;
			transform.parent = null;
			PlayerMovement.isGrabbing = false;
			PlayerMovement.grabbedObject = null;
		}
	}

	void FixedUpdate()
	{
		gravityDirection = Vector3.down;

		acceleration = gravityDirection * Physics.gravity.magnitude;

		velocity += (acceleration * Time.deltaTime);

		RaycastHit hit;
		if(Physics.BoxCast(transform.position, transform.GetChild(0).lossyScale / 2.2f, velocity.normalized, out hit, transform.rotation, (acceleration * Time.deltaTime).magnitude))
		{
			velocity = Vector3.zero;
		}

		transform.position += velocity * Time.deltaTime;
	}
}
