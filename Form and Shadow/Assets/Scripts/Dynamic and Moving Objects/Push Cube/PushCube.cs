using UnityEngine;

public class PushCube : MonoBehaviour {
    public bool canInteract;
    public bool grabbed;
    public bool blockedAhead;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Vector3 directionAwayFromPlayer;
    [HideInInspector]
	public Vector3 startPos;
    public AudioSource moveAudioSource;
    public AudioSource grabAudioSource;
    public AudioClip grabAudioClip;
    public AudioClip releaseAudioClip;

	void Start()
	{
		player = GameObject.Find("Player_Character");
		startPos = transform.position;
	}

	void Update()
	{
        if (!grabbed)
        {
            GetComponent<AudioSource>().Stop();
        }
        if (canInteract)
        {
            if (!grabbed)
            {
                GameController.CheckInteractToolip(true, true);
                if (Input.GetButtonDown("Grab"))
                {
                    grabAudioSource.clip = grabAudioClip;
                    grabAudioSource.Play();
                    if (!GameController.e_Grab_First_Time_Used)
                        GameController.e_Grab_First_Time_Used = true;
                    GameController.CheckInteractToolip(false, true);
                    grabbed = true;
                    player.transform.rotation = Quaternion.LookRotation(directionAwayFromPlayer, Vector3.up);
                    transform.parent = player.transform;
                    PlayerShadowInteraction.isGrabbing = true;
                    PlayerShadowInteraction.grabbedObject = gameObject;
                }
            }

            if (Input.GetButtonUp("Grab"))
            {
                grabAudioSource.clip = releaseAudioClip;
                grabAudioSource.Play();
                grabbed = false;
                transform.parent = null;
                PlayerShadowInteraction.isGrabbing = false;
                PlayerShadowInteraction.grabbedObject = null;
            }
        }

		if(GameController.resetting)
		{
            GameController.CheckInteractToolip(false, true);
            transform.position = startPos;
			grabbed = false;
            transform.parent = null;
			PlayerShadowInteraction.isGrabbing = false;
			PlayerShadowInteraction.grabbedObject = null;
		}
	}
}
