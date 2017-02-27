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
	private bool moving;

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
		if (pressed && !locked && !moving)
		{
			locked = true;
			moving = true;
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
		else if (!pressed && locked && !moving) 
		{
			locked = false;
			moving = true;
			int i = 0;
			currentPos.Clear();
			foreach (lerpPlatform platform in platforms)
			{
				IEnumerator ienum = MoveBack(platform, i);
				moveReturn.Add(ienum);
				StartCoroutine(ienum);
				i++;
			}
		}

		base.Update();
	}

	public IEnumerator MoveOut(lerpPlatform platform, int index)
	{
		float panStart = Time.time;
		currentPos.Add(platform.platformObject.transform.position);
		moveTime[index] = ((currentPos[index] - endPos[index]).magnitude / platform.moveSpeed);
		while ((Time.time < panStart + moveTime[index]) && pressed)
		{
			platform.platformObject.transform.position = Vector3.Lerp(currentPos[index], endPos[index], (Time.time - panStart) / moveTime[index]);
			yield return null;
		}
		moving = false;
	}

	public IEnumerator MoveBack(lerpPlatform platform, int index)
	{
		
		float panStart = Time.time;
		currentPos.Add(platform.platformObject.transform.position);
		moveTime[index] = ((currentPos[index] - startPos[index]).magnitude / platform.moveSpeed);
		while (Time.time < panStart + moveTime[index] && !pressed)
		{
			platform.platformObject.transform.position = Vector3.Lerp(currentPos[index], startPos[index], (Time.time - panStart) / moveTime[index]);
			yield return null;
		}
		moveTime.Clear();
		moveTowards.Clear();
		moveReturn.Clear();
		moving = false;
	}
}
