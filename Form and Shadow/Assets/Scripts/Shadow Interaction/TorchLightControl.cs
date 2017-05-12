using UnityEngine;

public class TorchLightControl : MonoBehaviour {
    public GameObject shadowMeldObjectVFXPrefab;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shadow") && other.gameObject.tag != "Player")
        {
            other.gameObject.layer = LayerMask.NameToLayer("Invisible Shadow");
            // Handle the logic of moving the 3D object to the shadowmeld ignore layer
            if (other.GetComponent<ShadowCollider>().transformParent)
            {
                other.GetComponent<ShadowCollider>().transformParent.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
            }
            else
            {
                other.transform.parent.gameObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invisible Shadow"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Shadow");
            // Handle the logic of moving the 3D object back to the original layer
            if (other.GetComponent<ShadowCollider>().transformParent)
            {
                other.GetComponent<ShadowCollider>().transformParent.layer = LayerMask.NameToLayer("Form");
            }
            else
            {
                other.transform.parent.gameObject.layer = LayerMask.NameToLayer("Form");
            }
        }
    }
}
