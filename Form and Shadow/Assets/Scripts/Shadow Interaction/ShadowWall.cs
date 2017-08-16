using UnityEngine;

public class ShadowWall : MonoBehaviour
{
    public bool isLit;
	
	void Update ()
    {
        bool lighting = false;
        foreach (Transform child in GameObject.Find("Lighting_Master_Control").GetComponentInChildren<Transform>())
        {
            if (!child.gameObject.activeSelf) continue;
            Vector3 lightDirection = child.forward.normalized;
            Vector3 Normal = -1 * transform.forward.normalized;

            if (Vector3.Dot(lightDirection, Normal) < 0)
                lighting = true;
        }
        isLit = lighting;
    }
}
