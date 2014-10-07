using UnityEngine;
using System.Collections;

public class splitforceScript : MonoBehaviour {

	public static string redirectToScene = "ingame";
	public static splitforceScript instance = null;
	
	void  SplitforceInitialised(bool isFailed, Hashtable additionalData) {
		// Proceed with game steps
		Debug.Log("Initing splitforce");
		UnitySplitForce.SFVariation v = UnitySplitForce.SFManager.Instance.initExperiment("testExperiment");
		if (v != null) {
			Debug.Log("Successfully inited splitforce");
		}
		// You can check if everything is correct
		if (isFailed) {
			if (additionalData.ContainsKey("errorMessage")) {
				// Log message
				Debug.Log("Failed to init splitforce");
			}
		}
		
		Application.LoadLevel(redirectToScene);
	}

	void Awake () {
		if(instance != null) {
			Destroy(gameObject);
			return;
		}
		
		instance = this;
		DontDestroyOnLoad(gameObject);
		
		UnitySplitForce.SFManager.Instance.initCallback = SplitforceInitialised;
		UnitySplitForce.SFManager.Init ("imrentqvlj", "piqdyhlomdwbkazjmxfwnmnwrefeuclhpluolnxdaythdsclwa", new Hashtable () {
			{"isDebug", false}
		});
	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
