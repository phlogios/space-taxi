using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

    public Ship player1;
    public Ship player2;

    public TextMesh player1Score;
    public TextMesh player2Score;

	public float playtime;

	// Use this for initialization
	void Start () {
		playtime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		playtime += Time.deltaTime;
        player1Score.text = ""+player1.score;
        player2Score.text = ""+player2.score;

		if (player1.score >= 5 || player2.score >= 5) {
			UnitySplitForce.SFVariation v = UnitySplitForce.SFManager.Instance.getExperiment("testExperiment");
			
			if (v != null) {
				
				v.trackQuantifiedGoal("accidents", player1.accidents + player2.accidents);
				
				v.trackTime("playtime", playtime);
				
				v.endVariation();
			}
		}
	}
}
