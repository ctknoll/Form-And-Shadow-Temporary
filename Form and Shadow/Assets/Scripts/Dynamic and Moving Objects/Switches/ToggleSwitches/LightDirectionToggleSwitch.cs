using UnityEngine;

public class LightDirectionToggleSwitch : ToggleSwitch
{

    public GameObject[] lightSources;
    public GameObject lightDirectionPointer;
    public float degreesToRotate;
    private bool turnedLightSourceForPressed;
    private bool turnedLightSourceForDepressed;
    private Vector3 startRotation;
	public bool turnClockwise;

    // Use this for initialization
    new void Start()
    {
        startRotation = lightDirectionPointer.transform.rotation.eulerAngles;
        turnedLightSourceForDepressed = true;
        turnedLightSourceForPressed = false;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            if (!turnedLightSourceForPressed)
            {
                foreach (GameObject lightSource in lightSources)
                {
                    if (lightSource.GetComponent<LightSourceControl>() != null)
                    {
                        lightSource.GetComponent<LightSourceControl>().turnLightSource(turnClockwise);
                        turnedLightSourceForPressed = true;
                        turnedLightSourceForDepressed = false;
                        timerAudioSource.Play();
                    }
                }
            }
        }
        else if (!pressed)
        {
            if(!turnedLightSourceForDepressed)
            {
                foreach (GameObject lightSource in lightSources)
                {
                    if (lightSource.GetComponent<LightSourceControl>() != null)
                    {
                        lightSource.GetComponent<LightSourceControl>().turnLightSource(turnClockwise);
                        turnedLightSourceForDepressed = true;
                        turnedLightSourceForPressed = false;
                        timerAudioSource.Play();
                    }
                }
            }
        }
		lightDirectionPointer.transform.eulerAngles = new Vector3(startRotation.x, lightSources[0].transform.eulerAngles.y - 90, startRotation.z);
    }
}