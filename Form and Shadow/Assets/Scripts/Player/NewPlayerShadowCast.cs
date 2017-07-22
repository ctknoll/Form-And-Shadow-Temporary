using UnityEngine;

public class NewPlayerShadowCast : MonoBehaviour
{
    [SerializeField] GameObject m_LightingMaster;
    [HideInInspector] public GameObject m_LightSourceAligned;

    void Start()
    {
        m_LightingMaster = GameObject.Find("Lighting");
    }

    public GameObject CheckLightSourceAligned()
    {
        GameObject mostAlignedLightSource = null;
        foreach(Transform lightTransform in m_LightingMaster.transform)
        {
            if (lightTransform.gameObject.activeSelf)
            {
                Transform cameraTransform = Camera.main.transform;
                cameraTransform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
                float tempAngle = Vector3.Angle(lightTransform.forward, cameraTransform.forward);
                if (Mathf.Abs(tempAngle) < 45)
                {
                    m_LightSourceAligned = lightTransform.gameObject;
                    mostAlignedLightSource = lightTransform.gameObject;
                }
            }
            else
                continue;
        }
        return mostAlignedLightSource;
    }
}
