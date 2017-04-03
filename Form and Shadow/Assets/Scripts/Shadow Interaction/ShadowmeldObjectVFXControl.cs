using UnityEngine;

public class ShadowmeldObjectVFXControl : MonoBehaviour {
    public float particlesPerScale;
    public enum ParticleSystemType {INVISIBLE, COLLIDE, DEATH};
    public ParticleSystemType particleSystemType;

	void Start ()
    {
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
        if(particleSystemType == ParticleSystemType.DEATH)
        {
            ParticleSystem.ShapeModule shapeMod = GetComponent<ParticleSystem>().shape;
            ParticleSystem.EmissionModule emissionMod = GetComponent<ParticleSystem>().emission;
            
            if(GetComponentInParent<ShadowmeldObjectControl>().shadowMeldObjectType == ShadowmeldObjectControl.ShadowMeldObjectType.SPIKES)
            {
                shapeMod.box = GetComponentInParent<BoxCollider>().size;

                float[] parentScales = new float[] { GetComponentInParent<BoxCollider>().size.x, GetComponentInParent<BoxCollider>().size.y, GetComponentInParent<BoxCollider>().size.z };
                emissionMod.rateOverTime = particlesPerScale * Mathf.Max(parentScales);
            }
        }
        if(particleSystemType == ParticleSystemType.COLLIDE)
        {
            ParticleSystem.ShapeModule shapeMod = GetComponent<ParticleSystem>().shape;
            ParticleSystem.EmissionModule emissionMod = GetComponent<ParticleSystem>().emission;

            if (GetComponentInParent<ShadowmeldObjectControl>().shadowMeldObjectType == ShadowmeldObjectControl.ShadowMeldObjectType.FLAT_SPIKES)
            {
                shapeMod.box = Vector3.Scale(shapeMod.box, new Vector3(2, 1, 2));
                float[] parentScales = new float[] { GetComponentInParent<BoxCollider>().bounds.size.x, GetComponentInParent<BoxCollider>().bounds.size.y, GetComponentInParent<BoxCollider>().bounds.size.z };
                emissionMod.rateOverTime = particlesPerScale * Mathf.Max(parentScales);
            }
            else
            {
                float[] parentScales = new float[] { transform.parent.lossyScale.x, transform.parent.lossyScale.y, transform.parent.lossyScale.z };
                emissionMod.rateOverTime = particlesPerScale * Mathf.Max(parentScales);
            }
        }
        else if(particleSystemType == ParticleSystemType.INVISIBLE)
        {
            ParticleSystem.EmissionModule emissionMod = GetComponent<ParticleSystem>().emission;
            float[] parentScales = new float[] { transform.parent.lossyScale.x, transform.parent.lossyScale.y, transform.parent.lossyScale.z };
            emissionMod.rateOverTime = particlesPerScale * Mathf.Max(parentScales);
        }
	}
}
