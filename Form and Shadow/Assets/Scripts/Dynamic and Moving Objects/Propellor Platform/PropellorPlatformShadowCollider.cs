using UnityEngine;

public class PropellorPlatformShadowCollider : MonoBehaviour
{
    [HideInInspector] public GameObject m_PropellorMesh;

    float personalTime;
    GameObject propellorObject;
    ShadowCollider shadowColliderMaster;

	void Start ()
	{
        personalTime = 0;
        shadowColliderMaster = GetComponentInParent<ShadowCollider>();
        propellorObject = m_PropellorMesh.transform.parent.gameObject;
    }

    void Update()
    {
        if (PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PLAYERSTATE.SHIFTING && !GameController.resetting && !GameController.paused)
        {
            personalTime += Time.deltaTime;
        }
        if (shadowColliderMaster.m_ZAxisCast)
        {
            GetComponent<BoxCollider>().size = new Vector3(m_PropellorMesh.transform.lossyScale.z - (m_PropellorMesh.transform.lossyScale.z - m_PropellorMesh.transform.lossyScale.x) * Mathf.Abs(Mathf.Cos((propellorObject.GetComponent<PropellorPlatform>().rotationSpeed * personalTime * Mathf.PI / 180))),
                m_PropellorMesh.transform.lossyScale.y, m_PropellorMesh.transform.lossyScale.z);
        }
        else
        {
            GetComponent<BoxCollider>().size = new Vector3(m_PropellorMesh.transform.lossyScale.z, m_PropellorMesh.transform.lossyScale.y,
                m_PropellorMesh.transform.lossyScale.x - (m_PropellorMesh.transform.lossyScale.x - m_PropellorMesh.transform.lossyScale.z) * Mathf.Abs(Mathf.Cos((propellorObject.GetComponent<PropellorPlatform>().rotationSpeed * personalTime * Mathf.PI / 180))));
        }
    }
}

