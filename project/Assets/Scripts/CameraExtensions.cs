using UnityEngine;
using System.Collections;

public class CameraExtensions : MonoBehaviour {
	public Transform ship1;
	public Transform ship2;

	public float mapX = 18.0f;
	public float mapY = 10.0f;

	private float minX;
	private float maxX;
	private float minY;
	private float maxY;

	// Use this for initialization
	void Start () {
		float vertExtent = camera.orthographicSize;
		float horzExtent = vertExtent * Screen.width / Screen.height;

		minX = horzExtent - mapX / 2.0f;
		maxX = mapX / 2.0f - horzExtent;
		minY = vertExtent - mapY / 2.0f;
		maxY = mapY / 2.0f - vertExtent;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = ship1.transform.position + (ship2.transform.position - ship1.transform.position) * 0.5f;
		transform.position = new Vector3 (transform.position.x, transform.position.y, -10.0f);

		Vector3 v3 = transform.position;
		v3.x = Mathf.Clamp(v3.x, minX, maxX);
		v3.y = Mathf.Clamp(v3.y, minY, maxY);
		transform.position = v3;
	}
}
