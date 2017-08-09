using UnityEngine;

public class ShadowCollider : MonoBehaviour {
    [HideInInspector] public Transform m_TransformParent;
    [HideInInspector] public bool m_ZAxisCast;
	
	void Update () 
	{
        FollowTransformParent();
	}

    public void FollowTransformParent()
    {
        if (m_ZAxisCast)
        {
            transform.position = new Vector3(m_TransformParent.position.x, m_TransformParent.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, m_TransformParent.position.y, m_TransformParent.position.z);
        }
    }
}
