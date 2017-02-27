﻿using UnityEngine;
using System.Collections;

public class ToggleSwitch : MonoBehaviour {

	public bool pressed;
    public GameObject switchButton;
    public float pressAnimationTime;
	public float timerDuration;

    private Vector3 highLerpPosition;
    private Vector3 lowLerpPosition;
	protected float runningTime = 0;


	public void Start()
	{
        pressed = false;
        highLerpPosition = switchButton.transform.position;
        lowLerpPosition = switchButton.transform.position - new Vector3(0, 0.1f, 0);
    }

	// Update is called once per frame
    void OnTriggerStay (Collider other)
    {
        if(other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
        {
			if (!pressed) {
				GameController.SetInteractText ("Press E to toggle this switch");
				if (Input.GetButtonDown ("Grab")) 
				{
					pressed = true;
					runningTime = timerDuration;
					StartCoroutine (PressSwitch ());
					if (timerDuration >= 0) StartCoroutine (DepressSwitch ());
					GameController.SetInteractText ("");
				}
			} 
			else 
			{
				if (Input.GetButtonDown ("Grab")) 
				{
					if (timerDuration == -1) 
					{
						pressed = false;
						runningTime = 0;
						StartCoroutine (DepressSwitch ());		
						GameController.SetInteractText ("");
					}
				}
			}
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
        {
            GameController.ResetInteractText();
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
		if (timerDuration >= 0) yield return new WaitForSeconds(timerDuration);

        float panStart = Time.time;
        while (Time.time < panStart + pressAnimationTime)
        {
            switchButton.transform.position = Vector3.Lerp(lowLerpPosition, highLerpPosition, (Time.time - panStart) / pressAnimationTime);
            yield return null;
        }
    }
}
