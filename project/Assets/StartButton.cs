using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {
	
	public Transform splitforcePrefab;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		Instantiate(splitforcePrefab);
	}
}
