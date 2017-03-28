using UnityEngine;

public class ShadowmeldObjectVFXControl : MonoBehaviour {
	void Start ()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 0, 0);
        SetParticleSettings();
	}

    void Update()
    {
        if(PlayerMovement.shadowMelded)
        {
            GetComponent<ParticleSystem>().Play();
        }
        else
        {
            GetComponent<ParticleSystem>().Clear();
            GetComponent<ParticleSystem>().Stop();
        }
    }
    void SetParticleSettings ()
    {
        ParticleSystem.ShapeModule shapeMod = GetComponent<ParticleSystem>().shape;
        shapeMod.mesh = GetComponentInParent<MeshFilter>().mesh;

        ParticleSystem.EmissionModule emissionMod = GetComponent<ParticleSystem>().emission;
        float[] parentScales = new float[] { transform.parent.lossyScale.x, transform.parent.lossyScale.y, transform.parent.lossyScale.z };
        emissionMod.rateOverTime = 500 * Mathf.Max(parentScales);
	}
}
