using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwitch : MonoBehaviour 
{
    public bool pressed;
    public GameObject switchButton;
    public float pressAnimationTime;
    private Vector3 highLerpPosition;
    private Vector3 lowLerpPosition;


	public void Start()
	{
		highLerpPosition = switchButton.transform.position;
        lowLerpPosition = switchButton.transform.position - new Vector3(0, 0.2f, 0);
	}
	
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Push Cube")
        {
            pressed = true;
            StartCoroutine(PressSwitch());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Push Cube")
        {
            pressed = false;
            StartCoroutine(DepressSwitch());
        }
    }

    public IEnumerator PressSwitch()
    {
        float panStart = Time.time;
        while (Time.time < panStart + pressAnimationTime)
        {
            switchButton.transform.position = Vector3.Lerp(highLerpPosition, lowLerpPosition, (Time.time - panStart) / pressAnimationTime);
            yield return null;
        }
    }

    public IEnumerator DepressSwitch()
    {
        float panStart = Time.time;
        while (Time.time < panStart + pressAnimationTime)
        {
            switchButton.transform.position = Vector3.Lerp(lowLerpPosition, highLerpPosition, (Time.time - panStart) / pressAnimationTime);
            yield return null;
        }
    }
}
