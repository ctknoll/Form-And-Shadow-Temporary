using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShadowBehavior : MonoBehaviour {

    public List<GameObject> shadowColliders;

	void Start ()
    {
        shadowColliders = new List<GameObject>();
        ShadowFinder shadowFinder = new ShadowFinder(gameObject, 120, 8);
        List<List<Vector2>> shadowPoints = shadowFinder.getShadowSamples();
        foreach (List<Vector2>shadowPoint in shadowPoints)
        {
            shadowColliders.Add(shadowFinder.generateCollidableShadow(shadowPoint));
        }
	}
}
