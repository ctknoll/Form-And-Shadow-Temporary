using UnityEngine;

public class ShadowCollider : MonoBehaviour {
	private ShadowCast shadowCast;
	private float errorMargin;

	// Use this for initialization
	void Start () 
	{
		errorMargin = 0.1f;
		shadowCast = GetComponentInParent<ShadowCast>();
		gameObject.layer = LayerMask.NameToLayer("Shadow Collider");

		if(gameObject.transform.parent.gameObject.tag == "Move Platform")
		{
			gameObject.name = "Platform Collider";
			CreatePlatformShadowCollider();
		}
		else if(gameObject.transform.parent.gameObject.tag == "Spikes")
		{
			gameObject.name = "Spike Collider";
			CreateSpikesShadowCollider();
		}
		else
		{
			gameObject.name = "Basic Collider";
			CreateBasicCollider();
		}
		Vector3 pos = transform.position + shadowCast.transformOffset;
        transform.position = pos;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		LockMovementAxis();
	}

	public void LockMovementAxis()
	{
		if(shadowCast.transformOffset.x > 0 + errorMargin || shadowCast.transformOffset.x < 0 - errorMargin)
		{
			transform.position = new Vector3(gameObject.transform.parent.GetComponent<ShadowCast>().wallTransform.position.x + shadowCast.transformOffset.x, transform.position.y, transform.position.z);
		}
		if(shadowCast.transformOffset.y > 0 + errorMargin || shadowCast.transformOffset.y < 0 - errorMargin)
		{
			transform.position = new Vector3(transform.position.x, gameObject.transform.parent.GetComponent<ShadowCast>().wallTransform.position.y + shadowCast.transformOffset.y, transform.position.z);
		}
		if(shadowCast.transformOffset.z > 0 + errorMargin || shadowCast.transformOffset.z < 0 - errorMargin)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, gameObject.transform.parent.GetComponent<ShadowCast>().wallTransform.position.z + shadowCast.transformOffset.z);
		}
	}

	public void CreatePlatformShadowCollider()
	{
		// First, add a box collider to the base shadow-casting object that mimics the size and scale of its
		// parent, the shadow-casting object
		gameObject.AddComponent<BoxCollider>();

		// Then, proceed with an unnecessarily complicated method to add a moving trigger zone on top of the
		// platform that childs the player shadow to it when within it so the player follows the platform
		GameObject platformCollider = new GameObject("PlatformShadowCollider");
		platformCollider.transform.position = transform.position;
		platformCollider.transform.rotation = transform.rotation;
		platformCollider.transform.parent = gameObject.transform;
		platformCollider.AddComponent<BoxCollider>();
		platformCollider.GetComponent<BoxCollider>().isTrigger = true;
		platformCollider.AddComponent<MovingShadowPlatform>();
		platformCollider.GetComponent<BoxCollider>().size = gameObject.transform.parent.transform.parent.GetComponent<BoxCollider>().size;
		platformCollider.GetComponent<BoxCollider>().center = gameObject.transform.parent.transform.parent.GetComponent<BoxCollider>().center;
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
