using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowWallEdge : MonoBehaviour {
    private float castInternalCooldown = 0.5f;
    private float castInternalCooldownStart;
    private List<GameObject> platformList = new List<GameObject>();

    public void OnTriggerStay(Collider other)
    {
        if(GetComponentInParent<ShadowWall>().isLit)
        {
            if(!other.gameObject.isStatic && !other.isTrigger)
            {
                if (GameController.masterTimer > castInternalCooldownStart + castInternalCooldown)
                {
                    Debug.Log("casting");
                    castInternalCooldownStart = GameController.masterTimer;
                    other.gameObject.GetComponentInParent<ShadowCollider>().transformParent.GetComponentInParent<ShadowCast>().CastShadowCollider(
                        (other.gameObject.transform.position - other.gameObject.GetComponentInParent<ShadowCollider>().transformParent.transform.position).normalized);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {

    }
    //public void OnTriggerEnter(Collider other)
    //{
    //    if(GetComponentInParent<ShadowWall>().isLit)
    //    {
    //        if (!other.gameObject.isStatic && !other.isTrigger)
    //        {
    //            Debug.Log(other.gameObject + " is entering");
    //            StartCoroutine(RecastShadow(other.gameObject.GetComponent<ShadowCollider>().transformParent.GetComponent<ShadowCast>(), other.gameObject, GetComponent<Collider>().ClosestPointOnBounds(other.transform.position)));
    //        }
    //    }
    //}

    //public void OnTriggerExit(Collider other)
    //{
    //    if (GetComponentInParent<ShadowWall>().isLit)
    //    {
    //        if (!other.gameObject.isStatic && !other.isTrigger && other.gameObject.tag != "Player")
    //        {
    //            Debug.Log(other.gameObject + " is exiting");
    //            if (other.gameObject.GetComponentInChildren<MovingPlatformShadowCollider>().playerChildedIn2D)
    //                GameObject.Find("Player_Shadow").transform.parent = null;
    //            other.gameObject.GetComponent<ShadowCollider>().transformParent.GetComponent<ShadowCast>().shadowColliders.Remove(other.gameObject);
    //            Destroy(other.gameObject);
    //        }
    //    }
    //}


	// Use this for initialization
	void Start ()
    {
        castInternalCooldownStart = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator RecastShadow(ShadowCast parentShadowCast, GameObject triggeringShadowCollider, Vector3 castLocation)
    {
        yield return new WaitForSeconds(0.05f);
        Debug.Log("recasting");
        //shadowCastObject.GetComponent<ShadowCast>().CastShadow(GameObject.Find("Lighting_Reference").transform.right);
        //shadowCastObject.GetComponent<ShadowCast>().CastShadow(-GameObject.Find("Lighting_Reference").transform.right);
        //shadowCastObject.GetComponent<ShadowCast>().CastShadow(GameObject.Find("Lighting_Reference").transform.forward);
        //parentShadowCast.GetComponent<ShadowCast>().CastShadowFromPoint(-GameObject.Find("Lighting_Reference").transform.forward, castLocation);
    }
}
