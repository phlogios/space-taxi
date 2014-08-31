using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	
	public float bulletSpeed = 2;
	public float shootInterval = 0.5f;
	public float force;
	
	public string buttonLeft = "PL1Left";
	public string buttonRight = "PL1Right";
	public string buttonShoot = "PL1Shoot";
	
	public Engine engineLeft;
	public Engine engineRight;
	public Part cockpit;
	public Part cannon;
	public Transform bulletPrefab;
	public Transform spawnEffect;
	
	Vector2 spawnPos;
	float shootCooldown;
	
	void Awake () {
		spawnPos = transform.position;
		respawn();
	}
	
	public void respawn() {
		shootCooldown = 0.0f;
		
		//TODO: Randomize position
		transform.position = spawnPos;
		gameObject.SetActive(true);
		GameObject.Instantiate(spawnEffect, spawnPos, Quaternion.identity);
		
		foreach(Part part in GetComponentsInChildren<Part>()) {
			part.respawn();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		//MOVEMENT
		
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
		
		engineRight.GetComponentInChildren<Particles>().on  = thrustingL;
		engineLeft.GetComponentInChildren<Particles>().on = thrustingR;
		
		if(thrustingL) {
			rigidbody2D.AddForceAtPosition(
				transform.up * force * Time.deltaTime, transform.TransformPoint(0.25f,0,0));
		}
		if(thrustingR) {
			rigidbody2D.AddForceAtPosition(
				transform.up * force * Time.deltaTime, transform.TransformPoint(-0.25f,0,0));
		}
		
		//SHOOTING
		shootCooldown -= Time.deltaTime;
		if(Input.GetButton(buttonShoot) && !cannon.broken) {
			if(shootCooldown <= 0) {
				shootCooldown = shootInterval;
				
				Transform bulletObj = GameObject.Instantiate(bulletPrefab) as Transform;
				bulletObj.transform.position = transform.position;
				bulletObj.rigidbody2D.velocity = rigidbody2D.velocity
											+ (Vector2)transform.up * bulletSpeed;
				
				//disable collision with self
				foreach(Collider2D ownCollider in GetComponentsInChildren<Collider2D>()) {
					Physics2D.IgnoreCollision(bulletObj.collider2D, ownCollider);
				}
			}
		}
		
		//DEATH
		if(cockpit.broken) {
			gameObject.SetActive(false);
			Invoke("respawn", 2);
		}
	}
}
