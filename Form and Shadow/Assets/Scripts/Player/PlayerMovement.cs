using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public Transform cameraTransform;
	public float movementSpeed;
	public float jumpSpeed;
	public float jumpTime;
	public float gravity;

    public GameObject shiftedPlane = null;
    public int shiftTimer = 0;

    private Vector3 moveDirection = Vector3.zero;
	private Vector3 rotationDirection;
	private float jumpHeldTime;

    [SerializeField]
    private CharacterController controller;

	void Start()
	{
		controller = GetComponent<CharacterController>();
	}
	void Update() 
	{
        if(Input.GetButtonDown("Fire3"))
        {
            Debug.Log("Recieved");
            if (CameraControl.in3DSpace)
            {
                Debug.Log("Not In Wall");
				CameraControl.in3DSpace = !CastToWall.castToWall(gameObject);
                if (!CameraControl.in3DSpace) return;
            }
            else
            {
                Debug.Log("In Wall");
				GameObject parent = GameObject.Find("Player_Parent");
				foreach (Transform child in parent.transform)
					child.transform.gameObject.SetActive(true);
				CameraControl.in3DSpace = CastToWall.removeFromWall(GameObject.Find("Player"), this.gameObject);
                if (CameraControl.in3DSpace) return;
				else transform.Find("Player").gameObject.SetActive(false);
            }
        }
			

        //Debug.Log("Check if moving");
        if (!CameraControl.in3DSpace)
        {
            Debug.Log("Trying to move");
			Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
            //Debug.Log(dir);

			if (Input.GetButton("Jump") && jumpHeldTime < jumpTime)
			{
				Debug.Log ("2D JUMP");
				controller.Move(shiftedPlane.GetComponent<Transform>().forward * jumpSpeed * Time.deltaTime);
				jumpHeldTime += Time.deltaTime;
			}
			if(Input.GetButtonUp("Jump"))
				jumpHeldTime = 0;

			controller.Move(-1 * shiftedPlane.GetComponent<Transform>().forward * gravity * Time.deltaTime);

			Vector3 movement = (shiftedPlane.GetComponent<Transform>().right * dir.x) + (shiftedPlane.GetComponent<Transform>().forward * dir.y);
            controller.Move(movement * Time.deltaTime * movementSpeed);
        }

        else
        {
			if (Input.GetButton("Jump") && jumpHeldTime < jumpTime)
			{
				controller.Move(new Vector3(0, jumpSpeed * Time.deltaTime, 0));
				jumpHeldTime += Time.deltaTime;
			}
			if(Input.GetButtonUp("Jump"))
				jumpHeldTime = 0;
			
            controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
            rotationDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
            transform.rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);

            if (Input.GetAxis("Horizontal") >= 0)
            {
                controller.Move(transform.right * Time.deltaTime * movementSpeed);
            }
            if (Input.GetAxis("Horizontal") <= 0)
            {
                controller.Move(-transform.right * Time.deltaTime * movementSpeed);
            }
            if (Input.GetAxis("Vertical") >= 0)
            {
                controller.Move(transform.forward * Time.deltaTime * movementSpeed);
            }
            if (Input.GetAxis("Vertical") <= 0)
            {
                controller.Move(-transform.forward * Time.deltaTime * movementSpeed);
            }
        }
	}
}