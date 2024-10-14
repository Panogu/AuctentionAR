using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOICreator : MonoBehaviour
{
    public Camera HL2Camera;

    public GameObject AOIPrefab;

    public List<GameObject> AOIs = new List<GameObject>();

    public DebugToggler debugToggler;
    public GameObject exampleContainer;


    // Start is called before the first frame update
    void Start()
    {
        // If the Example Container is enabled, find all the AOIs (childreen) and add them to the list
        if (exampleContainer.activeSelf)
        {
            foreach (Transform child in exampleContainer.transform)
            {
                AOIs.Add(child.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleBoundingBoxData((string, Rect) boundingBoxData)
    {
        //Debug.Log("Received bounding box data: " + boundingBoxData.Item1 + " " + boundingBoxData.Item2);

        // Create an AOI for the bounding box
        CreateAOI(boundingBoxData.Item1, boundingBoxData.Item2);
    }

    // Function for creating an AOI at specific 3d coordinates (convert from 2d coordinates to 3d)
    public void CreateAOI(string name, Rect rect)
    {
        // Create a new AOI if it doesn't exist yet
        GameObject AOI = AOIs.Find(aoi => aoi.name == name);
        bool created = false;
        if (AOI == null)
        {
            AOI = Instantiate(AOIPrefab);
            AOI.name = name;
            AOIs.Add(AOI);
            created = true;
        } else {
            // For debug reasons do not update the AOI if it already exists for now (we can set this up manually)
            return;
        }
        
        /*var bottomLeft = new Vector2(rect.x, HL2Camera.pixelHeight - rect.y);

        // Do a raycast from the camera to the 3d coordinates
        RaycastHit hit;
        Physics.Raycast(HL2Camera.ScreenPointToRay(bottomLeft), out hit);

        // Set the position of the AOI
        AOI.transform.position = hit.point;

        var topRight = new Vector2(rect.x + rect.width, HL2Camera.pixelHeight - rect.y - rect.height);

        // Do a raycast from the camera to the 3d coordinates
        Physics.Raycast(HL2Camera.ScreenPointToRay(topRight), out hit);

        // Set the AOI to the correct scale
        AOI.transform.localScale = new Vector3(hit.point.x - AOI.transform.position.x, hit.point.y - AOI.transform.position.y, 0.05f);*/

        // Get the center of the bounding box
        var center = new Vector2(rect.x + rect.width / 2, HL2Camera.pixelHeight - rect.y - rect.height / 2);

        // Do a raycast from the camera to the 3d coordinates
        RaycastHit hit;
        Physics.Raycast(HL2Camera.ScreenPointToRay(center), out hit);
        if (hit.collider == null)
        {
            Debug.Log("No collider found for AOI " + name);
            return;
        }

        // Set the position of the AOI
        AOI.transform.position = hit.point;

        // Move the AOI to the eye tracking layer
        AOI.layer = 8;

        // If the AOI was newly created, set a random color and add it to the debug toggler
        if(created)
        {
            AOI.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
            debugToggler.AddComponent(AOI.GetComponent<MeshRenderer>());
        }
    }

    public void ResetAOIs()
    {
        foreach (GameObject AOI in AOIs)
        {
            // Remo the AOI from the debug toggler
            debugToggler.meshRenderers.Remove(AOI.GetComponent<MeshRenderer>());
            Destroy(AOI);
        }
        AOIs.Clear();
    }
}
