using UnityEngine;

public class ShadowCollider : MonoBehaviour {
	private ShadowCast shadowCast;
  
    public Vector3 transformOffset;
    public Transform wallTransform;
    public Vector3 castDirection;

    [HideInInspector]
    public GameObject exceptionParent;
    [HideInInspector]
    public float errorMargin;
    [HideInInspector]
    public bool lockedInZAxis;

	void Start ()
    {
		errorMargin = 0.1f;
		gameObject.layer = LayerMask.NameToLayer("Shadow");

        // Does the object have a normal parented mesh, and is childed
        // to that normal parent?
		if (exceptionParent == null) 
		{
			shadowCast = GetComponentInParent<ShadowCast> ();

            switch (shadowCast.meshType)
            {
                case ShadowCast.MeshType.MOVE_PLATFORM:
                    gameObject.name = "Move_Platform_Shadow_Collider";
                    CreatePlatformShadowCollider();
                    break;
                case ShadowCast.MeshType.SPIKES:
                    gameObject.name = "Spikes_Shadow_Collider";
                    CreateSpikesShadowCollider();
                    break;
                case ShadowCast.MeshType.BASIC_CUBE:
                    gameObject.name = "Basic_Cube_Shadow_Collider";
                    CreateBasicCollider();
                    break;
                default:
                    Debug.Log("You didn't assign a meshtype on my parent!");
                    break;
            }
		}

		else
		{
			shadowCast = exceptionParent.GetComponent<ShadowCast> ();
            switch (shadowCast.meshType)
            {
                case ShadowCast.MeshType.PROPELLOR_PLATFORM:
                    gameObject.name = "Propellor_Platform_Shadow_Collider_Zone";
                    CreatePropellorShadowCollider();
                    break;
                case ShadowCast.MeshType.ENEMY_TOAD:
                    gameObject.name = "Enemy_Toad_Shadow_Collider";
                    CreateEnemyToadCollider();
                    break;
                default:
                    Debug.Log("You didn't assign a meshtype on my exception parent!");
                    break;
            }
        }
        GetTransformOffset();
        GetLockedAxis();

        if(shadowCast.meshType == ShadowCast.MeshType.PROPELLOR_PLATFORM)
            SetPropellorOffset();
	}
	
	void FixedUpdate () 
	{
        LockMovementAxis();

        if (exceptionParent != null)
        {
            FollowExceptionParent();
        }
	}

    public void GetTransformOffset()
    {
        if (LightSourceControl.zAxisMovement)
        {
            if(shadowCast.meshException)
                transformOffset = ((shadowCast.GetComponent<MeshCollider>().bounds.size.z / 2) * castDirection);
            else
                transformOffset = ((shadowCast.transform.lossyScale.z / 2) * castDirection);

        }
        else if (LightSourceControl.xAxisMovement)
        {
            if (shadowCast.meshException)
                transformOffset = ((shadowCast.GetComponent<MeshCollider>().bounds.size.x / 2) * castDirection);
            else
                transformOffset = ((shadowCast.transform.lossyScale.x / 2) * castDirection);
        }
    }

    public void GetLockedAxis()
    {
        if (transformOffset.z > 0 + errorMargin || transformOffset.z < 0 - errorMargin)
        {
            lockedInZAxis = true;
        }
        else
        {
            lockedInZAxis = false;
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

    public void SetPropellorOffset()
    {
        if (lockedInZAxis)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, wallTransform.position.z + transformOffset.z);
        }
        else
        {
            transform.position = new Vector3(wallTransform.position.x + shadowCast.transform.lossyScale.z / 2, transform.position.y, transform.position.z);
        }
    }

    public void FollowExceptionParent()
    {
        if (lockedInZAxis)
        {
            transform.position = new Vector3(exceptionParent.transform.position.x, exceptionParent.transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, exceptionParent.transform.position.y, exceptionParent.transform.position.z);
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
		platformColliderTriggerZone.transform.position = transform.position;
		platformColliderTriggerZone.transform.rotation = transform.rotation;
		platformColliderTriggerZone.transform.parent = gameObject.transform;
		platformColliderTriggerZone.AddComponent<BoxCollider>();
		platformColliderTriggerZone.GetComponent<BoxCollider>().isTrigger = true;
		platformColliderTriggerZone.AddComponent<MovingPlatformShadowCollider>();
		platformColliderTriggerZone.GetComponent<BoxCollider>().size = gameObject.transform.parent.transform.parent.GetComponent<BoxCollider>().size;
		platformColliderTriggerZone.GetComponent<BoxCollider>().center = gameObject.transform.parent.transform.parent.GetComponent<BoxCollider>().center;
	}

	public void CreatePropellorShadowCollider()
	{
		gameObject.AddComponent<TransformLock>();
		gameObject.AddComponent<BoxCollider>();
		gameObject.GetComponent<BoxCollider>().isTrigger = true;

        if (lockedInZAxis)
        {
            gameObject.transform.localScale = new Vector3(exceptionParent.transform.lossyScale.z, exceptionParent.transform.lossyScale.y, exceptionParent.transform.lossyScale.x);
        }
        else
        {
            gameObject.transform.localScale = exceptionParent.transform.lossyScale;
        }

        GameObject propellorShadowCollider = new GameObject("Propellor_Platform_Shadow_Collider");
		propellorShadowCollider.transform.position = transform.position;
        propellorShadowCollider.transform.parent = gameObject.transform;
        		
		propellorShadowCollider.AddComponent<BoxCollider>();
		propellorShadowCollider.AddComponent<PropellorPlatformShadowCollider>();
		propellorShadowCollider.GetComponent<PropellorPlatformShadowCollider> ().propellor = exceptionParent.transform.parent.gameObject;
    }

    public void CreateEnemyToadCollider()
    {
        gameObject.AddComponent<BoxCollider>();
        gameObject.AddComponent<Killzone>();
        gameObject.GetComponent<BoxCollider>().size = exceptionParent.GetComponent<MeshCollider>().bounds.size;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;

    }

    public void CreateSpikesShadowCollider()
	{
		// Creates a spike deathzone the player can fall into
		gameObject.AddComponent<BoxCollider>();
		gameObject.AddComponent<Killzone>();
		gameObject.GetComponent<BoxCollider>().size = gameObject.transform.parent.GetComponent<BoxCollider>().size;
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
	}

	public void CreateBasicCollider()
	{
		gameObject.AddComponent<BoxCollider>();
	}
}
