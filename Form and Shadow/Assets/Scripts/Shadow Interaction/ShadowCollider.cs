using UnityEngine;

public class ShadowCollider : MonoBehaviour {
	private ShadowCast shadowCast;

	// Use this for initialization
	void Start () 
	{
		shadowCast = GetComponentInParent<ShadowCast>();
		gameObject.AddComponent<BoxCollider>();
        Vector3 pos = transform.position + shadowCast.zOffset;
        transform.position = pos;
    }
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		
	}
}
