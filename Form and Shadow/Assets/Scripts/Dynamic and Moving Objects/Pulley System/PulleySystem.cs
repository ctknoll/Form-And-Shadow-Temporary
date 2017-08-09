using UnityEngine;

public class PulleySystem : MonoBehaviour
{
    public GameObject m_PulleyPlatformPrefab;
    [Range(1f, 3f)][SerializeField] float m_SpawnCooldown = 1f;
	float spawnTime;
	float personalTime;

	void Start ()
	{
		spawnTime = 0;
	}
	
	void Update ()
	{
		if(PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PlayerState.Shifting && !GameController.paused)
		{
			personalTime += Time.deltaTime;	
			if(personalTime >= spawnTime + m_SpawnCooldown)
			{
				spawnTime = personalTime;
				GameObject pulleyPlatform = Instantiate(m_PulleyPlatformPrefab, transform.GetChild(0).position, transform.rotation) as GameObject;
                foreach(Transform pathNode in GetComponentInChildren<Transform>())
                {
                    pulleyPlatform.GetComponent<PulleyMovingPlatform>().m_PathLocations.Add(pathNode);
                }
            }
		}
	}
}

