using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_OnTrigger : MovingPlatform {
	
	public float triggerTime = 0;

	new void Start()
	{
		base.Start();
		slowValue = 0;
	}

	public IEnumerator waitForPlatform(float timeToWait)
	{
		yield return new WaitForSeconds(timeToWait);
		slowValue = 1;
	}

	void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter (other);
		StartCoroutine (waitForPlatform (triggerTime));
	}
}
