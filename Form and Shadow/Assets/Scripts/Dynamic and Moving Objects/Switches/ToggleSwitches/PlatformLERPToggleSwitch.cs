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
	private bool animating;

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
	}

    // Update is called once per frame
	new void Update()
	{
		if (pressed && !locked && !animating)
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
		else if (!pressed && locked && !animating) 
		{
			locked = false;
			animating = true;
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

		base.Update();
	}

	public IEnumerator MoveOut(lerpPlatform platform, int index)
	{
		float panStart = Time.time;
		float localTime = Time.time;
		currentPos.Add(platform.platformObject.transform.position);
		moveTime[index] = ((currentPos[index] - endPos[index]).magnitude / platform.moveSpeed);
		while ((((!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) ? localTime += Time.deltaTime : localTime) < (panStart + moveTime[index]) + Time.time - localTime) && pressed)
		{
			platform.platformObject.transform.position = Vector3.Lerp(currentPos[index], endPos[index], (localTime - panStart) / moveTime[index]);
			yield return null;
		}
		animating = false;
	}

	//((!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) ? localTime += Time.deltaTime : localTime)
	public IEnumerator MoveBack(lerpPlatform platform, int index)
	{
        float panStart = Time.time;
		float localTime = Time.time;
		currentPos.Add(platform.platformObject.transform.position);
		moveTime[index] = ((currentPos[index] - startPos[index]).magnitude / platform.moveSpeed);
		while (((!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) ? localTime += Time.deltaTime : localTime) < (panStart + moveTime[index] + (Time.time - localTime)) && !pressed && index < moveReturn.Count)
		{
			platform.platformObject.transform.position = Vector3.Lerp(currentPos[index], startPos[index], (localTime - panStart) / moveTime[index]);
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
        moveTime.Clear();
        moveTowards.Clear();
        moveReturn.Clear();
        animating = false;
    }
}
