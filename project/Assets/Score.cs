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
		restartGame();
	}

	void restartGame() {
		playtime = 0;
		player1.respawn();
		player2.respawn();
		player1.score = 0;
		player2.score = 0;
		player1.accidents = 0;
		player2.accidents = 0;
	}
	
	// Update is called once per frame
	void Update () {
		playtime += Time.deltaTime;
        player1Score.text = ""+player1.score;
        player2Score.text = ""+player2.score;

		if (player1.score >= 2 || player2.score >= 2) {
			UnitySplitForce.SFVariation v = UnitySplitForce.SFManager.Instance.initExperiment("testExperiment");
			
			if (v != null) {
				
				v.trackQuantifiedGoal("accidents", player1.accidents + player2.accidents);
				
				v.trackTime("playtime", playtime);
				
				v.endVariation();
			}
			else {
				Debug.Log ("Failed to get variation");
			}
			restartGame();
		}
	}
}
