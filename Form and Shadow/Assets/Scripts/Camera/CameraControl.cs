using UnityEngine;

public class CameraControl : MonoBehaviour
{
    /*

    --Camera Control--
    Basic master control for the Main_Camera prefab, handling all logic of camera movement
    and looking, outside of the camera pan functions in PlayerMovement that handle the shadowshift
    enter, exit, and multiexit camera panning methods.

    */

	[Header("Player Target 2D and 3D")]
    public Transform target2D;
	public Transform target3D;

	[Header("3D Camera Variables")]
    // Camera follow distance
	public float distanceToPlayer3D;
    // Camera rotation speeds
    public float xMouseRotationSpeed;
    public float yMouseRotationSpeed;
    // Camera pan limits
    public float yMinPanLimit;
    public float yMaxPanLimit;

    private float currentDistanceToPlayer3D;
    private float x = 0.0f;
    private float y = 0.0f;

	[Header("2D Camera Variables")]
    public float distanceToPlayer2D;
    public float smoothSpeed = 0.125f;

	[HideInInspector]
	public static bool cameraIsPanning;



    void Start()
    {
		cameraIsPanning = false;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }


    void Update()
    {
        //Updating camera distance on every frame
        currentDistanceToPlayer3D = RaycastToCamera.distance;

        //Setting maximum distance so the camera doesnt go too far
        if (currentDistanceToPlayer3D > distanceToPlayer3D)
        {
            currentDistanceToPlayer3D = distanceToPlayer3D;
        }

    }
    void LateUpdate()
    {
        if (!cameraIsPanning && !PlayerShadowInteraction.shadowShiftingOut && !PlayerShadowInteraction.shadowShiftingIn && !GameController.paused)
		{
			if (PlayerShadowInteraction.in3DSpace)
	        {
                GetComponent<Camera>().orthographic = false;
                GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;

                x += Input.GetAxis("Mouse X") * xMouseRotationSpeed * distanceToPlayer3D * 0.02f;
	            y -= Input.GetAxis("Mouse Y") * yMouseRotationSpeed * 0.02f;

	            y = ClampAngle(y, yMinPanLimit, yMaxPanLimit);

	            Quaternion rotation = Quaternion.Euler(y, x, 0);

	            Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentDistanceToPlayer3D);
	            Vector3 position = rotation * negDistance + target3D.position;

	            transform.rotation = rotation;
	            transform.position = position;
	        }

			else
			{
                GetComponent<Camera>().orthographic = true;
                GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
                GetComponent<Camera>().backgroundColor = Color.black;

                Vector3 desiredPosition = target2D.transform.position + -transform.forward * distanceToPlayer2D;

				transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
			}
		}
		else if(!GameController.paused)
		{
            GetComponent<Camera>().orthographic = false;
            GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;

            if (PlayerShadowInteraction.in3DSpace)
				transform.LookAt(target3D.GetComponent<PlayerShadowInteraction>().shadowShiftFollowObject.transform);
			else if(!PlayerShadowInteraction.in3DSpace)
			{
				if(target3D.GetComponent<PlayerShadowInteraction>().shadowShiftFollowObject)
					transform.LookAt(target3D.GetComponent<PlayerShadowInteraction>().shadowShiftFollowObject.transform);
				else
					transform.LookAt(target3D.GetComponent<PlayerShadowInteraction>().shadowShiftFollowObject.transform);
			}
		}
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}