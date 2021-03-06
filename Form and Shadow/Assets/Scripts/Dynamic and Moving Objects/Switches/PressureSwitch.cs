using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PressureSwitch : MonoBehaviour 
{
    public bool playAudio;
    public AudioSource pressureSwitchAudioSource;
    public AudioClip pressureSwitchLowerClip;
    public AudioClip pressureSwitchRaiseClip;

    public bool canInteract;
	public bool pressed;
	private float activationTime = 0;
    
	private float holdCubeX;
	private float holdCubeZ;

    private List<Collider> inTriggerZone;
	
	public void Start()
	{
		holdCubeX = 0;
		holdCubeZ = 0;
        inTriggerZone = new List<Collider>();
	}

	// Update is called once per frame
	public void Update () 
	{
		if (pressed && activationTime < 30) 
		{
			transform.position += new Vector3(0, -(transform.lossyScale.y / 60), 0);
			activationTime++;
		}

		if (!pressed && activationTime > 0) 
		{
			transform.position += new Vector3(0, (transform.lossyScale.y / 60), 0);
			activationTime--;
		}
        if((PlayerMovement.shadowShiftingIn || PlayerMovement.shadowShiftingOut))
        {
            OnTriggerExit(GameObject.Find("Player_Character").GetComponent<Collider>());
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        inTriggerZone.Add(other);
        if((other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) || other.gameObject.tag == "Push Cube")
        {
            if (playAudio)
            {
                pressureSwitchAudioSource.clip = pressureSwitchLowerClip;
                StartCoroutine(ControlPressureSwitchAudio());
            }
        }
    }

    public IEnumerator ControlPressureSwitchAudio()
    {
        pressureSwitchAudioSource.Play();
        float audioStart = Time.time;
        float currentTimerDuration = audioStart;
        while (currentTimerDuration < audioStart + pressureSwitchAudioSource.clip.length)
        {
            if (!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut && !GameController.paused)
            {
                currentTimerDuration += Time.deltaTime;
            }
            if (GameController.paused || PlayerMovement.shadowShiftingIn || PlayerMovement.shadowShiftingOut)
            {
                pressureSwitchAudioSource.Pause();
            }
            else
            {
                pressureSwitchAudioSource.UnPause();
            }
            yield return null;
        }
        pressureSwitchAudioSource.Stop();
    }

    public void OnTriggerStay(Collider other)
	{
        if ((other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) || other.gameObject.tag == "Push Cube")
            pressed = true;
		//Holds Push Cube in place so player cannot move it after activating pressure plate
		if (other.gameObject.tag == "Push Cube") //Removed by DC by accident. Re-added from backup by JSM 2.27.17
		{
			other.transform.parent = gameObject.transform;
			if (holdCubeX == 0)
				holdCubeX = other.transform.position.x;
			if (holdCubeZ == 0)
				holdCubeZ = other.transform.position.z;
			other.transform.position = new Vector3(holdCubeX, other.transform.position.y, holdCubeZ);
		}
    }

	public void OnTriggerExit(Collider other)
    {
        if (inTriggerZone.Contains(other))
            inTriggerZone.RemoveAll((Collider c) => { return c.Equals(other);});
        if (inTriggerZone.FindAll((Collider c) => { return c.gameObject.tag == "Player" || c.gameObject.tag == "Push Cube";}).Count == 0)
            pressed = false;

        if ((other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) || other.gameObject.tag == "Push Cube")
        {
            if(playAudio)
            {
                pressureSwitchAudioSource.clip = pressureSwitchRaiseClip;
                StartCoroutine(ControlPressureSwitchAudio());
            }
        }
    }
		
}
