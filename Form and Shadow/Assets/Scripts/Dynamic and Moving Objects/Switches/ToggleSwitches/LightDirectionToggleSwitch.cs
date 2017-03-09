using UnityEngine;

public class LightDirectionToggleSwitch : ToggleSwitch
{

    public GameObject[] lightSources;
    public GameObject lightDirectionPointer;
    public float degreesToRotate;
    private bool locked;
    private Vector3 startRotation;
	public bool turnClockwise;

    // Use this for initialization
    new void Start()
    {
        startRotation = lightDirectionPointer.transform.rotation.eulerAngles;
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if (pressed)
        {
            if (!locked)
            {
                foreach (GameObject lightSource in lightSources)
                {
                    if (lightSource.GetComponent<LightSourceControl>() != null)
                    {
                        lightSource.GetComponent<LightSourceControl>().turnLightSource(turnClockwise);
                        locked = true;
                    }
                }
            }
        }
        else
        {
			foreach (GameObject lightSource in lightSources) 
			{
				if (lightSource.GetComponent<LightSourceControl>() != null)
				{
					locked = false;
				}
			}
        }
		lightDirectionPointer.transform.eulerAngles = new Vector3(startRotation.x, lightSources[0].transform.eulerAngles.y - 90, startRotation.z);
        base.Update();
    }
}