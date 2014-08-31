using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour {
    public float force = 100;
    public bool catastrophe = false;

    bool thrusting = false;

    bool broken;

	// Use this for initialization
	void OnEnable () {
        catastrophe = false;
        broken = false;
	}
	
	// Update is called once per frame
    void Update()
    {
        broken = transform.GetComponent<Part>().broken;

        if (transform.GetComponent<Part>().brokeThisFrame)
        {
            catastrophe = Random.Range(0.0f, 1.0f) < 0.4f;
        }

        transform.GetComponentInChildren<Particles>().on = false;
        if (thrusting)
        {
            transform.GetComponentInChildren<Particles>().on = true;

            GetComponentInParent<Rigidbody2D>().AddForceAtPosition(
            GetComponentInParent<Transform>().up * force * Time.deltaTime, transform.TransformPoint(0, 0, 0));
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        }
        else
        {
            audio.Stop();
        }

        if (!catastrophe)
        {
            thrusting = false;            
        }
	}

    public void Thrust()
    {
        if (!broken)
        {
            thrusting = true;
        }
        Debug.Log("Thrusting!");
    }
}
