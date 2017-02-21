using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLERPToggleSwitch : ToggleSwitch
{

    public GameObject[] platforms;
	private GameObject platObj;
	private Vector3 startPos;
	private Vector3 endPos;
    public float moveSpeed;
	public Vector3 directionToMove;
    private bool locked;
	public float moveTime;
	private bool atPeak;
	private bool inMotion;

    // Use this for initialization
    void Start()
    {
        base.Start();
		moveTime = (directionToMove.magnitude / moveSpeed);
		inMotion = false;
    }

    // Update is called once per frame
    public void Update()
    {
		if (pressed && !locked && !inMotion) {
			foreach (GameObject platform in platforms)
			{
				inMotion = true;
				startPos = platform.transform.position;
				endPos = startPos + directionToMove;
				StartCoroutine(MoveOut(platform));
				inMotion = false;
				atPeak = true;
				if (timerDuration != -1) 
				{
					StartCoroutine(MoveBack(platform));
				}
			}
			locked = true;
		} 
		else if (!pressed && atPeak) 
		{
			locked = false;
			atPeak = false;
			if (timerDuration == -1) 
			{
				foreach (GameObject platform in platforms)
				{
					startPos = platform.transform.position - directionToMove;
					endPos = platform.transform.position;
					StartCoroutine(MoveBack(platform));
				}
			}
		}
        else if (!pressed)
        {
            locked = false;
        }

        base.Update();
    }

	public IEnumerator MoveOut(GameObject platform)
	{
		float panStart = Time.time;
		while (Time.time < panStart + moveTime)
		{
			platform.transform.position = Vector3.Lerp(startPos, endPos, (Time.time - panStart) / moveTime);
			yield return null;
		}
	}

	public IEnumerator MoveBack(GameObject platform)
	{
		if (timerDuration != -1) yield return new WaitForSeconds (timerDuration);
		atPeak = false;
		inMotion = true;
		float panStart = Time.time;
		while (Time.time < panStart + moveTime)
		{
			platform.transform.position = Vector3.Lerp(endPos, startPos, (Time.time - panStart) / moveTime);
			yield return null;
		}
		inMotion = false;
	}
}
