using UnityEngine;

public class PushCubeMoveSpace : MonoBehaviour
{
    private PushCube pushCube;

    void Start()
    {
        pushCube = GetComponentInParent<PushCube>();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameController.CheckInteractToolip(false, true);
            pushCube.canInteract = false;
            pushCube.grabbed = false;
            pushCube.transform.parent = null;
            PlayerMovement.isGrabbing = false;
            PlayerMovement.grabbedObject = null;
        }
    }
}
