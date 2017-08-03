using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Camera))]

public class NewCameraControl : MonoBehaviour
{
    public static NewCameraControl m_Instance;

    [Header("Player Target 2D and 3D")]
    [SerializeField] Transform m_CameraTarget;
    Camera m_Camera;

    [Header("3D Camera Variables")]
    float m_CurrentDistance = 5f;
    [Range(8, 12)] public float m_DistanceMax = 10f;
    [Range(3, 6)][SerializeField] float m_DistanceMin = 3f;
    [Range(0.02f, 0.1f)][SerializeField] float m_DistanceSmooth = 0.05f;
    [Range(0.75f, 1.5f)][SerializeField] float m_DistanceResumeSmooth = 1f;
    [Range(4, 7)][SerializeField] float m_XMouseRotationSpeed = 5f;
    [Range(4, 7)][SerializeField] float m_YMouseRotationSpeed = 5f;
    [Range(1, 3)][SerializeField] float m_MouseWheelSensitivity = 1f;
    [Range(0.02f, 0.1f)][SerializeField] float m_XSmooth = 0.05f;
    [Range(0.075f, 0.12f)][SerializeField] float m_YSmooth = 0.1f;
    [Range(-75, 0)][SerializeField] float m_YMinPanLimit = -40f;
    [Range(80, 120)][SerializeField] float m_YMaxPanLimit = 80f;



    float mouseX = 0f;
    float mouseY = 0f;
    float velX = 0f;
    float velY = 0f;
    float velZ = 0f;
    float velocityDistance = 0f;
    float startDistance = 0f;
    float occlusionDistanceStep = 0.5f;
    int maxOcclusionChecks = 10;
    Vector3 position = Vector3.zero;
    Vector3 desiredPosition = Vector3.zero;
    float desiredDistance = 0f;
    float distanceSmooth = 0f;
    float preOccludedDistance = 0f;

    [Header("2D Camera Variables")]
    [Range(4, 6)] public float m_DistanceToPlayer2D;
    [Range(0.1f, 0.2f)][SerializeField] float m_CameraSmoothSpeed2D;

    public static bool cameraIsPanning;

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        m_CurrentDistance = Mathf.Clamp(m_CurrentDistance, m_DistanceMin, m_DistanceMax);
        startDistance = m_CurrentDistance;
        Reset();
        
