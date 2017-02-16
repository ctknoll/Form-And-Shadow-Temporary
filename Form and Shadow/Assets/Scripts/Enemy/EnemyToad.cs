using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyToad : MonoBehaviour {
    public List<GameObject> jumpObjects;
    public float jumpDuration;
    public float jumpCooldown;

    private bool jumping;
    private int currentJumpLocationIndex;
    private float jumpStart;
    private float personalTimer;

	// Use this for initialization
	void Start ()
    {
        transform.position = GetRelativePosition(jumpObjects[0]);
        currentJumpLocationIndex = 0;
        jumpStart = 0;
        personalTimer = 0;
        jumping = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!jumping)
        {
            personalTimer += Time.deltaTime;
            if (personalTimer >= jumpStart + jumpCooldown)
            {
                jumpStart = personalTimer;
                StartCoroutine(JumpToNextPlatform());
            }
        }
	}

    public Vector3 GetRelativePosition(GameObject targetObj)
    {
        Vector3 relativePosition = targetObj.transform.position + new Vector3(0, targetObj.transform.lossyScale.y / 2 + transform.lossyScale.y, 0);

        return relativePosition;
    }

    public IEnumerator JumpToNextPlatform()
    {
        jumping = true;
        float panStart = Time.time;
        Vector3 startPos = transform.position;

        if (currentJumpLocationIndex + 1 < jumpObjects.Count)
        {
            currentJumpLocationIndex += 1;
        }
        else
        {
            currentJumpLocationIndex = 0;
        }
        
        transform.rotation = Quaternion.LookRotation(transform.position - new Vector3(jumpObjects[currentJumpLocationIndex].transform.position.x, 
            transform.position.y, jumpObjects[currentJumpLocationIndex].transform.position.z), Vector3.up);

        while (Time.time < panStart + jumpDuration)
        {
            transform.position = Vector3.Lerp(startPos, GetRelativePosition(jumpObjects[currentJumpLocationIndex]), (Time.time - panStart) / jumpDuration);
            yield return null;
        }
        jumping = false;
        SpawnAcidPool();
    }

    public void SpawnAcidPool()
    {
        Debug.Log("Will spawn acid pool now, WIP");
    }
}
