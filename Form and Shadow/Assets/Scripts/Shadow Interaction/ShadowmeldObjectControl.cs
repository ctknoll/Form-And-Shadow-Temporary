using UnityEngine;

public class ShadowmeldObjectControl : MonoBehaviour {
    public enum ShadowMeldObjectType {GLASS, FLAT_SPIKES, WATER, ACID_POOL, CONVEYOR_BELT, ENEMY_TOAD};
    public ShadowMeldObjectType shadowMeldObjectType;
    private LayerMask startingLayer;
    private bool switched;

	void Start ()
    {
        startingLayer = gameObject.layer;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (PlayerMovement.shadowMelded)
        {
            if(!switched)
            {
                TurnOnShadowmeldLayerandCollision();
                switched = true;
            }
        }
        else
        {
            if (switched)
            {
                TurnOffShadowmeldLayerandCollision();
                switched = false;
            }
        }
    }

    public void TurnOnShadowmeldLayerandCollision()
    {
        switch (shadowMeldObjectType)
        {
            case ShadowMeldObjectType.GLASS:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
                break;
            case ShadowMeldObjectType.WATER:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
                break;
            case ShadowMeldObjectType.FLAT_SPIKES:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Collide");
                GetComponent<BoxCollider>().isTrigger = false;
                break;
            case ShadowMeldObjectType.ENEMY_TOAD:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Collide");
                GetComponent<MeshCollider>().convex = true;
                GetComponent<MeshCollider>().isTrigger = true;
                gameObject.AddComponent<Killzone>();
                break;
            default:
                break;
        }
    }

    public void TurnOffShadowmeldLayerandCollision()
    {
        gameObject.layer = startingLayer;
        switch (shadowMeldObjectType)
        {
            case ShadowMeldObjectType.GLASS:
                break;
            case ShadowMeldObjectType.WATER:
                break;
            case ShadowMeldObjectType.FLAT_SPIKES:
                GetComponent<BoxCollider>().isTrigger = true;
                break;
            case ShadowMeldObjectType.ENEMY_TOAD:
                GetComponent<MeshCollider>().isTrigger = false;
                GetComponent<MeshCollider>().convex = false;
                Destroy(GetComponent<Killzone>());
                break;
            default:
                break;
        }
    }
}
