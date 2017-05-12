using UnityEngine;
using System.Collections;

/*
    Written by: Daniel Colina
    --Checkpoint--
    Handles basic logic for checkpoint prefabs throughout the 
    level.

*/

public class Checkpoint : MonoBehaviour {
    public GameObject checkPointFlag;
    public float flagRaiseDuration;
    public bool triggered;
    private Vector3 endPos;

    void Update()
    {
        if(!triggered)
            endPos = checkPointFlag.transform.position + new Vector3(0, 0.65f, 0);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(!triggered)
            {
                PlayerMovement.playerStartPosition = transform.position;
                triggered = true;
                StartCoroutine(RaiseFlag());
            }
        }
    }

    IEnumerator RaiseFlag()
    {
        float panStart = GameController.masterTimer;
        float personalTimer = panStart;
        Vector3 startPos = checkPointFlag.transform.position;

        while (personalTimer < panStart + flagRaiseDuration)
        {
            Vector3 currentPos;
            if (!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
            {
                personalTimer += Time.deltaTime;
            }
            currentPos = Vector3.Lerp(startPos, endPos, (personalTimer - panStart) / flagRaiseDuration);
            checkPointFlag.transform.position = currentPos;
            yield return null;
        }
    }
}
