using UnityEngine;
using System.Collections;

public class TransformLock : MonoBehaviour
{
	private Vector3 startPos;
	private Quaternion startRotation;

	void Start()
	{
		startPos = transform.position;
		startRotation = transform.rotation;
	}

	void Update()
	{
		transform.position = startPos;
		transform.rotation = startRotation;
	}
}

