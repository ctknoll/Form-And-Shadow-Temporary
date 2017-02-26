using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLERPPressureSwitch : PressureSwitch
{

    public GameObject[] platforms;
	private GameObject platObj;
	private List<Vector3> startPos;
	private List<Vector3> endPos;
    private List<Vector3> currentPos;
    public float moveSpeed;
	public Vector3 directionToMove;
    private bool locked;
	private float moveTime;
	private bool atPeak;
	private bool inMotion;

    // Use this for initialization
    new void Start()
    {
        base.Start();
		moveTime = (directionToMove.magnitude / moveSpeed);
		inMotion = false;
        locked = false;
        startPos = new List<Vector3>();
        endPos = new List<Vector3>();
        currentPos = new List<Vector3>();
    }

    // Update is called once per frame
    new void Update()
    {

        if (pressed && !locked && !inMotion)
        {
            Debug.Log("Here");
            foreach (GameObject platform in platforms)
			{
                Debug.Log(platform);
				inMotion = true;
				startPos.Add(platform.transform.position);
                endPos.Add(platform.transform.position + directionToMove);
				StartCoroutine(MoveOut(platform, startPos.Count - 1));
				inMotion = false;
				atPeak = true;
			}
			locked = true;
		} 
		else if (!pressed && locked) 
		{
			locked = false;
			atPeak = false;
            int i = 0;
            foreach (GameObject platform in platforms)
            {
                currentPos.Add(platform.transform.position);
                moveTime = ((currentPos[i] - startPos[i]).magnitude / moveSpeed);
                StartCoroutine(MoveBack(platform, i));
                i++;
            }
        }

        base.Update();
    }

	public IEnumerator MoveOut(GameObject platform, int index)
	{
		float panStart = Time.time;
		while (Time.time < panStart + moveTime && pressed)
		{
            platform.transform.position = Vector3.Lerp(startPos[index], endPos[index], (Time.time - panStart) / moveTime);
			yield return null;
		}
	}

	public IEnumerator MoveBack(GameObject platform, int index)
	{
		atPeak = false;
		inMotion = true;
		float panStart = Time.time;
		while (Time.time < panStart + moveTime)
		{
			platform.transform.position = Vector3.Lerp(currentPos[index], startPos[index], (Time.time - panStart) / moveTime);
			yield return null;
		}
		inMotion = false;
	}
}
