using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

    public Ship player1;
    public Ship player2;

    public TextMesh player1Score;
    public TextMesh player2Score;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //player1Score.text = ""+player1.score;
        //player2Score.text = ""+player2.score;
		
		if(!player1) {
			GameObject ship = GameObject.Find("Ship(Local)");
			player1 = ship.GetComponent<Ship>();
		}
		if(player1)
			player1Score.text = ""+player1.score;
		
		player2Score.text = "";
	}
}
