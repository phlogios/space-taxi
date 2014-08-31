using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
    public float force = 3000;

	// Use this for initialization
	void Start () {
        Rigidbody2D[] rigidbodies = GameObject.FindObjectsOfType<Rigidbody2D>();
        foreach(Rigidbody2D body in rigidbodies) {
            Vector2 direction = -(transform.position - body.transform.position);
            body.AddForce(direction.normalized * force * Mathf.Min(1.0f / direction.magnitude, 1.0f));
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
