using UnityEngine;

[RequireComponent(typeof(Camera))]

public class NewCameraControl : MonoBehaviour
{
    [Header("Player Target 2D and 3D")]
    [SerializeField] Transform m_Target3D;
    [SerializeField] Transform m_Target2D;
    Camera m_Camera;

    [Header("3D Camera Variables")]
    public float m_DistanceToPlayer3D;
    [Range(30, 70)][SerializeField] float m_XMouseRotationSpeed;
    [Range(30, 70)][SerializeField] float m_YMouseRotationSpeed;
    [Range(-40, 10)][SerializeField] float m_YMinPanLimit;
    [Range(75, 90)][SerializeField] float m_YMaxPanLimit;
    float m_CurrentDistancetoPlayer3D;
    float x = 0.0f;
    float y = 0.0f;

    [Header("2D Camera Variables")]
    [Range(8, 12)] public float m_DistanceToPlayer2D;
    [Range(0.1f, 0.2f)][SerializeField] float m_CameraSmoothSpeed2D;

    public static bool cameraIsPanning;

    void Start()
    {
        cameraIsPanning = false;
        x = transform.eulerAngles.x;
        y = transform.eulerAngles.y;
        m_Camera = GetComponent<Camera>();
    }

    void Update()
    {
        m_CurrentDistancetoPlayer3D = RaycastToCamera.distance;

        if (m_CurrentDistancetoPlayer3D > m_DistanceToPlayer3D)
            m_CurrentDistancetoPlayer3D = m_DistanceToPlayer3D;
    }

    void LateUpdate()
    {
        switch(NewPlayerShadowInteraction.m_CurrentPlayerState)
        {
            case NewPlayerShadowInteraction.PLAYERSTATE.SHADOW:
                Update2DCameraMovement();
                break;
            case NewPlayerShadowInteraction.PLAYERSTATE.SHIFTING:
                UpdateShiftingCameraMovement();
                break;
            default:
                Update3DCameraMovement();
                break;
        }
    }

    void Update3DCameraMovement()
    {
        m_Camera.orthographic = false;
        m_Camera.clearFlags = CameraClearFlags.Skybox;

        x += Input.GetAxis("Mouse X") * m_XMouseRotationSpeed * m_DistanceToPlayer3D * 0.02f;
        y -= Input.GetAxis("Mouse Y") * m_YMouseRotationSpeed * 0.02f;

        y = ClampAngle(y, m_YMinPanLimit, m_YMaxPanLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -m_CurrentDistancetoPlayer3D);
        Vector3 position = rotation * negDistance + m_Target3D.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    void Update2DCameraMovement()
    {
        m_Camera.orthographic = true;
        m_Camera.clearFlags = CameraClearFlags.SolidColor;
        m_Camera.backgroundColor = Color.black;

        Vector3 desiredPosition = m_Target2D.transform.position + -transform.forward * m_DistanceToPlayer2D;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, m_CameraSmoothSpeed2D);
    }

    void UpdateShiftingCameraMovement()
    {
        m_Camera.orthographic = false;
        m_Camera.clearFlags = CameraClearFlags.Skybox;
        transform.LookAt(m_Target3D.GetComponentInParent<NewPlayerShadowInteraction>().m_ShadowShiftFollowObject.transform);
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