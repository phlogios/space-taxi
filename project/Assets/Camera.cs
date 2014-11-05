using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {
	public Transform ship1;
	public Transform ship2;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = ship1.transform.position + (ship2.transform.position - ship1.transform.position) * 0.5f;
		transform.position = new Vector3 (transform.position.x, transform.position.y, -10.0f);
	}
}
