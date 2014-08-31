using UnityEngine;
using System.Collections;

public class Part : MonoBehaviour {
	
	public int maxhp = 3;
	public bool broken {
		get {
			return hp <= 0;
		}
	}
	public Sprite[] sprites;
	int hp;
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void respawn() {
		hp = maxhp;
		GetComponent<SpriteRenderer>().sprite = sprites[0];
	}
	
	void OnCollisionEnter2D(Collision2D col) {
		float velocity = Mathf.Abs(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal.normalized));
		
		Bullet bullet = col.contacts[0].collider.GetComponent<Bullet>();
		if(bullet != null) {
			hp -= 1;
			Debug.Log("1 Dmg (bullet)");
		}
		else if(velocity > 8) {
			hp -= 6;
			Debug.Log("6 Dmg");
		}
		else if(velocity > 5) {
			hp -= 4;
			Debug.Log("4 Dmg");
		}
		else if(velocity > 2) {
			hp -= 2;
			Debug.Log("2 Dmg");
		}
		else if(velocity > 1) {
			hp -= 1;
			Debug.Log("1 Dmg");
		}
		else {
			Debug.Log("0 Dmg");
		}
		
		if(hp < 0) {
			hp = 0;
		}
		
		
		int spriteIndex = Mathf.RoundToInt((sprites.Length-1) * (float)(maxhp-hp) / maxhp);
		if(spriteIndex == sprites.Length - 1 && hp > 0 && sprites.Length > 1)
			spriteIndex = sprites.Length - 2;
		
		GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
	}
}
