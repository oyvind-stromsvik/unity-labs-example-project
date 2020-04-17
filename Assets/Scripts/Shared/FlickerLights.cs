using UnityEngine;
using System.Collections;

public class FlickerLights : MonoBehaviour {

	public float speedY;
	public float speedX;

	public int materialID;

	private Renderer r;
	private Material[] mats;

	// Use this for initialization
	void Start () {
		r = gameObject.GetComponentInChildren<Renderer> ();
		mats = r.materials;
	}
	
	// Update is called once per frame
	void Update () {
		mats [materialID].mainTextureOffset += new Vector2 (speedX, speedY);
		//r.material.mainTextureOffset += new Vector2 (speedX, speedY);
	}
}
