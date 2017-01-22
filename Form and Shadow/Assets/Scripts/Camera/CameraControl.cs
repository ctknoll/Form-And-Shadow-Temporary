using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class CameraControl : MonoBehaviour
{
	public static bool in3DSpace;
	[Header("Player Target")]
    public Transform target;

	[Header("3D Camera Variables")]
	public float distanceToPlayer3D;
    public float xSpeed;
    public float ySpeed;

    public float yMinLimit;
    public float yMaxLimit;

    float x = 0.0f;
    float y = 0.0f;

	[Header("2D Camera Variables")]
	public bool smooth;
	public float smoothSpeed = 0.125f;
	public float distanceToPlayer2D;
	public Vector3 normalToWall = new Vector3();

    void Start()
    {
		in3DSpace = true;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

	void Update()
	{
        if (Input.GetButtonDown("Cancel"))
            Application.Quit();

        distanceToPlayer3D = RaycastToCamera.distance;

		if(in3DSpace && GetComponent<Camera>().orthographic)
			GetComponent<Camera>().orthographic = false;
		
		if(distanceToPlayer3D > 8)
		{
			distanceToPlayer3D = 8;
		}
	}

    void LateUpdate()
    {
		if (target && in3DSpace)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distanceToPlayer3D * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distanceToPlayer3D);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }

		else if(target && !in3DSpace)
		{
			transform.rotation = Quaternion.LookRotation(LightSourceControl.lightSourceDirection, Vector3.up);
			Vector3 desiredPosition = target.transform.position + normalToWall * distanceToPlayer2D;
			if(smooth)
				transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
			else
				transform.position = desiredPosition;
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