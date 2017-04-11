using UnityEngine;
using System.Collections;

public class ToadCamoScript : MonoBehaviour {
	public Renderer rend;
	void Start() {
		rend = GetComponent<Renderer>();
	}
	void Update() {
		float scaleX = Mathf.Cos(Time.time) * 0.75F + 1;
		float scaleY = Mathf.Sin(Time.time) * 0.75F + 1;
		rend.material.mainTextureScale = new Vector2(scaleX, scaleY);
	}
}