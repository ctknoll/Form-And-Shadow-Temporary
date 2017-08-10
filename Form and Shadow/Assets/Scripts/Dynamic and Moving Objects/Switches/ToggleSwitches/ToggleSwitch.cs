using UnityEngine;

public abstract class ToggleSwitch : Switch
{
    bool playerCanInteract;
    float internalCooldown;
    float internalCooldownStart;

	protected new void Start ()
    {
        base.Start();
        internalCooldown = 0.5f;
        internalCooldownStart = 0f;
    }
	
	protected new void Update ()
    {
        base.Update();

        if(Time.time > internalCooldownStart + internalCooldown)
            UpdateToggleSwitchInput();
	}

    void UpdateToggleSwitchInput()
    {
        if(playerCanInteract)
        {
            if (Input.GetButtonDown("Grab"))
            {
                internalCooldownStart = Time.time;
                m_Pressed = !m_Pressed;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerCanInteract = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerCanInteract = false;
    }
}
