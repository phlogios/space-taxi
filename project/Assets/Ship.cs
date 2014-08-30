using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	public float force;
	public Engine engineLeft;
	public Engine engineRight;
	public Part cockpit;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bool thrustingL = Input.GetKey(KeyCode.LeftArrow) && !engineLeft.GetComponent<Part>().broken;
		bool thrustingR = Input.GetKey(KeyCode.RightArrow) && !engineRight.GetComponent<Part>().broken;
		
		engineLeft.GetComponentInChildren<ThrusterFX>().on  = thrustingL;
		engineRight.GetComponentInChildren<ThrusterFX>().on = thrustingR;
		
		if(thrustingL) {
			rigidbody2D.AddForceAtPosition(
				transform.up * force * Time.deltaTime, transform.TransformPoint(0.25f,0,0));
		}
		if(thrustingR) {
			rigidbody2D.AddForceAtPosition(
				transform.up * force * Time.deltaTime, transform.TransformPoint(-0.25f,0,0));
		}
		
		if(cockpit.broken) {
			Destroy(gameObject);
		}
	}
}
