using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		if (player1.score >= 5 || player2.score >= 5) {
			//C#
			JSONObject postData = new JSONObject(JSONObject.Type.OBJECT);
			postData.AddField("build", "0.1");
			postData.AddField("clientTimestamp", "2012-03-14T02:33:42.416587-07:00");
			postData.AddField("device", "Development machine");
			postData.AddField("eventType", "battle.won");
			postData.AddField("userId", 3133731337);
			postData.AddField("userName", "developer");
			postData.AddField("productKey", "8b0f4ec7-656c-11e4-acb0-b8e8563b3f9a");
			postData.AddField("sessionId", "fff324c6-30a2-11e3-ad80-485d60066bda");

			JSONObject eventData = new JSONObject(JSONObject.Type.OBJECT);
			eventData.AddField("winner", "Gustav");
			eventData.AddField("loser", "Robin");
			postData.AddField("data", eventData);

			Debug.Log(postData.Print());

			string ourPostData = "{\"someJSON\":42}";
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Content-Type", "application/json");
			headers.Add("Cookie", "Our session cookie");
			headers.Add("Remote-Address", "1.2.3.4");
			byte[] pData = System.Text.Encoding.UTF8.GetBytes(postData.Print());
			WWW www = new WWW("http://api.traintracks.io/v1/events", pData, headers);


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
