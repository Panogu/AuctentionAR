using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectOfInterest : MonoBehaviour
{
    public int millisToAverageOver;

    // The timestamps of the last hits
    public List<int> hits;

    /*public string ToJson()
    {
        return "{ \"name\": \"" + name + "\", \"gazePercentage\": " + gazePercentage + " }";
    }*/

    bool debugNextUpdateRedMaterial = false;
    Color defaultColor;

    public ObjectOfInterest(int millisToAverageOver)
    {
        this.millisToAverageOver = millisToAverageOver;
        this.hits = new List<int>();
    }

    public int ProcessGaze()
    {
        int hitcount = 0;

        // Count the hits between now and now - averageMillis
        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i] > DateTime.Now.Millisecond - millisToAverageOver)
            {
                // Count the hit
                hitcount++;
                debugNextUpdateRedMaterial = true;
            }
        }

        // Remove the hits that are older than averageMillis
        hits = hits.Where(x => x > DateTime.Now.Millisecond - millisToAverageOver).ToList();

        // Return the hitcount
        return hitcount;

    }

    public void RegisterGaze()
    {
        // Add the current timestamp to the hits
        hits.Add(DateTime.Now.Millisecond);
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultColor = GetComponent<Renderer>().material.color;
        Debug.Log("Starting ObjectOfInterest: " + gameObject.name);

    }

    // Update is called once per frame
    void Update()
    {
        if(debugNextUpdateRedMaterial)
        {
            GetComponent<Renderer>().material.color = Color.red;
            debugNextUpdateRedMaterial = false;
        }
        else
        {
            GetComponent<Renderer>().material.color = defaultColor;
        }
    }
}
