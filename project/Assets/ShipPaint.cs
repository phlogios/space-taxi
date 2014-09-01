using UnityEngine;
using System.Collections;

public class ShipPaint : MonoBehaviour {

	void Awake () {
		if(!networkView.isMine) {
			GetComponent<SpriteRenderer>().color = new Color(0.8f,0.1f,0.1f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
