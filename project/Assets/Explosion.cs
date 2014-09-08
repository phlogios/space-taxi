using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
    public float force;
    public Ship owner;

	// Use this for initialization
	void Start () {
        Rigidbody2D[] rigidbodies = GameObject.FindObjectsOfType<Rigidbody2D>();
        Debug.Log(force);
        foreach(Rigidbody2D body in rigidbodies) {
            Vector2 direction = -(transform.position - body.transform.position);
            body.AddForce(direction.normalized * force * Mathf.Min(1.0f / direction.magnitude, 1.0f));
            if(owner && Vector3.Magnitude(body.transform.position - transform.position) < 8.0f) {
                Ship ship = body.GetComponent<Ship>();
                if (ship)
                {
                    ship.lastAttacker = owner;
                }
                            
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
