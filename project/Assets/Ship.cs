using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	
	public float bulletSpeed = 2;
	public float shootInterval = 0.5f;
    public float recoil = 1500;
    public int maxAmmo = 3;
    public float reloadTime = 3;
    public int score = 0;
	public int accidents = 0;
	
	public string buttonLeft = "PL1Left";
	public string buttonRight = "PL1Right";
	public string buttonShoot = "PL1Shoot";
	public string buttonSelfDestroy = "PL1SelfDestroy";
	
	public Thruster engineLeft;
	public Thruster engineRight;
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
	string prevDestructTimerText;
	
	void Start () {
		accidents = 0;
		respawn();
	}
	
	public void respawn() {
        lastAttacker = null;
        lastAttackerCooldown = 2.0f;
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
        bool shooting = false;
		foreach(Touch touch in Input.touches) {
			if(touch.position.x / Screen.width > 0.5f && touch.position.y / Screen.height < 0.5f) {
				pressingR = true;	
			}
            else if (touch.position.x / Screen.width < 0.5f && touch.position.y / Screen.height < 0.5f)
            {
				pressingL = true;
			}
            if (touch.position.y / Screen.height > 0.5f)
            {
                shooting = true;
            }
      	}

        if (pressingL)
        {
            
            engineRight.Thrust();
        }

        if (pressingR)
        {
            engineLeft.Thrust();
        }
		
		
		//SHOOTING
        if (Input.GetButton(buttonShoot))
            shooting = true;
		shootCooldown -= Time.deltaTime;
		if(shooting && !cannon.broken && ammo > 0) {
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
			if(destructTimer >= 0) {
				destructTimerText.text = ""+(int)(destructTimer + 1.0f);
				destructTimerText.transform.rotation = Quaternion.identity;
				destructTimerText.transform.position = transform.position + Vector3.up * 0.9f;
			}
			else {
				destructTimerText.text = ""; //needed for beep check
			}
		}

		if(prevDestructTimerText != destructTimerText.text) {
			beep();
		}
		prevDestructTimerText = destructTimerText.text;
		
        //DEATH
        bool selfDestroyed = (selfdestroying && destructTimer <= 0.0f);
		//DEATH
        bool dead = cockpit.broken || selfDestroyed;
		if(dead) {
            if (lastAttacker)
            {
                Ship attackerShip = lastAttacker.GetComponent<Ship>();
                if (attackerShip && !selfDestroyed)
                {
                    attackerShip.score++;
                }
            } else {
				accidents++;
			}

            Transform explosionObject = Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, -2.0f), Quaternion.identity) as Transform;
            if (selfDestroyed)
            {
                Explosion e = explosionObject.GetComponent<Explosion>();
                e.force = 400.0f;
                e.owner = transform.GetComponent<Ship>();
            }
			gameObject.SetActive(false);
			
			foreach(ParticleSystem particle in GetComponentsInChildren<ParticleSystem>(true)) {
				particle.Clear();
			}
			
			Invoke("respawn", 2);
		}
	}
	
	void beep() {
		if(destructTimerText.text != "")
			audio.Play();
	}
	
	[RPC]
	public void setDestructTimerText(string newtext) {
		destructTimerText.text = newtext;
		beep();
	}
}