        cameraIsPanning = false;
        m_Camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        switch(PlayerShadowInteraction.m_CurrentPlayerState)
        {
            case PlayerShadowInteraction.PLAYERSTATE.SHADOW:
                Update2DCameraMovement();
                break;
            case PlayerShadowInteraction.PLAYERSTATE.SHIFTING:
                UpdateShiftingCameraMovement();
                break;
            default:
                Update3DCameraMovement();
                break;
        }
    }

    public void Reset()
    {
        mouseX = 0;
        mouseY = 10;
        m_CurrentDistance = startDistance;
        desiredDistance = m_CurrentDistance;
        preOccludedDistance = m_CurrentDistance;
    }

    void Update3DCameraMovement()
    {
        m_Camera.orthographic = false;
        m_Camera.clearFlags = CameraClearFlags.Skybox;

        HandlePlayerInput();

        var count = 0;
        do
        {
            CalculateDesiredPosition();
            count++;
        } while (CheckIfOccluded(count));
        
        UpdatePosition3D();
    }

    void HandlePlayerInput()
    {
        var deadZone = 0.01f;
        if(Input.GetMouseButton(1))
        {
            // If the RMB is down, get mouse axis input to rotate camera
            mouseX += CrossPlatformInputManager.GetAxis("Mouse X") * m_XMouseRotationSpeed;
            mouseY -= CrossPlatformInputManager.GetAxis("Mouse Y") * m_YMouseRotationSpeed;
        }
        // Limit mouse Y
        mouseY = ClampAngle(mouseY, m_YMinPanLimit, m_YMaxPanLimit);

        if (CrossPlatformInputManager.GetAxis("Mouse ScrollWheel") < -deadZone || CrossPlatformInputManager.GetAxis("Mouse ScrollWheel") > deadZone)
        {
            desiredDistance = Mathf.Clamp(m_CurrentDistance - CrossPlatformInputManager.GetAxis("Mouse ScrollWheel") * m_MouseWheelSensitivity, m_DistanceMin, m_DistanceMax);
            preOccludedDistance = desiredDistance;
            distanceSmooth = m_DistanceSmooth;
        }
    }

    void CalculateDesiredPosition()
    {
        // Evaluate distance
        ResetDesiredDistance();
        m_CurrentDistance = Mathf.SmoothDamp(m_CurrentDistance, desiredDistance, ref velocityDistance, distanceSmooth);

        // Calculate desired position
        desiredPosition = CalculatePosition(mouseY, mouseX, m_CurrentDistance);
    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return m_CameraTarget.position + rotation * direction;
    }

    bool CheckIfOccluded(int count)
    {
        var isOccluded = false;

        var nearestDistance = CheckCameraPoints(m_CameraTarget.position, desiredPosition);

        if (nearestDistance != -1)
        {
            if (count < maxOcclusionChecks)
            {
                isOccluded = true;
                m_CurrentDistance -= occlusionDistanceStep;

                if (m_CurrentDistance < 0.25f)
                    m_CurrentDistance = 0.25f;
            }
            else
                m_CurrentDistance = nearestDistance - Camera.main.nearClipPlane;

            desiredDistance = m_CurrentDistance;
            distanceSmooth = m_DistanceResumeSmooth;
        }
        return isOccluded;
    }

    float CheckCameraPoints(Vector3 from, Vector3 to)
    {
        var nearestDistance = -1f;

        RaycastHit hitInfo;

        ClipPlanePoints clipPlanePoints = ClipPlaneAtNear(to);

        // Draw lines in the editor to make it easier to visualize
        Debug.DrawLine(from, to + transform.forward * -m_Camera.nearClipPlane, Color.red);
        Debug.DrawLine(from, clipPlanePoints.upperLeft);
        Debug.DrawLine(from, clipPlanePoints.upperRight);
        Debug.DrawLine(from, clipPlanePoints.lowerLeft);
        Debug.DrawLine(from, clipPlanePoints.lowerRight);

        Debug.DrawLine(clipPlanePoints.upperLeft, clipPlanePoints.upperRight);
        Debug.DrawLine(clipPlanePoints.upperRight, clipPlanePoints.lowerRight);
        Debug.DrawLine(clipPlanePoints.lowerRight, clipPlanePoints.lowerLeft);
        Debug.DrawLine(clipPlanePoints.lowerLeft, clipPlanePoints.upperLeft);

        if (Physics.Linecast(from, clipPlanePoints.upperLeft, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.lowerLeft, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            if(hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.upperRight, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.lowerRight, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, to + transform.forward * -m_Camera.nearClipPlane, out hitInfo) && !hitInfo.collider.CompareTag("Player"))
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
                nearestDistance = hitInfo.distance;

        return nearestDistance;
    }

    void ResetDesiredDistance()
    {
        if (desiredDistance < preOccludedDistance)
        {
            var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);

            var nearestDistance = CheckCameraPoints(m_CameraTarget.position, pos);

            if (nearestDistance == -1 || nearestDistance > preOccludedDistance)
            {
                desiredDistance = preOccludedDistance;
            }
        }
    }

    void UpdatePosition3D()
    {
        var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, m_XSmooth);
        var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, m_YSmooth);
        var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, m_XSmooth);
        position = new Vector3(posX, posY, posZ);

        transform.position = position;
        transform.LookAt(m_CameraTarget);
    }

    void Update2DCameraMovement()
    {
        m_Camera.orthographic = true;
        m_Camera.clearFlags = CameraClearFlags.SolidColor;
        m_Camera.backgroundColor = Color.black;

        Vector3 desiredPosition = m_CameraTarget.transform.position + -transform.forward * m_DistanceToPlayer2D;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, m_CameraSmoothSpeed2D);
    }

    void UpdateShiftingCameraMovement()
    {
        m_Camera.orthographic = false;
        m_Camera.clearFlags = CameraClearFlags.Skybox;
        transform.LookAt(m_CameraTarget.GetComponentInParent<PlayerShadowInteraction>().m_ShadowShiftFollowObject.transform);
    }

    public struct ClipPlanePoints
    {
        public Vector3 upperLeft;
        public Vector3 upperRight;
        public Vector3 lowerLeft;
        public Vector3 lowerRight;
    }

    public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos)
    {
        var clipPlanePoints = new ClipPlanePoints();

        if (!Camera.main)
            return clipPlanePoints;

        var transform = Camera.main.transform;
        var halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
        var aspect = Camera.main.aspect;
        var distance = Camera.main.nearClipPlane;
        var height = distance * Mathf.Tan(halfFOV);
        var width = height * aspect;

        clipPlanePoints.lowerRight = pos + transform.right * width;
        clipPlanePoints.lowerRight -= transform.up * height;
        clipPlanePoints.lowerRight += transform.forward * distance;

        clipPlanePoints.lowerLeft = pos - transform.right * width;
        clipPlanePoints.lowerLeft -= transform.up * height;
        clipPlanePoints.lowerLeft += transform.forward * distance;

        clipPlanePoints.upperRight = pos + transform.right * width;
        clipPlanePoints.upperRight += transform.up * height;
        clipPlanePoints.upperRight += transform.forward * distance;

        clipPlanePoints.upperLeft = pos - transform.right * width;
        clipPlanePoints.upperLeft += transform.up * height;
        clipPlanePoints.upperLeft += transform.forward * distance;
        return clipPlanePoints;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        do
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
        } while (angle < -360 || angle > 360);
        return Mathf.Clamp(angle, min, max);
    }
}