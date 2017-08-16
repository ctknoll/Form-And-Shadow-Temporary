using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PulleyMovingPlatform : MovingPlatform
{
    new void Start()
    {
        base.Start();
    }

    new void Update () 
	{
        base.Update();
        if (m_IsFinished)
        {
            StartCoroutine(DestroyPlatform());
        }
    }

	public IEnumerator DestroyPlatform()
	{
		yield return new WaitForSeconds(0.5f);
        if(GetComponentInChildren<MovingPlatformTriggerZone>().m_PlayerChilded)
		    GameObject.Find("Player").transform.parent = null;

        List<GameObject> platformShadowColliders = new List<GameObject>();
        if(GetComponentInChildren<ShadowCast>().m_ShadowColliders.m_NorthShadowCollider)
            platformShadowColliders.Add(GetComponentInChildren<ShadowCast>().m_ShadowColliders.m_NorthShadowCollider);
        if (GetComponentInChildren<ShadowCast>().m_ShadowColliders.m_EastShadowCollider)
            platformShadowColliders.Add(GetComponentInChildren<ShadowCast>().m_ShadowColliders.m_EastShadowCollider);
        if (GetComponentInChildren<ShadowCast>().m_ShadowColliders.m_SouthShadowCollider)
            platformShadowColliders.Add(GetComponentInChildren<ShadowCast>().m_ShadowColliders.m_SouthShadowCollider);
        if (GetComponentInChildren<ShadowCast>().m_ShadowColliders.m_WestShadowCollider)
            platformShadowColliders.Add(GetComponentInChildren<ShadowCast>().m_ShadowColliders.m_WestShadowCollider);

        foreach (GameObject pulleyShadowCollider in platformShadowColliders)
        {
            if (pulleyShadowCollider.GetComponentInChildren<MovingPlatformTriggerZone>().m_PlayerChilded)
            {
                GameObject.Find("Player_Shadow").transform.parent = null;
            }
            Destroy(pulleyShadowCollider);
        }
        Destroy(gameObject);
	}
}