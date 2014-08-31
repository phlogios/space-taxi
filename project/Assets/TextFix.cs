using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextFix : MonoBehaviour {

	// Use this for initialization
	void Start () {
		renderer.sortingLayerName = "GUI";
	}
}
