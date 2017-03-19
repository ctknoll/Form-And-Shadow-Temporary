﻿using UnityEngine;
using System.Collections;

public class AcidPool : MonoBehaviour {
    public float acidPoolDuration;
    public float acidPoolGrowScale;

	void Start ()
    {
        StartCoroutine(GrowAcidPool());
	}
	
	IEnumerator GrowAcidPool()
    {
        float growStartTime = Time.time;
        float growPersonalTimer = growStartTime;
        Vector3 startScale = transform.lossyScale;
        Vector3 endScale = new Vector3(acidPoolGrowScale * transform.lossyScale.x, transform.lossyScale.y, acidPoolGrowScale * transform.lossyScale.z);

        while (growPersonalTimer < growStartTime + acidPoolDuration)
        {
            if (!PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
            {
                growPersonalTimer += Time.deltaTime;
            }
            transform.localScale = Vector3.Lerp(startScale, endScale, (growPersonalTimer - growStartTime) / acidPoolDuration);
            yield return null;
        }
        Destroy(gameObject);
    }
}