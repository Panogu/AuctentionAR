using ARETT;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class GazeProcessor : MonoBehaviour
{

    // Connect the DataProvider-Prefab from ARETT in the Unity Editor
    public DataProvider DataProvider;
    private ConcurrentQueue<Action> _mainThreadWorkQueue = new ConcurrentQueue<Action>();

    public HeatmapCalculator heatmapCalculator;
    public GameObject gazePoint;

    public bool useARETT = false;

    // Start is called before the first frame update
    void Start()
    {
        if (useARETT)
        {
            StartArettData();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there is something to process
        if (!_mainThreadWorkQueue.IsEmpty)
        {
            // Process all commands which are waiting to be processed
            // Note: This isn't 100% thread save as we could end in a loop when there is still new data coming in.
            //       However, data is added slowly enough so we shouldn't run into issues.
            while (_mainThreadWorkQueue.TryDequeue(out Action action))
            {
                // Invoke the waiting action
                action.Invoke();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!useARETT)
        {
            // Do a Raycast from the middle of the camera to check if we are looking at an AOI
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                // Check if the hit object is an AOI
                if (hit.collider.gameObject.tag == "AOI")
                {
                    // Get the name of the AOI
                    string aoiName = hit.collider.gameObject.name;

                    // Register the gaze hit
                    heatmapCalculator.RegisterGazeInput(aoiName);
                }
                else
                {
                    heatmapCalculator.RegisterGazeInput("None");
                }
                gazePoint.transform.position = hit.point;
            }
            else
            {
                heatmapCalculator.RegisterGazeInput("None");
            }
        }
    }

    /// <summary>
    /// Starts the Coroutine to get Eye tracking data on the HL2 from ARETT.
    /// </summary>
    public void StartArettData()
    {
        StartCoroutine(SubscribeToARETTData());
    }

    /// <summary>
    /// Subscribes to newDataEvent from ARETT.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SubscribeToARETTData()
    {
        //*
        _mainThreadWorkQueue.Enqueue(() =>
        {
            DataProvider.NewDataEvent += HandleDataFromARETT;
        });
        //*/

        print("subscribed to ARETT events");
        yield return null;

    }

    /// <summary>
    /// Unsubscribes from NewDataEvent from ARETT.
    /// </summary>
    public void UnsubscribeFromARETTData()
    {
        _mainThreadWorkQueue.Enqueue(() =>
        {
            DataProvider.NewDataEvent -= HandleDataFromARETT;
        });

    }




    /// <summary>
    /// Handles gaze data from ARETT and allows you to do something with it
    /// </summary>
    /// <param name="gd"></param>
    /// <returns></returns>
    public void HandleDataFromARETT(GazeData gd)
    {
        // Some exemplary values from ARETT.
        // for a full list of available data see:
        // https://github.com/AR-Eye-Tracking-Toolkit/ARETT/wiki/Log-Format#gaze-data
        string t = "Received GazeData\n";
        t += "GazePointAOIHit:" + gd.GazePointAOIHit;
        t += "\nEyeDataRelativeTimestamp:" + gd.EyeDataRelativeTimestamp;
        t += "\nGazeDirection: " + gd.GazeDirection;
        t += "\nGazePointWebcam: " + gd.GazePointWebcam;
        t += "\nGazeHasValue: " + gd.GazeHasValue;
        t += "\nGazePoint: " + gd.GazePoint;
        t += "\nGazePointMonoDisplay: " + gd.GazePointMonoDisplay;
        //Debug.Log(t);

        // Check if the gaze has a value
        if (gd.GazeHasValue)
        {
            // Check if the gaze has hit an Area of Interest
            if (gd.GazePointAOIHit)
            {
                // Get the name of the AOI
                string aoiName = gd.GazePointAOIName;

                // Register the gaze hit
                heatmapCalculator.RegisterGazeInput(aoiName);
                gazePoint.transform.position = gd.GazePointAOIHitPosition;
            }
            else
            {
                heatmapCalculator.RegisterGazeInput("None");
            }
        }

    }
}
