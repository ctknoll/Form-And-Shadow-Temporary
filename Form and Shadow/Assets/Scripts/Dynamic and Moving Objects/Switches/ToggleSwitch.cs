using UnityEngine;
using System.Collections;

public class ToggleSwitch : MonoBehaviour {

	public bool pressed;
    public GameObject leverArm;
    public float pressAnimationTime;
	public float timerDuration;
    public AudioSource timerAudioSource;
    public AudioSource switchAudioSource;

    private Vector3 startLerpRotation;
    private Vector3 endLerpRotation;
	public float runningTime = 0;


	public void Start()
	{
        pressed = false;
        startLerpRotation = leverArm.transform.eulerAngles;
        endLerpRotation = leverArm.transform.eulerAngles + new Vector3(0, 0, 90);
    }

	// Update is called once per frame
    void OnTriggerStay (Collider other)
    {
        if(other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
        {
			if (!pressed) {
                GameController.ToggleInteractTooltip(true);
				if (Input.GetButtonDown ("Grab")) 
				{
					pressed = true;
					runningTime = timerDuration;
					StartCoroutine (PressSwitch ());
                    if (timerDuration >= 0) StartCoroutine(DepressSwitch());
					GameController.ToggleInteractTooltip (false);
				}
			}
            //else
            //{
            //    if (Input.GetButtonDown("Grab"))
            //    {
            //        pressed = false;
            //        GetComponent<AudioSource>().Stop();
            //        runningTime = 0;
            //        StartCoroutine(DepressSwitch());
            //        GameController.ToggleInteractTooltip(false);
            //    }
            //}
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
        {
            GameController.ToggleInteractTooltip(false);
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

        switchAudioSource.Play();

        while (Time.time < panStart + pressAnimationTime)
        {
            leverArm.transform.eulerAngles = Vector3.Lerp(startLerpRotation, endLerpRotation, (Time.time - panStart) / pressAnimationTime);
            yield return null;
        }
    }

    public IEnumerator DepressSwitch()
    {
        timerAudioSource.Play();
        if (timerDuration >= 0) yield return new WaitForSeconds(timerDuration);
        switchAudioSource.Play();
        timerAudioSource.Stop();

        float panStart = Time.time;
        while (Time.time < panStart + pressAnimationTime)
        {
            leverArm.transform.eulerAngles = Vector3.Lerp(endLerpRotation, startLerpRotation, (Time.time - panStart) / pressAnimationTime);
            yield return null;
        }
        pressed = false;
    }
}
