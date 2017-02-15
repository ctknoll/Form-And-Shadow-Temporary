using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleSwitch : MonoBehaviour {

	public bool pressed;
    public GameObject switchButton;
    public float pressAnimationTime;
	public float timerDuration;

    private Vector3 highLerpPosition;
    private Vector3 lowLerpPosition;
	private float runningTime = 0;


	public void Start()
	{
        pressed = false;
        highLerpPosition = switchButton.transform.position;
        lowLerpPosition = switchButton.transform.position - new Vector3(0, 0.1f, 0);
    }

	// Update is called once per frame
    void OnTriggerStay (Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (Input.GetButtonDown ("Grab")) 
			{
				if(pressed) 
				{
					pressed = false;
					runningTime = 0;
                    StartCoroutine(DepressSwitch());
				} 
				else 
				{
					pressed = true;
					runningTime = timerDuration;
                    StartCoroutine(PressSwitch());
                    StartCoroutine(DepressSwitchWait());
				}

			}
        }
    }
	public void Update () 
	{
		if (runningTime > 0) 
		{
			runningTime -= Time.deltaTime;
		} 
		else if (runningTime <= 0 && runningTime > -1) 
		{
			pressed = false;
            runningTime = 0;
		}
	}

    public IEnumerator PressSwitch()
    {
        float panStart = Time.time;
        while (Time.time < panStart + pressAnimationTime)
        {
            switchButton.transform.position = Vector3.Lerp(highLerpPosition, lowLerpPosition, (Time.time - panStart) / pressAnimationTime);
            yield return null;
        }
    }

    public IEnumerator DepressSwitch()
    {
        float panStart = Time.time;
        while (Time.time < panStart + pressAnimationTime)
        {
            switchButton.transform.position = Vector3.Lerp(lowLerpPosition, highLerpPosition, (Time.time - panStart) / pressAnimationTime);
            yield return null;
        }
    }

    public IEnumerator DepressSwitchWait()
    {
        yield return new WaitForSeconds(timerDuration);

        float panStart = Time.time;
        while (Time.time < panStart + pressAnimationTime)
        {
            switchButton.transform.position = Vector3.Lerp(lowLerpPosition, highLerpPosition, (Time.time - panStart) / pressAnimationTime);
            yield return null;
        }
    }
}
