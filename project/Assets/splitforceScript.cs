using UnityEngine;
using System.Collections;

public class splitforceScript : MonoBehaviour {

	void  SplitforceInitialised(bool isFailed, Hashtable additionalData) {
		UnitySplitForce.SFVariation v = UnitySplitForce.SFManager.Instance.initExperiment("testExperiment");

		// Proceed with game steps
		
		// You can check if everything is correct
		if (isFailed) {
			if (additionalData.ContainsKey("errorMessage")) {
				// Log message
			}
		}
	}

	void Awake () {
		UnitySplitForce.SFManager.Instance.initCallback = SplitforceInitialised;
		UnitySplitForce.SFManager.Init ("pkamluvpgi", "ljxxrehgcwnwyjypgbfweknbbwbllgorzdgwahfmuwapkusunw", new Hashtable () {
			{"isDebug", true}
		});
	}


	// Use this for initialization
	void Start () {
		UnitySplitForce.SFVariation v = UnitySplitForce.SFManager.Instance.initExperiment("testExperiment");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
