using UnityEngine;

public class ShadowCollider : MonoBehaviour {
	private ShadowCast shadowCast;

	// Use this for initialization
	void Start () 
	{
		shadowCast = GetComponentInParent<ShadowCast>();

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
			gameObject.AddComponent<BoxCollider>();
		}
		Vector3 pos = transform.position + shadowCast.zOffset;
		transform.position = pos;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		
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
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
	}
}
