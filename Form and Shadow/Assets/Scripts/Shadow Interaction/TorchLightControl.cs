﻿using UnityEngine;

public class TorchLightControl : MonoBehaviour {
    public GameObject shadowMeldObjectVFXPrefab;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shadow") && other.gameObject.tag != "Player")
        {
            other.gameObject.layer = LayerMask.NameToLayer("Invisible Shadow");
            // Handle the logic of moving the 3D object to the shadowmeld ignore layer
            if (other.GetComponent<ShadowCollider>().exceptionParent)
            {
                other.GetComponent<ShadowCollider>().exceptionParent.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
                Instantiate(shadowMeldObjectVFXPrefab, other.GetComponent<ShadowCollider>().exceptionParent.transform);
            }
            else
            {
                other.transform.parent.gameObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
                Instantiate(shadowMeldObjectVFXPrefab, other.transform.parent);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invisible Shadow"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Shadow");
            // Handle the logic of moving the 3D object back to the original layer
            if (other.GetComponent<ShadowCollider>().exceptionParent)
            {
                other.GetComponent<ShadowCollider>().exceptionParent.layer = LayerMask.NameToLayer("Form");
                Destroy(other.GetComponent<ShadowCollider>().exceptionParent.transform.FindChild("ShadowmeldObjectVFX(Clone)").gameObject);
            }
            else
            {
                other.transform.parent.gameObject.layer = LayerMask.NameToLayer("Form");
                Destroy(other.transform.parent.FindChild("ShadowmeldObjectVFX(Clone)").gameObject);
            }
        }
    }
}
