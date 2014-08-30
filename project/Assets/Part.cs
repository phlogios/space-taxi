using UnityEngine;
using System.Collections;

public class Part : MonoBehaviour {
	public float forceLimit;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter2D(Collision2D col) {
		float velocity = Mathf.Abs(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal.normalized));
		if(velocity > forceLimit) {
			Debug.Log("AJJ! "+velocity);
		}
	}
}
