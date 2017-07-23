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

    void SetParticleSettings ()
    {
        if(particleSystemType == ParticleSystemType.DEATH)
        {
            ParticleSystem.ShapeModule shapeMod = GetComponent<ParticleSystem>().shape;
            ParticleSystem.EmissionModule emissionMod = GetComponent<ParticleSystem>().emission;
            
            if(GetComponentInParent<ShadowmeldObjectControl>().shadowMeldObjectType == ShadowmeldObjectControl.ShadowMeldObjectType.SPIKES)
            {
                shapeMod.scale = GetComponentInParent<BoxCollider>().size;

                float[] parentScales = new float[] { GetComponentInParent<BoxCollider>().size.x, GetComponentInParent<BoxCollider>().size.y, GetComponentInParent<BoxCollider>().size.z };
                float cappedParentScale = Mathf.Max(parentScales);
                cappedParentScale = Mathf.Clamp(cappedParentScale, 1, 10);
                emissionMod.rateOverTime = particlesPerScale * cappedParentScale;
            }
        }
        if(particleSystemType == ParticleSystemType.COLLIDE)
        {
            ParticleSystem.ShapeModule shapeMod = GetComponent<ParticleSystem>().shape;
            ParticleSystem.EmissionModule emissionMod = GetComponent<ParticleSystem>().emission;

            if (GetComponentInParent<ShadowmeldObjectControl>().shadowMeldObjectType == ShadowmeldObjectControl.ShadowMeldObjectType.FLAT_SPIKES)
            {
                shapeMod.scale = Vector3.Scale(shapeMod.scale, new Vector3(2, 1, 2));
                float[] parentScales = new float[] { GetComponentInParent<BoxCollider>().bounds.size.x, GetComponentInParent<BoxCollider>().bounds.size.y, GetComponentInParent<BoxCollider>().bounds.size.z };
                float cappedParentScale = Mathf.Max(parentScales);
                cappedParentScale = Mathf.Clamp(cappedParentScale, 1, 10);
                emissionMod.rateOverTime = particlesPerScale * cappedParentScale;
            }
            else
            {
                float[] parentScales = new float[] { transform.parent.lossyScale.x, transform.parent.lossyScale.y, transform.parent.lossyScale.z };
                float cappedParentScale = Mathf.Max(parentScales);
                cappedParentScale = Mathf.Clamp(cappedParentScale, 1, 10);
                emissionMod.rateOverTime = particlesPerScale * cappedParentScale;
            }
        }
        else if(particleSystemType == ParticleSystemType.INVISIBLE)
        {
            ParticleSystem.EmissionModule emissionMod = GetComponent<ParticleSystem>().emission;
            float[] parentScales = new float[] { transform.parent.lossyScale.x, transform.parent.lossyScale.y, transform.parent.lossyScale.z };

            float cappedParentScale = Mathf.Max(parentScales);
            cappedParentScale = Mathf.Clamp(cappedParentScale, 1, 10);
            emissionMod.rateOverTime = particlesPerScale * cappedParentScale;
        }
	}
}
