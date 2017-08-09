using UnityEngine;

public abstract class Pressure_Switch : MonoBehaviour
{
    public bool m_Pressed;
    Animator switchAnimator;

	void Start ()
    {
        switchAnimator = GetComponentInChildren<Animator>();	
	}
	
	void Update ()
    {
        switchAnimator.SetBool("Pressed", m_Pressed);
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
