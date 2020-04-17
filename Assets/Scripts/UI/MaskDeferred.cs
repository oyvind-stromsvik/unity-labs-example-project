using UnityEngine;
using System.Collections;

public class MaskDeferred : MonoBehaviour {
	void Start () {
		gameObject.GetComponent<Camera> ().clearStencilAfterLightingPass = true;
	}
}
