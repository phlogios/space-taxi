﻿using UnityEngine;
using System.Collections;
using System.IO;

public class NetworkManager : MonoBehaviour {
	
	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(gameObject);
		
		StreamReader cfg = new StreamReader("network.txt");
		string ip = cfg.ReadLine();
		//string ip = "80.78.215.45";//cfg.ReadLine();
		if(ip == null || ip.Trim() == "")
			host();
		else
			join(ip);
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Application.loadedLevelName == "connect" && Network.connections.Length > 0) {
			Application.LoadLevel("ingame");
		}
	}
	
	void host() {
		NetworkConnectionError result = Network.InitializeServer(3, 2233, false);
		Debug.Log("Hosting Server");
		Debug.Log(result);
	}
	
	void join(string ip) {
		NetworkConnectionError result = Network.Connect(ip, 2233);
		Debug.Log("Joining IP: "+ip);
		Debug.Log(result);
	}
}
