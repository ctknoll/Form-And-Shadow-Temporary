using UnityEngine;

public class ShadowmeldObjectControl : MonoBehaviour {
    public enum ShadowMeldObjectType {GLASS, WATER, FLAT_SPIKES, SPIKES, ACID_POOL, CONVEYOR_BELT, ENEMY_TOAD, NON_INTERACTIVE};
    public ShadowMeldObjectType shadowMeldObjectType;
    private LayerMask startingLayer;
    private bool switched;

    private Object shadowmeldCollideObjectVFX;
    private Object shadowmeldInvisibleObjectVFX;
    private Object shadowmeldDeathObjectVFX;
    private GameObject currentShadowmeldVFX;


    void Start ()
    {
        startingLayer = gameObject.layer;
        shadowmeldCollideObjectVFX = Resources.Load("ShadowmeldCollideObjectVFX");
        shadowmeldInvisibleObjectVFX = Resources.Load("ShadowmeldInvisibleObjectVFX");
        shadowmeldDeathObjectVFX = Resources.Load("ShadowmeldDeathObjectVFX");
        currentShadowmeldVFX = null;
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
        //if(shadowMeldObjectType != ShadowMeldObjectType.CONVEYOR_BELT)
        //    UpdateShadowmeldRenderMode();
        UpdateShadowmeldVFX();
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
            case ShadowMeldObjectType.ACID_POOL:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
                break;
            case ShadowMeldObjectType.CONVEYOR_BELT:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Ignore");
                break;
            case ShadowMeldObjectType.FLAT_SPIKES:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Collide");
                GetComponent<BoxCollider>().isTrigger = false;
                break;
            case ShadowMeldObjectType.NON_INTERACTIVE:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Collide");
                break;
            case ShadowMeldObjectType.ENEMY_TOAD:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Death");
                GetComponent<MeshCollider>().convex = true;
                GetComponent<MeshCollider>().isTrigger = true;
                gameObject.AddComponent<Killzone>();
                break;
            case ShadowMeldObjectType.SPIKES:
                gameObject.layer = LayerMask.NameToLayer("Shadowmeld Death");
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

    public void UpdateShadowmeldRenderMode()
    {
        if (PlayerMovement.shadowMelded)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Shadowmeld Ignore") || gameObject.layer == LayerMask.NameToLayer("Shadowmeld Collide"))
            {
                if (GetComponent<ShadowCast>().singleMesh)
                {
                    GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer meshRend in meshRenderers)
                    {
                        meshRend.enabled = false;
                    }
                }
            }
        }
        else
        {
            if (GetComponent<ShadowCast>().singleMesh)
            {
                GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRend in meshRenderers)
                {
                    meshRend.enabled = true;
                }
            }
        }
    }

    public void UpdateShadowmeldVFX()
    {
        if (PlayerMovement.shadowMelded)
        {
            if (currentShadowmeldVFX == null)
            {
                if (gameObject.layer == LayerMask.NameToLayer("Shadowmeld Ignore"))
                {
                    currentShadowmeldVFX = Instantiate(shadowmeldInvisibleObjectVFX, gameObject.transform) as GameObject;
                }
                else if(gameObject.layer == LayerMask.NameToLayer("Shadowmeld Collide"))
                {
                    currentShadowmeldVFX = Instantiate(shadowmeldCollideObjectVFX, gameObject.transform) as GameObject;
                }
                else if(gameObject.layer == LayerMask.NameToLayer("Shadowmeld Death"))
                {
                    currentShadowmeldVFX = Instantiate(shadowmeldDeathObjectVFX, gameObject.transform) as GameObject;
                }
            }
        }
        else
            Destroy(currentShadowmeldVFX);
    }
}