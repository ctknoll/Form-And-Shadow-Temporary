using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyToad : MonoBehaviour {
    public List<GameObject> jumpObjects;
    public float jumpDuration;
    public float jumpCooldown;
    public GameObject acidPoolPrefab;

    private bool jumping;
    private int currentJumpLocationIndex;
    private float jumpStart;
    private float personalTimer;

	void Start ()
    {
        transform.position = GetRelativeJumpPosition(jumpObjects[0]);
        currentJumpLocationIndex = 0;
        jumpStart = 0;
        personalTimer = 0;
        jumping = false;
	}
	
	void Update ()
    {
        if(!jumping && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)
        {
            personalTimer += Time.deltaTime;
            if (personalTimer >= jumpStart + jumpCooldown)
            {
                jumpStart = personalTimer;
                StartCoroutine(JumpToNextPlatform());
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = gameObject.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = null;
        }
    }

    public Vector3 GetRelativeJumpPosition(GameObject targetObj)
    {
        Vector3 relativePosition = targetObj.transform.position + new Vector3(0, targetObj.transform.GetChild(0).transform.lossyScale.y / 2 + transform.lossyScale.y, 0);

        return relativePosition;
    }

    public Vector3 GetRelativeAcidPoolPosition(GameObject targetObj)
    {
        Vector3 relativePosition = targetObj.transform.position + new Vector3(0, targetObj.transform.GetChild(0).transform.lossyScale.y / 2, 0);

        return relativePosition;
    }

    public IEnumerator JumpToNextPlatform()
	{
		jumping = true;
		float panStart = Time.time;
		float jumpPersonalTimer = panStart;
		Vector3 startPos = transform.position;
		float sinStart = 0;
		float sinEnd = 1.5f * Mathf.PI;
		bool isHigher = false;
		bool isSame = false;

		//target platform change
		currentJumpLocationIndex = ++currentJumpLocationIndex % jumpObjects.Count;

		Debug.Log(GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).y);
        Debug.Log(startPos.y);

		//same height
		if ((GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).y > startPos.y - .5f) && (GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).y < startPos.y + .5f)) 
		{
			sinEnd -= .5f * Mathf.PI;
			isSame = true;
		} 
		//platform is higher
		else if (GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).y > startPos.y + .5f) 
		{
            sinStart -= .5f * Mathf.PI;
            sinEnd -= .5f * Mathf.PI;
            isHigher = true;
		} 
		//platform is lower

        float heightDifference = Mathf.Abs(GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).y - startPos.y);
		float actualHeightDifference = heightDifference;
		heightDifference = Mathf.Max((.5f / Mathf.PI) * Mathf.Sqrt (Mathf.Pow(GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).x - startPos.x, 2) + Mathf.Pow(GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).z - startPos.z, 2)), heightDifference);

		if (isSame)
			heightDifference = (1f / Mathf.PI) * Mathf.Sqrt (Mathf.Pow(GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).x - startPos.x, 2) + Mathf.Pow(GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).z - startPos.z, 2));

		//rotate to platform
        transform.rotation = Quaternion.LookRotation(transform.position - new Vector3(jumpObjects[currentJumpLocationIndex].transform.position.x, 
            transform.position.y, jumpObjects[currentJumpLocationIndex].transform.position.z), Vector3.up);

        while (jumpPersonalTimer < panStart + jumpDuration)
        {
            Vector3 currentPos;
            if(!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)
            {
                jumpPersonalTimer += Time.deltaTime;
            }
			currentPos.x = startPos.x + ((jumpPersonalTimer - panStart) / jumpDuration) * (GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).x - startPos.x);
			currentPos.z = startPos.z + ((jumpPersonalTimer - panStart) / jumpDuration) * (GetRelativeJumpPosition(jumpObjects[currentJumpLocationIndex]).z - startPos.z);
			float currentYSin = Mathf.Sin((sinStart + (sinEnd - sinStart) * ((jumpPersonalTimer - panStart) / jumpDuration)));
			if (!isHigher) 
			{
				currentPos.y = startPos.y + currentYSin * heightDifference;
			} 
			else 
			{
				currentPos.y = startPos.y + (1 + currentYSin) * heightDifference;
			}

			transform.position = new Vector3(currentPos.x, currentPos.y, currentPos.z);
            yield return null;
        }
        jumping = false;
        //SpawnAcidPool();
    }

    public void SpawnAcidPool()
    {
        Instantiate(acidPoolPrefab, GetRelativeAcidPoolPosition(jumpObjects[currentJumpLocationIndex]), Quaternion.identity); 
    }
}
