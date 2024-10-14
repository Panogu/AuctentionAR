using System;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CertificateAccepter : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

public class HeatmapCalculator : MonoBehaviour
{
    public List<String> gazeLog = new List<String>();
    public float millisToAverageOver = 5.0f;
    public int samplingRate = 30;
    public String serverAddress = "https://panogu.tunnelto.dev/startAuction";

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(SendHeatmapData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SendHeatmapData()
    {
        while (true)
        {
            Dictionary<string, float> heatmap = CalculateHeatmap();

            //Debug.Log("Heatmap: " + heatmap);

            // Create a JSON string from the heatmap
            string json = "{";
            if(heatmap.Count > 0){
                foreach (KeyValuePair<string, float> entry in heatmap)
                {
                    json += "\"" + entry.Key + "\": " + entry.Value + ", ";
                }
                json = json.Substring(0, json.Length - 2);
            }
            json += "}";

            json = "{\"heatmap\": " + json + "}";

            Debug.Log("Uploading Heatmap: " + json);

            UnityWebRequest www = new UnityWebRequest(serverAddress, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateAccepter();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }

            if(www.result == UnityWebRequest.Result.Success)
            {
                // Get the bidders name from the body
                string response = www.downloadHandler.text;

                Debug.Log(response);

                Debug.Log("Heatmap uploaded successfully");
            }

            yield return new WaitForSeconds(5);
        }
    }

    public void RegisterGazeInput(string object_of_interest) {
        gazeLog.Add(object_of_interest);
    
    }

    public Dictionary<string, float> CalculateHeatmap()
    {
        int entriesToConsider = (int)(millisToAverageOver * samplingRate);
        int totalEntries = Math.Min(entriesToConsider, gazeLog.Count);

        List<string> lastEntries = gazeLog.Skip(Math.Max(0, gazeLog.Count() - totalEntries)).ToList();

        Dictionary<string, float> heatmap = new Dictionary<string, float>();

        foreach (string entry in lastEntries)
        {
            if (heatmap.ContainsKey(entry))
            {
                heatmap[entry]++;
            }
            else
            {
                heatmap[entry] = 1;
            }
        }

        List<string> keys = new List<string>(heatmap.Keys);
        foreach (string key in keys)
        {
            heatmap[key] = heatmap[key] / totalEntries;
        }

        // Clear the gaze log
        gazeLog.Clear();

        // Now `heatmap` contains the percentage of each unique value in the last `n` entries
        return heatmap;
    }

    // Upload an example heatmap
    public void UploadExampleHeatMap()
    {
        StartCoroutine(SendExampleHeatmap());
    }

    IEnumerator SendExampleHeatmap()
    {
            Dictionary<string, float> heatmap = new Dictionary<string, float>();
            heatmap["cereal_1"] = 0.5f;
            heatmap["cereal_2"] = 0.9f;
            heatmap["cereal_3"] = 0.7f;

            //Debug.Log("Heatmap: " + heatmap);

            // Create a JSON string from the heatmap
            string json = "{";
            if (heatmap.Count > 0)
            {
                foreach (KeyValuePair<string, float> entry in heatmap)
                {
                    json += "\"" + entry.Key + "\": " + entry.Value + ", ";
                }
                json = json.Substring(0, json.Length - 2);
            }
            json += "}";

            json = "{\"heatmap\": " + json + "}";

            Debug.Log("Uploading Heatmap: " + json);

            UnityWebRequest www = new UnityWebRequest(serverAddress, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateAccepter();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Get the bidders name from the body
                string response = www.downloadHandler.text;

                Debug.Log(response);

                Debug.Log("Heatmap uploaded successfully");
            }

            yield return null;
    }

}
