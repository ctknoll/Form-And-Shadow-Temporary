using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowColliderCast : MonoBehaviour 
{
	public GameObject shadowCollider;
	// Update is called once per frame
	void Update () 
	{
		RaycastHit hit;
		if(!shadowCollider)
		{
			if(Physics.Raycast(transform.position, LightSourceControl.lightSourceDirection, out hit, 20f))
			{
				if(hit.collider.gameObject.tag == "Shadow Wall")
				{
					shadowCollider = new GameObject();
					shadowCollider.transform.position = hit.point;
					shadowCollider.transform.localScale = gameObject.transform.lossyScale;
					shadowCollider.AddComponent<EdgeCollider2D>();
				}
			}
		}
	}
}
