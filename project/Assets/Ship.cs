using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	
	public float bulletSpeed = 2;
	public float shootInterval = 0.5f;
	public float force = 100;
    public float recoil = 1500;
    public int maxAmmo = 3;
    public float reloadTime = 3;
    public int score = 0;
	
	public string buttonLeft = "PL1Left";
	public string buttonRight = "PL1Right";
	public string buttonShoot = "PL1Shoot";
	public string buttonSelfDestroy = "PL1SelfDestroy";
	
	public Engine engineLeft;
	public Engine engineRight;
	public Part cockpit;
	public Part cannon;
	public TextMesh destructTimerText;
	public Transform bulletPrefab;
	public Transform spawnEffect;
    public Transform explosion;

    public Ship lastAttacker = null;

    float lastAttackerCooldown = 2.0f;

	float shootCooldown;
    float reloadProgress;
    int ammo;
	bool selfdestroying = false;
	float destructTimer;
	
	void Start () {
		respawn();
	}
	
	public void respawn() {
		shootCooldown = 0.0f;
		selfdestroying = false;
        reloadProgress = 0.0f;
        ammo = maxAmmo;
		
		Vector2 spawnPos = Vector2.zero;
		Ship[] ships = GameObject.FindObjectsOfType<Ship>();
		for(int i=0; i < 20; i++) { //try at most 20 times to avoid getting infinite loops
			SpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
			int spawnIndex = Random.Range(0, spawnPoints.Length);
			spawnPos = spawnPoints[spawnIndex].transform.position;
			
			bool shipNearby = false;
			foreach(Ship ship in ships) {
				if(ship != this && Vector2.Distance(ship.transform.position, spawnPos) < 2) {
					shipNearby = true;
					break;
				}
			}
			if(!shipNearby) {
				break;
			}
			//otherwise, try randomizing again.
		}
		transform.position = spawnPos;
		
		gameObject.SetActive(true);
		GameObject.Instantiate(spawnEffect, spawnPos, Quaternion.identity);
		
		foreach(Part part in GetComponentsInChildren<Part>(true)) {
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
		
		engineRight.GetComponentsInChildren<Particles>(true)[0].on = thrustingL;
        engineLeft.GetComponentsInChildren<Particles>(true)[0].on = thrustingR;
		
		if(thrustingL) {
			rigidbody2D.AddForceAtPosition(
				transform.up * force * Time.deltaTime, transform.TransformPoint(0.25f,0,0));
			if(!engineLeft.audio.isPlaying) {
				engineLeft.audio.Play();
			}
		}
		else {
			engineLeft.audio.Stop();
		}
		if(thrustingR) {
			rigidbody2D.AddForceAtPosition(
				transform.up * force * Time.deltaTime, transform.TransformPoint(-0.25f,0,0));
			if(!engineRight.audio.isPlaying) {
				engineRight.audio.Play();
			}
		}
		else {
			engineRight.audio.Stop();
		}
		
		//SHOOTING
		shootCooldown -= Time.deltaTime;
		if(Input.GetButton(buttonShoot) && !cannon.broken && ammo > 0) {
			if(shootCooldown <= 0) {
				shootCooldown = shootInterval;
                ammo--;
				
				Transform bulletObj = GameObject.Instantiate(bulletPrefab) as Transform;
				bulletObj.transform.position = transform.position + 0.4f * transform.up;
				bulletObj.rigidbody2D.velocity = rigidbody2D.velocity
											+ (Vector2)transform.up * bulletSpeed
											- 0.4f * (Vector2)transform.right * rigidbody2D.angularVelocity * Mathf.Deg2Rad;
				
                //apply recoil
                rigidbody2D.AddForceAtPosition(-transform.up * recoil * Time.deltaTime, transform.TransformPoint(0, 1, 0));

				//disable collision with self
				foreach(Collider2D ownCollider in GetComponentsInChildren<Collider2D>()) {
					Physics2D.IgnoreCollision(bulletObj.collider2D, ownCollider);
				}
                bulletObj.GetComponent<Bullet>().shooter = this;
			}
		}

        if (ammo <= 0)
        {
            reloadProgress += Time.deltaTime;
            if (reloadProgress > reloadTime)
            {
                reloadProgress = 0.0f;
                ammo = maxAmmo;
            }
        }

        //SCORING
        if (lastAttacker != null)
        {
            lastAttackerCooldown -= Time.deltaTime;
            if (lastAttackerCooldown < 0)
            {
                lastAttacker = null;
                lastAttackerCooldown = 2.0f;
            }
        }
		
		//SELF DESCTRUCTION
		if(!selfdestroying && Input.GetButtonDown(buttonSelfDestroy)) {
			selfdestroying = true;
			destructTimer = 3.0f;
		}
		
		if(selfdestroying) {
			destructTimer -= Time.deltaTime;
			destructTimerText.text = ""+(int)(destructTimer + 1.0f);
			destructTimerText.transform.rotation = Quaternion.identity;
			destructTimerText.transform.position = (Vector2)transform.position + 0.9f * Vector2.up;
		}
		else {
			destructTimerText.text = "";
		}

        bool selfDestroyed = (selfdestroying && destructTimer <= 0.0f);
        if (selfDestroyed)
        {
            score--;
        }

		//DEATH
        bool dead = cockpit.broken || selfDestroyed;
		if(dead) {
            if (lastAttacker)
            {
                Ship attackerShip = lastAttacker.GetComponent<Ship>();
                if (attackerShip)
                {
                    attackerShip.score++;
                }
            }

            Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
			gameObject.SetActive(false);
			
			foreach(ParticleSystem particle in GetComponentsInChildren<ParticleSystem>(true)) {
				particle.Clear();
			}
			
			Invoke("respawn", 2);
		}
	}
}
