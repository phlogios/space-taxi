using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	
	public float bulletSpeed = 2;
	public float shootInterval = 0.5f;
    public float recoil = 1500;
    public int maxAmmo = 3;
    public float reloadTime = 3;
    public int score = 0;
	
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
		if(networkView.isMine)
			gameObject.name = "Ship(Local)";
		else
			gameObject.name = "Ship(Remote)";
		
		respawn();
	}
	
	public void respawn() {
        lastAttacker = null;
        lastAttackerCooldown = 2.0f;
		shootCooldown = 0.0f;
		selfdestroying = false;
        reloadProgress = 0.0f;
        ammo = maxAmmo;
		
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		
		Vector3 spawnPos = Vector3.zero;
		Ship[] ships = GameObject.FindObjectsOfType<Ship>();
		for(int i=0; i < 20; i++) { //try at most 20 times to avoid getting infinite loops
			SpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
			int spawnIndex = Random.Range(0, spawnPoints.Length);
			spawnPos = spawnPoints[spawnIndex].transform.position;
			
			bool shipNearby = false;
			foreach(Ship ship in ships) {
				if(ship != this && Vector3.Distance(ship.transform.position, spawnPos) < 2) {
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
		
		if(networkView.isMine) {
			Network.Instantiate(spawnEffect, spawnPos, Quaternion.identity, 0);
		}
		
		foreach(Part part in GetComponentsInChildren<Part>(true)) {
			part.respawn();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
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
		
		// ROTATE TIMER TEXT
		destructTimerText.transform.rotation = Quaternion.identity;
		destructTimerText.transform.position = transform.position + 0.9f * Vector3.up;
		
		////////////////////////////////
		// LOCAL ONLY AFTER HERE
		////////////////////////////////
		if(!networkView.isMine)
			return;
		
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

        if (pressingL)
        {
            
            engineRight.Thrust();
        }

        if (pressingR)
        {
            engineLeft.Thrust();
        }
		
		
		//SHOOTING
		shootCooldown -= Time.deltaTime;
		if(Input.GetButton(buttonShoot) && !cannon.broken && ammo > 0) {
			if(shootCooldown <= 0) {
				shootCooldown = shootInterval;
                ammo--;
				
				Transform bulletObj = Network.Instantiate(bulletPrefab, transform.position + 0.4f * transform.up, Quaternion.identity, 0) as Transform;
				bulletObj.rigidbody.velocity = rigidbody.velocity
											+ transform.up * bulletSpeed
											- 0.4f * transform.right * rigidbody.angularVelocity.z * Mathf.Deg2Rad;
				
                //apply recoil
                rigidbody.AddForceAtPosition(-transform.up * recoil * Time.deltaTime, transform.TransformPoint(0, 1, 0));
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
		
		//SELF DESCTRUCTION
		if(!selfdestroying && Input.GetButtonDown(buttonSelfDestroy)) {
			selfdestroying = true;
			destructTimer = 3.0f;
		}
		
		if(selfdestroying) {
			destructTimer -= Time.deltaTime;
			if(destructTimer >= 0)
				destructTimerText.text = ""+(int)(destructTimer + 1.0f);
			else
				destructTimerText.text = ""; //needed for beep check
		}
		else {
			destructTimerText.text = "";
		}
		if(prevDestructTimerText != destructTimerText.text) {
			networkView.RPC("setDestructTimerText", RPCMode.Others, destructTimerText.text);
			beep();
		}
		prevDestructTimerText = destructTimerText.text;
		
        //DEATH
        bool selfDestroyed = (selfdestroying && destructTimer <= 0.0f);
		bool dead = cockpit.broken || selfDestroyed;
		if(dead) {
			networkView.RPC("death", RPCMode.All);
		}
	}
	
	void OnCollisionEnter(Collision col) {
		//forward to part
		col.contacts[0].thisCollider.GetComponent<Part>().collideCallback(col);
	}
	
	[RPC]
	public void death() {Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
		gameObject.SetActive(false);
	
        bool selfDestroyed = (selfdestroying && destructTimer <= 0.0f);
		if (selfDestroyed)
        {
            score--;
        }
        if (lastAttacker)
        {
            Ship attackerShip = lastAttacker.GetComponent<Ship>();
            if (attackerShip && !selfDestroyed)
            {
                attackerShip.score++;
            }
        }
		
		foreach(ParticleSystem particle in GetComponentsInChildren<ParticleSystem>(true)) {
			particle.Clear();
		}
		
		Invoke("respawn", 2);
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
