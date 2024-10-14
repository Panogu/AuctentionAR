using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterventionController : MonoBehaviour
{
    public GameObject markerPrefab;
    List<GameObject> activeMarkers = new List<GameObject>();

    public void HandleMarkerData((string, int) locationDuration)
    {
        PlaceMarkerAtAOI(locationDuration);
    }

    public void PlaceMarkerAtAOI((string, int) locationDuration)
    {
        // Find the game object with the name of the AOI
        GameObject aoi = GameObject.Find(locationDuration.Item1);

        if (aoi == null)
        {
            Debug.Log("AOI for placing marker not found");
            return;
        }

        // Instantiate a marker at the AOI if it does not exist yet
        GameObject marker = activeMarkers.Find(marker => aoi.name + "_marker" == name);
        if (marker == null)
        {
            marker = Instantiate(markerPrefab, aoi.transform.position, Quaternion.identity);
            marker.name = locationDuration.Item1 + "_marker";
            marker.GetComponent<MarkerController>().SetInterventionController(this);
            activeMarkers.Add(marker);
        }
        marker.GetComponent<MarkerController>().SetLife(locationDuration.Item2);
    }

    public void RemoveMarker(GameObject marker)
    {
        activeMarkers.Remove(marker);
    }
}
