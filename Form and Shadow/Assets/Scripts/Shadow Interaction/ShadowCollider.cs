using UnityEngine;

public class ShadowCollider : MonoBehaviour {
    [HideInInspector] public Transform m_TransformParent;
    [HideInInspector] public bool m_ZAxisCast;
	
    void Start()
    {
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Shadow");
            if (child.GetComponent<MeshRenderer>())
                Destroy(child.GetComponent<MeshRenderer>());
        }
    }

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
