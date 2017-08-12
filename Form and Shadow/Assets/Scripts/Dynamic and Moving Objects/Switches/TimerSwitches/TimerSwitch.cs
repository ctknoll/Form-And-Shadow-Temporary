using UnityEngine;

public abstract class TimerSwitch : Switch
{
    [SerializeField] float m_TimerDuration;

    float timerDurationStart;
    bool playerCanInteract;
    bool timerIsActive;

	protected new void Start ()
    {
        base.Start();
	}
	
	protected new void Update ()
    {
        base.Update();

        if (!timerIsActive)
            UpdateTimerSwitchInput();
        else
            UpdateTimerDuration();
	}

    void UpdateTimerSwitchInput()
    {
        if(playerCanInteract)
        {
            if(Input.GetButtonDown("Grab"))
            {
                Activate();
            }
        }
    }

    void UpdateTimerDuration()
    {
        if (Time.time > timerDurationStart + m_TimerDuration)
        {
            Deactivate();
        }
    }

    void Activate()
    {
        timerDurationStart = Time.time;
        timerIsActive = true;
        m_Pressed = true;
    }

    void Deactivate()
    {
        timerIsActive = false;
        m_Pressed = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !timerIsActive)
            playerCanInteract = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerCanInteract = false;
    }
}
