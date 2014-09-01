using UnityEngine;
using System.Collections;

public class Part : MonoBehaviour {
	
	public int maxhp = 3;
    public int explodehp = -5;
	public Transform partExplosion;

    public bool brokeThisFrame;

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
		brokenLastFrame = false;
		hp = maxhp;
		GetComponent<SpriteRenderer>().sprite = sprites[0];
		gameObject.SetActive(true);
	}
	
	public void collideCallback(Collision col) {
		if(!networkView.isMine)
			return;
		
		float velocity = Mathf.Abs(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal.normalized));
		
		Bullet bullet = col.contacts[0].otherCollider.GetComponent<Bullet>();
        Part otherShip = col.contacts[0].otherCollider.GetComponent<Part>();
        if (otherShip != null)
        {
            transform.parent.GetComponent<Ship>().lastAttacker = otherShip.GetComponentInParent<Ship>();
            Debug.Log("Attacked!");
        }

		if(bullet != null) {
			hp -= 1;
            transform.parent.GetComponent<Ship>().lastAttacker = bullet.shooter;
			Debug.Log("1 Dmg (bullet)");
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

		setHP(hp);
	}
	
	public int getHP() {
		return hp;
	}
	
	public void setHP(int _hp) {
		hp = _hp;
		
		if(hp <= explodehp) {
			if(networkView.isMine) {
				networkView.RPC("explodePart", RPCMode.Others);
			}
			explodePart();
		}
		
		int cappedHP = Mathf.Max(0, hp);
		int spriteIndex = Mathf.RoundToInt((sprites.Length-1) * (float)(maxhp-cappedHP) / maxhp);
		if(spriteIndex == sprites.Length - 1 && hp > 0 && sprites.Length > 1)
			spriteIndex = sprites.Length - 2;
		
		GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
	}
	
	[RPC]
	public void explodePart() {
		gameObject.SetActive(false);
		GameObject.Instantiate(partExplosion, transform.position, Quaternion.identity);
	}
}
