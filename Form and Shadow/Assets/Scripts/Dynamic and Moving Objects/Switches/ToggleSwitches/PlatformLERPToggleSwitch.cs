using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLERPToggleSwitch : ToggleSwitch
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

    // Use this for initialization
    new void Start()
    {
        base.Start();
		moveTime = (directionToMove.magnitude / moveSpeed);
        startPos = new List<Vector3>();
        endPos = new List<Vector3>();
        currentPos = new List<Vector3>();
    }

    // Update is called once per frame
    new public void Update()
    {
		if (pressed && !locked) {
			foreach (GameObject platform in platforms)
			{
                startPos.Add(platform.transform.position);
                endPos.Add(platform.transform.position + directionToMove);
                StartCoroutine(MoveOut(platform, startPos.Count - 1));
			}
			locked = true;
		} 
		else if (!pressed && locked) 
		{
            locked = false;
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
		while (Time.time < panStart + moveTime)
		{
            platform.transform.position = Vector3.Lerp(startPos[index], endPos[index], (Time.time - panStart) / moveTime);
            yield return null;
        }
	}

	public IEnumerator MoveBack(GameObject platform, int index)
	{
		float panStart = Time.time;
		while (Time.time < panStart + moveTime)
		{
            platform.transform.position = Vector3.Lerp(currentPos[index], startPos[index], (Time.time - panStart) / moveTime);
            yield return null;
        }
	}
}
