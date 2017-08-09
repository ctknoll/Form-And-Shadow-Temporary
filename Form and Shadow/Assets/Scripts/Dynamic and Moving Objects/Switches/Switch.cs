
using UnityEngine;

public abstract class Switch : MonoBehaviour
{
    public bool m_Pressed = false;

    Animator switchAnimator;

    protected void Start ()
    {
        switchAnimator = GetComponentInChildren<Animator>();
    }

    protected void Update ()
    {
        UpdateSwitchAnimator();	
	}

    void UpdateSwitchAnimator()
    {
        switchAnimator.SetBool("Pressed", m_Pressed);
    }
}
