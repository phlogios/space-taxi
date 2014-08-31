using UnityEngine;
using System.Collections;

public class Part : MonoBehaviour {
	public float forceLimit;
	public int maxhp = 3;
	public bool broken {
		get {
			return hp > 0;
		}
	}
	int hp;
	
	void Awake () {
		respawn ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void respawn() {
		//TODO: Randomize position
		hp = maxhp;
	}
	
	void OnCollisionEnter2D(Collision2D col) {
		float velocity = Mathf.Abs(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal.normalized));
		
		if(velocity > 10) {
			hp -= 3;
			Debug.Log("3 dmg");
		}
		else if(velocity > 5) {
			hp -= 2;
			Debug.Log("2 dmg");
		}
		else if(velocity > 1) {
			hp -= 1;
			Debug.Log("1 dmg");
		}
		else {
			Debug.Log("0 dmg");
		}
		
		if(hp < 0) {
			hp = 0;
		}
	}
}
