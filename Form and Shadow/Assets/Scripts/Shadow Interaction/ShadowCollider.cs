using UnityEngine;

public class ShadowCollider : MonoBehaviour {
	private ShadowCast shadowCast;
  
    public Vector3 transformOffset;
    public Transform wallTransform;
    public Vector3 castDirection;
    [HideInInspector]
    public GameObject transformParent;
    [HideInInspector]
    public float errorMargin;
    [HideInInspector]
    public bool lockedInZAxis;

	void Start ()
    {
		errorMargin = 0.1f;
		gameObject.layer = LayerMask.NameToLayer("Shadow");

        shadowCast = transformParent.GetComponent<ShadowCast>();
        transform.localScale = transformParent.transform.lossyScale;

        if(shadowCast.shadowType != ShadowCast.ShadowType.PROPELLOR_PLATFORM)
            transform.rotation = transformParent.transform.rotation;

        switch (shadowCast.shadowType)
        {
            case ShadowCast.ShadowType.MOVE_PLATFORM:
                gameObject.name = "Move_Platform_Shadow_Collider";
                CreatePlatformShadowCollider();
                break;
            case ShadowCast.ShadowType.SPIKES:
                gameObject.name = "Spikes_Shadow_Collider";
                CreateSpikesShadowCollider();
                break;
            case ShadowCast.ShadowType.BASIC_SHADOW:
                gameObject.name = "Basic_Cube_Shadow_Collider";
                CreateBasicShadowCollider();
                break;
            case ShadowCast.ShadowType.PROPELLOR_PLATFORM:
                gameObject.name = "Propellor_Platform_Shadow_Collider";
                CreatePropellorShadowCollider();
                break;
            case ShadowCast.ShadowType.ENEMY_TOAD:
                gameObject.name = "Enemy_Toad_Shadow_Collider";
                CreateEnemyToadShadowCollider();
                break;
            case ShadowCast.ShadowType.PUSH_CUBE:
                gameObject.name = "Push_Cube_Shadow_Collider";
                CreateBasicShadowCollider();
                break;
            default:
                Debug.Log("You didn't assign a meshtype on my exception parent!");
                break;
        }
    }
	
	void Update () 
	{
        LockMovementAxis();

        if (transformParent != null)
        {
            FollowTransformParent();
        }
	}

    public void LockMovementAxis()
	{
        if (lockedInZAxis)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, wallTransform.position.z + transformOffset.z);
        }
        else
        {
			transform.position = new Vector3(wallTransform.position.x + transformOffset.x, transform.position.y, transform.position.z);
		}
    }

    public void FollowTransformParent()
    {
        if (lockedInZAxis)
        {
            transform.position = new Vector3(transformParent.transform.position.x, transformParent.transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transformParent.transform.position.y, transformParent.transform.position.z);
        }
    }

    public void CreatePlatformShadowCollider()
	{
		// First, add a box collider to the base shadow-casting object that mimics the size and scale of its
		// parent, the shadow-casting object
		gameObject.AddComponent<BoxCollider>();


        // Then, proceed with an unnecessarily complicated method to add a moving trigger zone on top of the
        // platform that childs the player shadow to it when within it so the player follows the platform
        GameObject platformColliderTriggerZone = new GameObject("Move_Platform_Shadow_Collider_Trigger_Zone");
        platformColliderTriggerZone.layer = LayerMask.NameToLayer("Shadow");
		platformColliderTriggerZone.transform.position = transform.position;
		platformColliderTriggerZone.transform.rotation = transform.rotation;
        platformColliderTriggerZone.transform.localScale = transform.lossyScale;
		platformColliderTriggerZone.transform.parent = gameObject.transform;
		platformColliderTriggerZone.AddComponent<BoxCollider>();
		platformColliderTriggerZone.GetComponent<BoxCollider>().isTrigger = true;
        platformColliderTriggerZone.GetComponent<BoxCollider>().center = new Vector3(0, transform.lossyScale.y * 2, 0);
        platformColliderTriggerZone.AddComponent<MovingPlatformShadowCollider>();
    }

    public void CreatePropellorShadowCollider()
	{
		gameObject.AddComponent<BoxCollider>();
		gameObject.GetComponent<BoxCollider>().isTrigger = true;

        GameObject propellorShadowColliderZone = new GameObject("Propellor_Platform_Shadow_Collider_Zone");
        propellorShadowColliderZone.transform.position = transform.position;
        propellorShadowColliderZone.transform.parent = gameObject.transform;
        propellorShadowColliderZone.AddComponent<BoxCollider>();
        propellorShadowColliderZone.AddComponent<PropellorPlatformShadowCollider>();
        propellorShadowColliderZone.GetComponent<PropellorPlatformShadowCollider>().propellorMesh = transformParent.gameObject;
    }

    public void CreateEnemyToadShadowCollider()
    {
        gameObject.AddComponent<BoxCollider>();
        gameObject.AddComponent<Killzone>();
        gameObject.GetComponent<BoxCollider>().size = transformParent.GetComponent<MeshCollider>().bounds.size;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;

    }

    public void CreateSpikesShadowCollider()
	{
		gameObject.AddComponent<BoxCollider>();
        gameObject.GetComponent<BoxCollider>().size = transformParent.GetComponent<BoxCollider>().size;

        if (transformParent.GetComponent<ShadowmeldObjectControl>().shadowMeldObjectType != ShadowmeldObjectControl.ShadowMeldObjectType.FLAT_SPIKES)
        {
            gameObject.AddComponent<Killzone>();
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
	}

	public void CreateBasicShadowCollider()
	{
        gameObject.AddComponent<BoxCollider>();
    }
}
