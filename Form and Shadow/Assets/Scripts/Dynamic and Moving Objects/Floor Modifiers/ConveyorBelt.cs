using UnityEngine;

public class ConveyorBelt : MonoBehaviour {

    public float moveSpeed;
    public MoveDirection moveDirection;

	public bool useMomentum;
	public bool dismountPlayer;
	private float velocity;

    [HideInInspector]
    public enum MoveDirection {Forward, Right};

    // Use this for initialization
	void Start ()
    {
        foreach (Transform child in transform.parent)
        {
            if (child.name == "Conveyor_Direction_Pointer")
            {
                child.rotation = transform.parent.rotation;
                child.transform.Rotate(Vector3.up * 90, Space.Self);
                if (moveDirection == MoveDirection.Forward && moveSpeed < 0)
                    child.transform.Rotate(Vector3.up * 180, Space.Self);
                else if (moveDirection == MoveDirection.Right && moveSpeed >= 0)
                    child.transform.Rotate(Vector3.up * 90, Space.Self);
                else if (moveDirection == MoveDirection.Right && moveSpeed < 0)
                    child.transform.Rotate(Vector3.up * -90, Space.Self);
            }   
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
        if(GetComponent<AudioSource>())
        {
            if (GameController.paused)
            {
                GetComponent<AudioSource>().Pause();
            }
            else
            {
                GetComponent<AudioSource>().UnPause();
            }
        }
	}

	public void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.tag == "Player") 
		{
            Debug.Log("Enter: " + other.gameObject.GetComponent<PlayerMovement>().gravConst);
			other.gameObject.GetComponent<PlayerMovement>().gravity = 0;
		}
	}

	public void OnTriggerStay(Collider other)
    {
        if(!GameController.paused)
        {
            if (other.GetComponent<Collider>() != null && !other.GetComponent<Collider>().isTrigger)
            {
                if (useMomentum)
                {
                    Vector3 directionToMove = new Vector3();
                    if (moveDirection == MoveDirection.Forward)
                    {
                        if (moveSpeed >= 0)
                            directionToMove = -1 * transform.parent.FindChild("Conveyor_Direction_Pointer").right;
                        else if (moveSpeed < 0)
                            directionToMove = transform.parent.FindChild("Conveyor_Direction_Pointer").right;
                    }
                    else
                    {
                        if (moveSpeed >= 0)
                            directionToMove = -1 * transform.parent.FindChild("Conveyor_Direction_Pointer").forward;
                        else if (moveSpeed < 0)
                            directionToMove = transform.parent.FindChild("Conveyor_Direction_Pointer").forward;
                    }

                    if (other.gameObject.tag == "Player")
                    {
                        other.gameObject.GetComponent<PlayerMovement>().conveyorVelocity = moveSpeed * directionToMove;
                    }
                    else
                    {
                        if (moveDirection == MoveDirection.Forward)
                        {
                            other.transform.position += moveSpeed * Time.deltaTime * transform.forward;
                        }
                        else other.transform.position += moveSpeed * Time.deltaTime * transform.right;
                    }
                }
                else
                {
                    if (moveDirection == MoveDirection.Forward)
                    {
                        other.transform.position += moveSpeed * Time.deltaTime * transform.forward;
                    }
                    else other.transform.position += moveSpeed * Time.deltaTime * transform.right;
                }
            }
        }
    }

	public void OnTriggerExit(Collider other)
	{
        if (other.gameObject.tag == "Player")
        {
            if (dismountPlayer) 
			{
				other.transform.position += other.transform.forward;
			}
			other.gameObject.GetComponent<PlayerMovement>().gravity = other.gameObject.GetComponent<PlayerMovement>().gravConst;
		}
	}
}