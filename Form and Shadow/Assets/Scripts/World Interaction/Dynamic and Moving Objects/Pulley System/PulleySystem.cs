using UnityEngine;
using System.Collections;

public class PulleySystem : MonoBehaviour {
	public float spawnCooldown;
	public GameObject pulleyPlatformPrefab;

	private float spawnTime;
	private float personalTime;

	void Start ()
	{
		spawnTime = 0;
	}
	
	void Update ()
	{
		if(!PlayerMovement.shiftingIn && !PlayerMovement.shiftingOut)
		{
			personalTime += Time.deltaTime;	
			if(personalTime >= spawnTime + spawnCooldown)
			{
				spawnTime = personalTime;
				GameObject pulleyPlatform = Instantiate(pulleyPlatformPrefab, transform.position, transform.rotation, null) as GameObject;
				pulleyPlatform.GetComponent<PulleySystemPlatform>().pulleyStart = transform.GetChild(0).gameObject.transform;
				pulleyPlatform.GetComponent<PulleySystemPlatform>().pulleyEnd = transform.GetChild(1).gameObject.transform;
			}
		}
	}
}

