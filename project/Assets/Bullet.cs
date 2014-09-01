using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public Transform dinkEffect;
	public Transform bulletSound;
	
    public Ship shooter;
	// Use this for initialization
	
	void Awake() {
		//TODO: use RPC call and pass some user ID of the shooter. Does not work to send "this" by RPC.
		if(networkView.isMine) {
			init(GameObject.Find("Ship(Local)").GetComponent<Ship>());
		}
		else {
			init(GameObject.Find("Ship(Remote)").GetComponent<Ship>());
		}
	}
	
	void Start () {
		Instantiate(bulletSound);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	[RPC]
	public void init(Ship _shooter) {
		
		//disable collision with self
		foreach(Collider shooterCollider in _shooter.GetComponentsInChildren<Collider>()) {
			Physics.IgnoreCollision(collider, shooterCollider);
		}
        
		shooter = _shooter;	
	}
	
	void OnCollisionEnter(Collision col) {
        Part part = col.contacts[0].otherCollider.GetComponent<Part>();
        if (part != null)
        {
            Instantiate(dinkEffect, transform.position, Quaternion.identity);
        }
		Destroy(gameObject);
	}
}
