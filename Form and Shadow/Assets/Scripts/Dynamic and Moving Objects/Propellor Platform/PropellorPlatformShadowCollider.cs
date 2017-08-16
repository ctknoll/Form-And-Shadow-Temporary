using UnityEngine;

public class PropellorPlatformShadowCollider : MonoBehaviour
{
    [HideInInspector] public float m_PropellorRotationSpeed;
    [HideInInspector] public bool m_ZAxisCast;

    float personalTime;
    Vector3 startingScale;
    Vector3 currentScale;

	void Start ()
	{
        if (m_ZAxisCast)
            startingScale = transform.lossyScale;
        else
            startingScale = new Vector3(transform.lossyScale.z, transform.lossyScale.y, transform.lossyScale.x);
        currentScale = transform.lossyScale;
        personalTime = 0;
    }

    void Update()
    {
        if (PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PlayerState.Shifting && !GameController.m_Resetting && !GameController.m_Paused)
            personalTime += Time.deltaTime;

        if (m_ZAxisCast)
            currentScale.x = startingScale.z - (startingScale.z - startingScale.x) * Mathf.Abs(Mathf.Cos(m_PropellorRotationSpeed * personalTime * Mathf.PI / 180));
        //GetComponent<BoxCollider>().size = new Vector3(m_PropellorMesh.transform.lossyScale.z - (m_PropellorMesh.transform.lossyScale.z - m_PropellorMesh.transform.lossyScale.x) * Mathf.Abs(Mathf.Cos((m_PropellorRotationSpeed * personalTime * Mathf.PI / 180))),
        //m_PropellorMesh.transform.lossyScale.y, m_PropellorMesh.transform.lossyScale.z);
        else
            currentScale.z = startingScale.x - (startingScale.x - startingScale.z) * Mathf.Abs(Mathf.Cos(m_PropellorRotationSpeed * personalTime * Mathf.PI / 180));
        //    GetComponent<BoxCollider>().size = new Vector3(m_PropellorMesh.transform.lossyScale.z, m_PropellorMesh.transform.lossyScale.y,
        //        m_PropellorMesh.transform.lossyScale.x - (m_PropellorMesh.transform.lossyScale.x - m_PropellorMesh.transform.lossyScale.z) * Mathf.Abs(Mathf.Cos((m_PropellorRotationSpeed * personalTime * Mathf.PI / 180))));
        transform.localScale = currentScale;
    }
}

