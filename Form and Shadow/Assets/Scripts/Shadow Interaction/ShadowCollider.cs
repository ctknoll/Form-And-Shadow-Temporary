using UnityEngine;

public class ShadowCollider : MonoBehaviour {
	private ShadowCast shadowCast;

	// Use this for initialization
	void Start () 
	{
		shadowCast = GetComponentInParent<ShadowCast>();
		gameObject.AddComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		Vector3 pos = transform.position;
		pos.z = shadowCast.zOffset - 0.1f;
		transform.position = pos;
	}
}
