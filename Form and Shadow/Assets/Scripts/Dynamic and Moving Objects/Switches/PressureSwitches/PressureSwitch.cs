using UnityEngine;

public abstract class PressureSwitch : Switch
{

	protected new void Start ()
    {
        base.Start();
    }
	
	protected new void Update ()
    {
        base.Update();
	}

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Push_Cube"))
            m_Pressed = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.CompareTag("Push_Cube"))
            m_Pressed = false;
    }
}
