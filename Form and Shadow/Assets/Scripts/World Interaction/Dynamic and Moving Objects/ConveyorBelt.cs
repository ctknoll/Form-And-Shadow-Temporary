using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour {
	public float spawnCooldown;
	public GameObject conveyorPlatformPrefab;

	private float spawnTime;

	void Start ()
	{
		spawnTime = 0;
	}
	
	void Update ()
	{
		if(Time.time >= spawnTime + spawnCooldown)
		{
			spawnTime = Time.time;
			GameObject conveyorPlatform = Instantiate(conveyorPlatformPrefab, transform.position, transform.rotation, null) as GameObject;
			conveyorPlatform.GetComponent<ConveyorPlatform>().conveyorStart = transform.GetChild(0).gameObject.transform;
			conveyorPlatform.GetComponent<ConveyorPlatform>().conveyorEnd = transform.GetChild(1).gameObject.transform;
		}
	}
}

