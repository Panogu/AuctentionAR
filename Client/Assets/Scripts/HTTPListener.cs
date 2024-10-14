// Unity HTTP listener
// Largely based on https://gist.github.com/amimaro/10e879ccb54b2cacae4b81abea455b10
// No license specified!

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class HTTPListener : MonoBehaviour
{
	private HttpListener listener;
	private Thread listenerThread;

	private List<(string, Rect)> objects = new List<(string, Rect)>();
	private List<(string, int)> markers = new List<(string, int)>();

	void Start ()
	{
		Debug.Log("Starting HTTPListener script");

		listener = new HttpListener();
		
		// Set the HoloLenses/Devices IP address, or use the following line to get the IPv4 automatically:		
		var ipAddress = GetIP4Address();
		// Change the port if needed, it needs to match to from where you send the data. 
		listener.Prefixes.Add($"http://{ipAddress}:5050/"); 

		listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
		listener.Start ();

		listenerThread = new Thread (StartListener);
		listenerThread.Start ();
		Debug.Log ("Server Started");
		Debug.Log("Listening on: " + $"http://{ipAddress}:5050/");
		
	}

	void Update()
	{
		// Do something with the received data
		foreach ((string, Rect) val in objects)
		{
			Debug.Log($"Object name: {val.Item1}" + "\n" + $"Bounding box: {val.Item2}");
            BroadcastMessage("HandleBoundingBoxData", val);
        }
		// Clear the list
		objects.Clear();

		foreach ((string, int) val in markers)
		{
			Debug.Log($"Marker at AOI: {val.Item1}" + "\n" + $"Duration: {val.Item2}");
            BroadcastMessage("HandleMarkerData", val);
        }
		// Clear the list
		markers.Clear();
	}
	

	    /// <summary>
	    /// Gets the IP v4 address from the current device
	    /// </summary>
	    /// <returns>ip address as string</returns>
	private static string GetIP4Address()
	{
		string IP4Address = String.Empty;
		foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
		{
		    if (IPA.AddressFamily == AddressFamily.InterNetwork)
		    {
			IP4Address = IPA.ToString();
			break;
		    }
		}
		return IP4Address;
	}

	private void StartListener ()
	{
		while (true) {               
			var result = listener.BeginGetContext (ListenerCallback, listener);
			result.AsyncWaitHandle.WaitOne ();
		}
	}

	private void ListenerCallback (IAsyncResult result)
	{				
		var context = listener.EndGetContext(result);		
		
		Debug.Log ("Request received");


		// from a GET-request: example.com/?key=value&...
		if (context.Request.QueryString.AllKeys.Length > 0)
		{
			/* **************************************
			 * !!! Use only one of the three options!
			 * ***************************************
			*/


			/* **************************************
			 * 1.
			 * 
			 * Use the following to loop through all the parameters in the GET-Request's URL
			 * ***************************************
			*/
			foreach (var key in context.Request.QueryString.AllKeys)
			{
				var value = context.Request.QueryString.GetValues(key)[0];

				/*Debug.Log($"key: {key}");
				Debug.Log($"value: {value}");*/


                // DO something with the received key value pair
                // To get a specific value by key (and not the key in the current foreach-loop): context.Request.QueryString["somekey"]

                if (key.StartsWith("yolo_cereal"))
                {
                    var bBoxCoordTLx = int.Parse(context.Request.QueryString["coordTLx"]);  // top left x-value
                    var bBoxCoordTLy = int.Parse(context.Request.QueryString["coordTLy"]);
                    var bBoxCoordBRx = int.Parse(context.Request.QueryString["coordBRx"]);  // bottom right x-value
                    var bBoxCoordBRy = int.Parse(context.Request.QueryString["coordBRy"]);

                    Vector2 bBoxCoordTL = new Vector2(bBoxCoordTLx, bBoxCoordTLy);
                    Vector2 bBoxCoordBR = new Vector2(bBoxCoordBRx, bBoxCoordBRy);

					Rect boundingBox = new Rect(bBoxCoordBR, bBoxCoordTL-bBoxCoordBR);

					// Get the name (key after "yolo_") of the object
					string objectName = key.Substring(5);

					// Add the object name and bounding box to the list (as BroadcastMessage cannot be called from a thread)
					objects.Add((objectName, boundingBox));

					Debug.Log($"Object name: {objectName}" + "\n" + $"Bounding box: {boundingBox}");

                }

				if (key.StartsWith("aoi_"))
				{
					var aoiName = key.Substring(4);
					var aoiDuration = int.Parse(context.Request.QueryString["duration"]);

					// Add the AOI name and duration to the list (as BroadcastMessage cannot be called from a thread)
					markers.Add((aoiName, aoiDuration));

					Debug.Log($"Marker at AOI: {aoiName}" + "\n" + $"Duration: {aoiDuration}");
				}

            }

			// Respond with a simple "OK" message
			context.Response.StatusCode = 200;
			context.Response.ContentLength64 = 0;
			context.Response.OutputStream.Close();
			Debug.Log("Response sent");
		}
	}
}
