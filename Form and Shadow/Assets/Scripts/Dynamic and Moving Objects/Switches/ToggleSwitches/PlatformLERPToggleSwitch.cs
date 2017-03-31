using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLERPToggleSwitch : ToggleSwitch
{
	public lerpPlatform[] platforms;

	private List<Vector3> startPos;
	private List<Vector3> endPos;
	private List<Vector3> currentPos;
	private List<IEnumerator> moveTowards;
	private List<IEnumerator> moveReturn;
	private List<float> moveTime;
	private bool locked;
	private bool platformAnimating;
	private float clearBuffer;

    [System.Serializable]
    public class lerpPlatform
    {
        public GameObject platformObject;
        public Vector3 directionToMove;
        public float moveSpeed;
    };

	// Use this for initialization
	new void Start()
	{
		base.Start();
		locked = false;
		moveTime = new List<float>();
		startPos = new List<Vector3>();
		endPos = new List<Vector3>();
		currentPos = new List<Vector3>();
		moveTowards = new List<IEnumerator>();
		moveReturn = new List<IEnumerator>();
		clearBuffer = 0;
	}

    // Update is called once per frame
	void Update()
	{
		if (pressed && !locked && !platformAnimating)
		{
			locked = true;
			
			int i = 0;
			currentPos.Clear();
			foreach (lerpPlatform platform in platforms)
			{
				if(startPos.Count < platforms.Length) startPos.Add(platform.platformObject.transform.position);
				if(endPos.Count < platforms.Length) endPos.Add(platform.platformObject.transform.position + platform.directionToMove);
				moveTime.Add((platform.directionToMove.magnitude / platform.moveSpeed));
				IEnumerator ienum = MoveOut(platform, i);
				moveTowards.Add(ienum);
				StartCoroutine(ienum);
				i++;
			}
		} 
		else if (!pressed && locked && !platformAnimating) 
		{
			locked = false;
			platformAnimating = true;
			int i = 0;
			currentPos.Clear();
			foreach (lerpPlatform platform in platforms)
			{
				IEnumerator ienum = MoveBack(platform, i);
				moveReturn.Add(ienum);
				StartCoroutine(ienum);
				i++;
			}
            StartCoroutine(Clear());
        }
	}

	public IEnumerator MoveOut(lerpPlatform platform, int index)
	{
		float panStart = Time.time;
		float localTime = Time.time;
		currentPos.Add(platform.platformObject.transform.position);
		moveTime[index] = ((currentPos[index] - endPos[index]).magnitude / platform.moveSpeed);
		while ((((!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused) ? localTime += Time.deltaTime : localTime) < (panStart + moveTime[index]) + Time.time - localTime) && pressed)
		{
			platform.platformObject.transform.position = Vector3.Lerp(currentPos[index], endPos[index], (localTime - panStart) / moveTime[index]);
			yield return null;
		}
		platformAnimating = false;
	}

	//((!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) ? localTime += Time.deltaTime : localTime)
	public IEnumerator MoveBack(lerpPlatform platform, int index)
	{
        float panStart = Time.time;
		float personalTime = panStart;
		currentPos.Add(platform.platformObject.transform.position);
		moveTime[index] = ((currentPos[index] - startPos[index]).magnitude / platform.moveSpeed);
		while ((personalTime - panStart) < moveTime[index] && !pressed)
		{
			if ((!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)) 
			{
				personalTime += Time.deltaTime;
			} 
			else 
			{
				clearBuffer += Time.deltaTime / platforms.Length;
			}
			platform.platformObject.transform.position = Vector3.Lerp(currentPos[index], startPos[index], (personalTime - panStart) / moveTime[index]);
			yield return null;
		}
	}

    public IEnumerator Clear()
    {
        float max = 0;
        foreach(float f in moveTime)
        {
            if (f > max)
                max = f;
        }
        yield return new WaitForSeconds(max);
		while (clearBuffer > 0) 
		{
			clearBuffer -= Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
        moveTime.Clear();
        moveTowards.Clear();
        moveReturn.Clear();
        platformAnimating = false;
		clearBuffer = 0;
    }
}
