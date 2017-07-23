using UnityEngine;
using System.Collections;

public class AcidPool : MonoBehaviour {
    public float acidPoolDuration;
    public float acidPoolGrowScale;
	public bool destroySelf;

	void Start ()
    {
        StartCoroutine(GrowAcidPool());
	}
	
	IEnumerator GrowAcidPool()
    {
        float growStartTime = GameController.masterTimer;
        float growPersonalTimer = growStartTime;
        Vector3 startScale = transform.lossyScale;
        Vector3 endScale = new Vector3(acidPoolGrowScale * transform.lossyScale.x, transform.lossyScale.y, acidPoolGrowScale * transform.lossyScale.z);

        while (growPersonalTimer < growStartTime + acidPoolDuration)
        {
            if (NewPlayerShadowInteraction.m_CurrentPlayerState != NewPlayerShadowInteraction.PLAYERSTATE.SHIFTING)
            {
                growPersonalTimer += Time.deltaTime;
            }
            transform.localScale = Vector3.Lerp(startScale, endScale, (growPersonalTimer - growStartTime) / acidPoolDuration);
			yield return null;
        }
		if(destroySelf) StartCoroutine(removeAcidPool());		 
    }

	IEnumerator removeAcidPool()
	{
		while (GameController.resetting) 
		{
			yield return new WaitForSeconds(Time.deltaTime);
		}
		Destroy(gameObject);
	}
}