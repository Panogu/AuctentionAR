// Unity HTTP listener
// Largely based on https://gist.github.com/amimaro/10e879ccb54b2cacae4b81abea455b10
// No license specified!

using System;
using System.Net;
using System.Threading;
using UnityEngine;

public class HandleDataFromHTTPListener : MonoBehaviour
{
	public bool NewDataArrived;
	public string TmpVariable01;
	public string TmpVariable02;

	/*
	 * Note: You need to use the script "HTTPListener.cs" in combination with this script.
	 * 
	 * This is an example on you to use the data coming from the HTTPListener in another function/script.
	 * You cannot directly call functions in HTTPListener.cs with the received parameters as input. 
	 * (It has to do with Unity's way of handling threading or something like this.)
	 * Using the Update() function here to check if NewDataArrived=true is a working solution.
	 * 
	 * 
	 */

	void Start ()
	{
		NewDataArrived = false;
		TmpVariable01 = "";
		TmpVariable02 = "";


	}
	void Update()
	{
		if (NewDataArrived)
		{
			HandleIncomingData(TmpVariable01, TmpVariable02);
			NewDataArrived = false;
			TmpVariable01 = "";
			TmpVariable02 = "";
		}
	}

	private void HandleIncomingData(string tmp01, string tmp02)
    {
		// Do something with the incoming data
    }

}
