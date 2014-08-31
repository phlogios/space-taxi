using UnityEngine;
using System.Collections;

public class ParticleAutoDestroy : MonoBehaviour {
	
	float age = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		age += Time.deltaTime;
		if(age > particleSystem.duration)
			Destroy (gameObject);
	}
}
