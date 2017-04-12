using UnityEngine;

public class PropellorPlatformShadowCollider : MonoBehaviour
{
	public GameObject propellor;
    public float personalTime;
	private GameObject propellorMesh;
    private ShadowCollider shadowColliderMaster;

	void Start ()
	{
		propellorMesh = propellor.transform.GetChild(0).gameObject;
        personalTime = 0;
        shadowColliderMaster = GetComponentInParent<ShadowCollider>();
    }
	
	void FixedUpdate ()
	{
		if(!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.resetting && !GameController.paused)
		{
            personalTime += Time.deltaTime;
		}
        if (shadowColliderMaster.lockedInZAxis)
        {
            GetComponent<BoxCollider>().size = new Vector3(propellorMesh.transform.lossyScale.z - (propellorMesh.transform.lossyScale.z - propellorMesh.transform.lossyScale.x) * Mathf.Abs(Mathf.Cos((propellor.GetComponent<PropellorPlatform>().rotationSpeed * personalTime * Mathf.PI / 180))),
                propellorMesh.transform.lossyScale.y, propellorMesh.transform.lossyScale.z);
        }
        else
        {
            GetComponent<BoxCollider>().size = new Vector3(propellorMesh.transform.lossyScale.x, propellorMesh.transform.lossyScale.y,
                propellorMesh.transform.lossyScale.x - (propellorMesh.transform.lossyScale.x - propellorMesh.transform.lossyScale.z) * Mathf.Abs(Mathf.Cos((propellor.GetComponent<PropellorPlatform>().rotationSpeed * personalTime * Mathf.PI / 180))));
        }
    }
}

