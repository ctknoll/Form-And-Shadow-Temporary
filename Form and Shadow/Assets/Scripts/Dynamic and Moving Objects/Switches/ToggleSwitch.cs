using UnityEngine;
using System.Collections;

/*

--Toggle Switch--
Abstract Class meant for all Toggle Switches to inherit from.
NOTE: DO NOT INSTANTIATE

*/

public abstract class ToggleSwitch : MonoBehaviour {
    public GameObject leverArm;

    public enum SwitchType {TIMER_TOGGLE, FLIP_TOGGLE}
    public SwitchType switchType;
    public float timerDuration;
	public float switchDelay;

    public bool pressed;
    public float switchFlipAnimationTime;
    public AudioSource timerAudioSource;
    public AudioSource switchFlipAudioSource;

    private bool animating;
    [HideInInspector]
    public bool toggledOn;
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
		if(other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.switch_cooldown)
        {
            if(!animating)
            {
                GameController.CheckInteractToolip(true, false);
                if (Input.GetButtonDown("Grab"))
                {
                    if (switchType == SwitchType.TIMER_TOGGLE)
                    {
                        if(!GameController.e_Switch_First_Time_Used)
                        {
                            GameController.e_Switch_First_Time_Used = true;
                        }
                        pressed = true;

                        StartCoroutine(PressSwitchTimer());
                        if(timerDuration > 0)
                            StartCoroutine(ControlTimerSwitchAudio());
                        GameController.CheckInteractToolip(false, false);
						GameController.switch_cooldown = true;
						StartCoroutine (SwitchCooldown (switchDelay));
                    }
                    else if(switchType == SwitchType.FLIP_TOGGLE)
                    {
                        if (!GameController.e_Switch_First_Time_Used)
                        {
                            GameController.e_Switch_First_Time_Used = true;
                        }
                        StartCoroutine(PressFlipToggle());
                        StartCoroutine(ControlFlipSwitchAudio());
                        GameController.CheckInteractToolip(false, false);
						GameController.switch_cooldown = true;
						StartCoroutine (SwitchCooldown (switchDelay));
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
        {
            GameController.CheckInteractToolip(false, false);
        }
    }

    public IEnumerator PressSwitchTimer()
    {
        switchFlipAudioSource.Play();
        animating = true;
        float panStart = Time.time;
        float flipPersonalTimer = panStart;
        while (flipPersonalTimer < panStart + switchFlipAnimationTime)
        {
            if (!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)
            {
                flipPersonalTimer += Time.deltaTime;
            }
            leverArm.transform.eulerAngles = Vector3.Lerp(startLerpRotation, endLerpRotation, (flipPersonalTimer - panStart) / switchFlipAnimationTime);
            yield return null;
        }
    }

    public IEnumerator ControlTimerSwitchAudio()
    {
        timerAudioSource.Play();
        float audioStart = Time.time;
        float currentTimerDuration = audioStart;
        while(currentTimerDuration < audioStart + timerAudioSource.clip.length)
        {
            if (!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)
            {
                currentTimerDuration += Time.deltaTime;
            }
            if (GameController.paused || PlayerMovement.shadowShiftingIn || PlayerMovement.shadowShiftingOut)
            {
                switchFlipAudioSource.Pause();
                timerAudioSource.Pause();
            }
            else
            {
                switchFlipAudioSource.UnPause();
                timerAudioSource.UnPause();
            }
            yield return null;
        }
        timerAudioSource.Stop();
        StartCoroutine(DepressSwitchTimer());
    }

    public IEnumerator DepressSwitchTimer()
    {
        switchFlipAudioSource.Play();
        animating = true;
        float panStart = Time.time;
        float flipPersonalTimer = panStart;
        while (flipPersonalTimer < panStart + switchFlipAnimationTime)
        {
            if (!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)
            {
                flipPersonalTimer += Time.deltaTime;
            }
            leverArm.transform.eulerAngles = Vector3.Lerp(endLerpRotation, startLerpRotation, (flipPersonalTimer - panStart) / switchFlipAnimationTime);
            yield return null;
        }
        pressed = false;
        animating = false;
    }

    public IEnumerator ControlFlipSwitchAudio()
    {
        switchFlipAudioSource.Play();
        float audioStart = Time.time;
        float currentTimerDuration = audioStart;
        while (currentTimerDuration < audioStart + switchFlipAudioSource.clip.length)
        {
            if (!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)
            {
                currentTimerDuration += Time.deltaTime;
            }
            if (GameController.paused || PlayerMovement.shadowShiftingIn || PlayerMovement.shadowShiftingOut)
            {
                switchFlipAudioSource.Pause();
            }
            else
            {
                switchFlipAudioSource.UnPause();
            }
            yield return null;
        }
        switchFlipAudioSource.Stop();
    }

	public IEnumerator SwitchCooldown(float timeToWait)
	{
		yield return new WaitForSeconds(timeToWait);
		GameController.switch_cooldown = false;
	}

    public IEnumerator PressFlipToggle()
    {
        animating = true;
        float panStart = Time.time;
        float flipPersonalTimer = panStart;
        while (flipPersonalTimer < panStart + switchFlipAnimationTime)
        {
            if (!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)
            {
                flipPersonalTimer += Time.deltaTime;
            }
            if (!pressed)
            {
                Debug.Log("Forward");
                leverArm.transform.eulerAngles = Vector3.Lerp(startLerpRotation, endLerpRotation, (flipPersonalTimer - panStart) / switchFlipAnimationTime);
            }
            else
            {
                Debug.Log("Backwards");
                leverArm.transform.eulerAngles = Vector3.Lerp(endLerpRotation, startLerpRotation, (flipPersonalTimer - panStart) / switchFlipAnimationTime);
            }
            yield return null;
        }
        pressed = !pressed;
        animating = false;
    }
}
