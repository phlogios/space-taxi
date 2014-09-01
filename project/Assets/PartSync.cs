using UnityEngine;
using System.Collections;

public class PartSync : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		int hp = GetComponent<Part>().getHP();
		stream.Serialize(ref hp);
		
		if(stream.isReading) {
			GetComponent<Part>().setHP(hp);
		}
	}
}
