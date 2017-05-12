﻿using UnityEngine;

public class TransformLock : MonoBehaviour
{
	private Vector3 startPos;
	private Quaternion startRotation;

	void Start()
	{
		startPos = transform.position;
		startRotation = transform.rotation;
	}

	void FixedUpdate()
	{
		transform.position = startPos;
		transform.rotation = startRotation;
	}
}

