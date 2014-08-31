using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	
	public string buttonLeft = "PL1Left";
	public string buttonRight = "PL1Right";
	public string buttonShoot = "PL1Shoot";
	
	public float force;
	public Engine engineLeft;
	public Engine engineRight;
	public Part cockpit;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bool pressingL = Input.GetButton(buttonLeft);
		bool pressingR = Input.GetButton(buttonRight);
		foreach(Touch touch in Input.touches) {
			if(touch.position.x / Screen.width > 0.5f) {
				pressingR = true;	
			}
			else {
				pressingL = true;
			}
		}
		
		bool thrustingL = pressingL && !engineRight.GetComponent<Part>().broken;
		bool thrustingR = pressingR && !engineLeft.GetComponent<Part>().broken;
		
		engineRight.GetComponentInChildren<ThrusterFX>().on  = thrustingL;
		engineLeft.GetComponentInChildren<ThrusterFX>().on = thrustingR;
		
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
