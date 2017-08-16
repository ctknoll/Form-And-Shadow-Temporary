using UnityEngine;

public class GearPlatform : MonoBehaviour {
	[Range(30, 60)] public float rotationSpeed = 30;
    [SerializeField] bool rotateClockwise = true;

	void Start()
	{

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
		if(PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PlayerState.Shifting && !GameController.m_Resetting && !GameController.m_Paused)
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