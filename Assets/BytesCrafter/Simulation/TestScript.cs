﻿
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BytesCrafter.USocketNet;

public class TestScript : MonoBehaviour
{
	public string dataSting = "Content: {\"identity\":\"3AfHKEvInRRFhfQzAAAq\",\"stat\":\"\",\"pos\":{\"x\":-2.394459,\"y\":0.000005103648,\"z\":-6.080234},\"rot\":{\"x\":0,\"y\":346.206,\"z\":0},\"sca\":{\"x\":0,\"y\":0,\"z\":0}} Last: 2018-1-12 17:41:16\n";
	public int rawSize = 0;
	public int baseSize = 0;

	void OnValidate ()
	{
		byte[] bytesToEncode = Encoding.UTF8.GetBytes (dataSting);
		rawSize = bytesToEncode.Length;

		string encodedText = Convert.ToBase64String (bytesToEncode);

		byte[] base64 = System.Convert.FromBase64String (encodedText);
		baseSize = base64.Length;

		//byte[] bytesToEncode = Encoding.UTF8.GetBytes (inputText);
		//string encodedText = Convert.ToBase64String (bytesToEncode);

		//byte[] decodedBytes = Convert.FromBase64String (encodedText);
		//string decodedText = Encoding.UTF8.GetString (decodedBytes);
	}

	//public List<string> sending;
	//public List<string> lastTest;
	//public List<string> newTest;

	/*
	public string vectorString = "";
	public CustomState states = new CustomState();
	public List<string> lastState = new List<string>();

	void Update()
	{
		vectorString = transform.position.ToString ();

		//if(!lastTest.Equals(newTest))
		//{
			//states.syncValue = stateJson.states;
			//states.syncStatus = "Synchronizing...";
			//Debug.Log (states.syncStatus);

			//socketNet.StateSync (stateJson);

		//	newTest = lastTest;//stateJson.states = lastState;
		//	sending = newTest;
		//}

		//Send data as transform.
		StateJson stateJson = new StateJson();

		bool sameAsLast = false;

		if(states.synchronize)
		{
			if(lastState.Equals(states.syncValue))
			{
				//states.syncValue = lastState;
				//states.syncValue = stateJson.states;

				lastState = new List<string> (states.syncValue.Count);
				foreach(string str in states.syncValue) { lastState.Add (str); }
				//lastState = states.syncValue;

				sameAsLast = true;
				states.syncStatus = "Synchronizing...";
			}

			else
			{
				states.syncStatus = "Waiting...";
			}
		}

		//if(socketNet != null && sameAsLast)
		//{
		//	socketNet.StateSync (stateJson);
		//}
	}
	*/
} 