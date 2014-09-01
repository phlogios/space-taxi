using UnityEngine;
using System.Collections;

public class SpawnShip : MonoBehaviour {
	
	public Transform shipPrefab;
	
	// Use this for initialization
	void Awake () {
		if(Network.connections.Length == 0)
			Application.LoadLevel("connect");
		Network.Instantiate(shipPrefab, Vector3.zero, Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if(Application.loadedLevelName != "connect" && Network.connections.Length == 0)
			Application.LoadLevel("connect");
	}
}
