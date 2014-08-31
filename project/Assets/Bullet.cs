using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public Transform dinkEffect;
	public Transform bulletSound;
	
    public Ship shooter;
	// Use this for initialization
	void Start () {
		Instantiate(bulletSound);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter2D(Collision2D col) {
        Part part = col.contacts[0].collider.GetComponent<Part>();
        if (part != null)
        {
            Instantiate(dinkEffect, transform.position, Quaternion.identity);
        }
		Destroy(gameObject);
	}
}
