using UnityEngine;

public class Checkpoint : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerMovement.playerStartPosition = transform.position;
        }
    }
}
