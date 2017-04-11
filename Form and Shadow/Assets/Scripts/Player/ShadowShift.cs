using UnityEngine;
using System.Collections;

public class ShadowShift : MonoBehaviour {

	// This is the time it takes to scale up/down.
	public float duration = 0f;

	// This remains true while it is in the prossess of scaling.
	private bool isScaling = false;


	// This is called every frame.  I will use it to check if space is pressed down, then trigger the scale.
	void Update () {

		if (Input.GetKeyDown ("L")) {  ///----------------------------------THIS IS WHERE WHAT KEY NEEDS TO BE PRESSED TO INITIATE SHADOW SHIFT
			StartCoroutine (ShadowShifter ());
		}
	}

	public IEnumerator ShadowShifter () {

		// Check if we are scaling now, if so exit method to avoid overlap.
		if (isScaling)
			yield break;

		// Declare that we are scaling now.
		isScaling = true;

		// Grab the current time and store it in a variable.
		float startTime = Time.time;

		while (Time.time - startTime < duration) {
			float amount = (Time.time - startTime) / duration * 5; ///-----THIS IS THE SPEED AT WHICH IT SHADOW SHIFTS (BIGGER THE # AFTER DURIATION THE FASTER IT IS)
			transform.localScale = Vector3.Lerp (Vector3.one , Vector3.one * .2f, amount * 5); ///-----THIS NEED TO CHANGE SO THE PLAYER GOES FLAT AND NOT SMALLER IN ALL AXIS
			yield return null;
		}

		// Snap the scale to 3.0f.
		transform.localScale = Vector3.one * 0.2f;

		// Leave the scale at 3 for 2 seconds (this can be changed at any time).
		yield return new WaitForSeconds (0f*5); //-----------------------------------THIS IS HOW LONG IT WAITS BEFORE SCALE BACK UP TO NORMAL SIZE

		// Now for the scale down part.  Store the current time in the same variable.
		startTime = Time.time;

		while (Time.time - startTime < duration) {
			float amount = (Time.time - startTime) / duration * 5; ///-----THIS IS THE SPEED AT WHICH IT SHADOW SHIFTS (BIGGER THE # AFTER DURIATION THE FASTER IT IS)
			transform.localScale = Vector3.Lerp (Vector3.one * .2f, Vector3.one, amount * 5); ///-----THIS NEED TO CHANGE SO THE PLAYER GOES FLAT AND NOT BIGGER IN ALL AXIS
			yield return null;
		}

		// Snap the scale to 1.0.
		transform.localScale = Vector3.one;

		// Declare that we are no longer modifing the scale.
		isScaling = false;
	}
}