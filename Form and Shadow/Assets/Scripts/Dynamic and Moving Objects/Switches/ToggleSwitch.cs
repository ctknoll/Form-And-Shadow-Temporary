using UnityEngine;
using System.Collections;

public class ToggleSwitch : MonoBehaviour {
    public GameObject leverArm;

    public bool timerToggleSwitch;
    public bool lerpToggleSwitch;
    public float timerDuration;

    public bool pressed;
    public float switchFlipAnimationTime;
    public AudioSource timerAudioSource;
    public AudioSource switchFlipAudioSource;

    private bool animating;
    private float runningTime = 0;
    private bool toggledOn;
    private Vector3 startLerpRotation;
    private Vector3 endLerpRotation;


	public void Start()
	{
        animating = false;
        toggledOn = false;
        pressed = false;
        startLerpRotation = leverArm.transform.eulerAngles;
        endLerpRotation = leverArm.transform.eulerAngles + new Vector3(0, 0, 90);
    }

	// Update is called once per frame
    void OnTriggerStay (Collider other)
    {
        if(other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
        {
            if(!animating)
            {
                GameController.ToggleInteractTooltip(true);
                if (Input.GetButtonDown("Grab"))
                {
                    if (timerToggleSwitch)
                    {
                        pressed = true;
                        runningTime = timerDuration;
                        StartCoroutine(PressSwitchTimer());
                        StartCoroutine(DepressSwitchTimer());
                        GameController.ToggleInteractTooltip(false);
                    }
                    else
                    {
                        pressed = true;
                        StartCoroutine(PressSwitchToggle());
                        GameController.ToggleInteractTooltip(false);
                    }
                }
            }
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
            runningTime = 0;
		}
	}

    public IEnumerator PressSwitchTimer()
    {
        switchFlipAudioSource.Play();
        animating = true;
        float panStart = Time.time;
        while (Time.time < panStart + switchFlipAnimationTime)
        {
            leverArm.transform.eulerAngles = Vector3.Lerp(startLerpRotation, endLerpRotation, (Time.time - panStart) / switchFlipAnimationTime);
            yield return null;
        }
    }

    public IEnumerator PressSwitchToggle()
    {
        switchFlipAudioSource.Play();
        animating = true;
        float panStart = Time.time;
        while (Time.time < panStart + switchFlipAnimationTime)
        {
            if (!toggledOn)
                leverArm.transform.eulerAngles = Vector3.Lerp(startLerpRotation, endLerpRotation, (Time.time - panStart) / switchFlipAnimationTime);
            else
                leverArm.transform.eulerAngles = Vector3.Lerp(endLerpRotation, startLerpRotation, (Time.time - panStart) / switchFlipAnimationTime);
            yield return null;
        }
        if (!toggledOn)
            toggledOn = true;
        else
            toggledOn = false;
        if (lerpToggleSwitch)
        {
            if (!toggledOn)
                pressed = false;
        }
        else
            pressed = false;
        animating = false;
    }

    public IEnumerator DepressSwitchTimer()
    {
        timerAudioSource.Play();
        animating = true;
        if (timerDuration >= 0) yield return new WaitForSeconds(timerDuration);
        switchFlipAudioSource.Play();
        timerAudioSource.Stop();

        float panStart = Time.time;
        while (Time.time < panStart + switchFlipAnimationTime)
        {
            leverArm.transform.eulerAngles = Vector3.Lerp(endLerpRotation, startLerpRotation, (Time.time - panStart) / switchFlipAnimationTime);
            yield return null;
        }
        pressed = false;
        animating = false;
    }
}
