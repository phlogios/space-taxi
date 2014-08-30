using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ThrusterFX : MonoBehaviour {
	
	public bool on = false;
	
	// Use this for initialization
	void Start () {
		particleSystem.renderer.sortingLayerName = "Effects";
	}
	
	// Update is called once per frame
	void Update () {
		particleSystem.enableEmission = on;
	}
}
