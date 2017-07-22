﻿using UnityEngine;

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
		if(!PlayerShadowInteraction.shadowShiftingIn && !PlayerShadowInteraction.shadowShiftingOut && !GameController.paused)
		{
			personalTime += Time.deltaTime;	
			if(personalTime >= spawnTime + spawnCooldown)
			{
				spawnTime = personalTime;
				GameObject pulleyPlatform = Instantiate(pulleyPlatformPrefab, transform.GetChild(0).position, transform.rotation, null) as GameObject;
				pulleyPlatform.GetComponent<PulleySystemPlatform>().pulleyStart = transform.GetChild(0).gameObject.transform;
				pulleyPlatform.GetComponent<PulleySystemPlatform>().pulleyEnd = transform.GetChild(1).gameObject.transform;
			}
		}
	}
}

