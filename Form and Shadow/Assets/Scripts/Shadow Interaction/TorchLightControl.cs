using UnityEngine;

public class TorchLightControl : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shadow"))
            other.gameObject.layer = LayerMask.NameToLayer("Invisible Shadow");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invisible Shadow"))
            other.gameObject.layer = LayerMask.NameToLayer("Shadow");
    }
}
