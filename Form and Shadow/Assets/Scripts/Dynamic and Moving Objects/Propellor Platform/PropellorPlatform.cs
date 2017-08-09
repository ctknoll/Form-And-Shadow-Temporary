using UnityEngine;

public class PropellorPlatform : MonoBehaviour {
	[Range(60, 120)] public float rotationSpeed = 60f;
    [SerializeField] bool rotateClockwise = true;

	void Start()
	{
		GetComponent<BoxCollider>().size = new Vector3(gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.x, 0.5f, 
			gameObject.transform.GetChild(0).gameObject.GetComponent<Transform>().localScale.z);
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.transform.parent = null;
		}
	}

	void Update () 
	{
		if(PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PLAYERSTATE.SHIFTING && !GameController.resetting && !GameController.paused)
        {
            if(rotateClockwise)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
            }
            else
            {
                transform.Rotate(Vector3.up, Time.deltaTime * -rotationSpeed);
            }
        }
    }
}