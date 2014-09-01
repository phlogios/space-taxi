using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour {
    public float force = 100;
    public bool catastrophe = false;
	
	bool thrusting = false;
    bool broken;

	// Use this for initialization
	void OnEnable () {
		thrusting = false;
        catastrophe = false;
        broken = false;
	}
	
	// Update is called once per frame
    void Update()
    {
        broken = transform.GetComponent<Part>().broken;

        if (networkView.isMine && transform.GetComponent<Part>().brokeThisFrame)
        {
            catastrophe = true;//DEBUG Random.Range(0.0f, 1.0f) < 0.4f;
        }

        transform.GetComponentInChildren<Particles>().on = false;
        if (thrusting)
        {
            transform.GetComponentInChildren<Particles>().on = true;
			
			if(networkView.isMine) {
	            GetComponentInParent<Rigidbody>().AddForceAtPosition(
	            	GetComponentInParent<Transform>().up * force * Time.deltaTime, transform.TransformPoint(0, 0, 0));
			}
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        }
        else
        {
            audio.Stop();
        }
	}

    public void Thrust()
    {
		if(networkView.isMine) {
	        if (!broken)
	        {
	            thrusting = true;
	        }
        	//Debug.Log("Thrusting!");
		}
    }
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		bool _thrusting = thrusting;
		stream.Serialize(ref _thrusting);
		
		if(stream.isReading)
			thrusting = _thrusting;
		
		if(networkView.isMine) {
        	if (!catastrophe)
        	{
				thrusting = false;            
        	}
		}
	}
}
