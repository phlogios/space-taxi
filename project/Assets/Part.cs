using UnityEngine;
using System.Collections;

public class Part : MonoBehaviour {
	
	public int maxHP = 3;
    public int explodeHP = -5;
	public Transform partExplosion;

    public bool brokeThisFrame;
    public bool detachChildrenOnDeath;

	public bool broken {
		get {
			return hp <= 0;
		}
	}
	public Sprite[] sprites;
	int hp;
    bool brokenLastFrame = false;
	
	// Update is called once per frame
	void Update () {
        brokeThisFrame = false;
        if (broken && !brokenLastFrame)
        {
            brokeThisFrame = true;
        }
        brokenLastFrame = broken;
	}
	
	public void respawn() {
		hp = maxHP;
		GetComponent<SpriteRenderer>().sprite = sprites[0];
		Destroy (GetComponent<Rigidbody2D> ());
		Destroy (null);
		gameObject.SetActive(true);
	}

	public void explode() {
		gameObject.SetActive(false);
		GameObject.Instantiate(partExplosion, transform.position, Quaternion.identity);
	}

	void OnCollisionEnter2D(Collision2D col) {
		float velocity = Mathf.Abs(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal.normalized));
		
		Bullet bullet = col.contacts[0].collider.GetComponent<Bullet>();
        Part otherPart = col.contacts[0].collider.GetComponent<Part>();
        if (otherPart != null && transform.parent)
        {
			Ship otherShip = otherPart.GetComponentInParent<Ship>();
			Ship myShip = transform.GetComponentInParent<Ship>();
			if(otherShip) {
            	myShip.lastAttacker = otherPart.GetComponentInParent<Ship>();
            	Debug.Log("Attacked!");
			}
        }

		if(bullet != null) {
			hp -= (int) bullet.damage;
			Ship myShip = transform.GetComponentInParent<Ship>();
			if(myShip) {
            	myShip.lastAttacker = bullet.shooter;
			}
			Destroy (bullet.gameObject);
			//Debug.Log("1 Dmg (bullet)");
		}
		else if(velocity > 5) {
			hp -= 6;
			//Debug.Log("6 Dmg");
		}
		else if(velocity > 2.5f) {
			hp -= 5;
			//Debug.Log("4 Dmg");
		}
		else if(velocity > 1.4f) {
			hp -= 2;
			//Debug.Log("2 Dmg");
		}
		else if(velocity > 1) {
			hp -= 1;
			//Debug.Log("1 Dmg");
		}
		else {
			//Debug.Log("0 Dmg");
		}


		if(hp <= explodeHP) {
			if(detachChildrenOnDeath) {
				Part[] childParts = transform.GetComponentsInChildren<Part>();
				foreach(Part childPart in childParts) {
					Rigidbody2D rg = childPart.gameObject.AddComponent("Rigidbody2D") as Rigidbody2D;
				}
				transform.DetachChildren();
			}
			explode();
		}
		
		
		int cappedHP = Mathf.Max(0, hp);
		int spriteIndex = Mathf.RoundToInt((sprites.Length-1) * (float)(maxHP-cappedHP) / maxHP);
		if(spriteIndex == sprites.Length - 1 && hp > 0 && sprites.Length > 1)
			spriteIndex = sprites.Length - 2;
		
		GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
	}
}
