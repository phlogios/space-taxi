using UnityEngine;
using System.Collections;

public class SplitforceTest : MonoBehaviour {
	void test() {
		PlayerPrefs.DeleteAll();
		
		// Proceed with game steps
		Debug.Log("Initing splitforce");
		UnitySplitForce.SFVariation v = UnitySplitForce.SFManager.Instance.initExperiment("testExperiment");
		if (v != null) {
			Debug.Log("Successfully inited splitforce");
		}
		
		//Test if fetching data works
		float thrusterPower = v.VariationData("Thruster power").DataToFloat(-1);
		if(thrusterPower == -1)
			Debug.LogError("Fetching variation data failed");
		else
			Debug.Log("Successfully fetched data: thrusterPower="+thrusterPower);
		
		v.endVariation();
	}
	
	void  SplitforceInitialised(bool isFailed, Hashtable additionalData) {
		// You can check if everything is correct
		if (isFailed) {
			if (additionalData.ContainsKey("errorMessage")) {
				// Log message
				Debug.LogError("Failed to init splitforce");
			}
		}
		
		test();
	}

	void Awake () {
		reset();
	}
	
	void OnMouseDown() {
		reset();
	}
	
	void reset() {
		UnitySplitForce.SFManager.Instance.initCallback = SplitforceInitialised;
		UnitySplitForce.SFManager.Init ("imrentqvlj", "piqdyhlomdwbkazjmxfwnmnwrefeuclhpluolnxdaythdsclwa", new Hashtable () {
			{"isDebug", false},
			{"skipCache", true}
		});
	}
}
