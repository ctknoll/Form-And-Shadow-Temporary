using UnityEngine;
using System.Collections;

public class ToadCamoScript : MonoBehaviour {
	public Renderer rend;
	void Start() {
		rend = GetComponent<Renderer>();
	}
	void Update() {
		float scaleX = Mathf.Cos(GameController.masterTimer) * 0.75F + 1;
		float scaleY = Mathf.Sin(GameController.masterTimer) * 0.75F + 1;
		rend.material.mainTextureScale = new Vector2(scaleX, scaleY);
	}
}