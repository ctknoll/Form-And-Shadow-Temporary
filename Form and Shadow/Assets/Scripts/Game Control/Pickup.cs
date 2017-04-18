using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour 
{
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
            StartCoroutine(GrabPickup());
		}
	}
    IEnumerator GrabPickup()
    {
        GetComponent<AudioSource>().Play();
        GetComponent<ShadowCast>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GameObject.Find("Game_Controller").GetComponent<GameController>().ScoreIncrement(100);
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        Destroy(gameObject);
    }
}