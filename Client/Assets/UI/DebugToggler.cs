using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugToggler : MonoBehaviour
{
    public bool toggleState = true;
    public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Toggle()
    {

        Debug.Log("Toggling debug objects");

        toggleState = !toggleState;
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = toggleState;
            }
        }

        if (toggleState)
        {
            // Set the culling mask of the main camera to include the EyeTracking layer
            Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("EyeTracking");
        } else {
            // Set the culling mask of the main camera to not include the EyeTracking layer
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("EyeTracking"));
        }
    }

    public void SetToggle(bool state)
    {
        toggleState = state;
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.enabled = toggleState;
        }
    }

    public void AddComponent(MeshRenderer renderer)
    {
        meshRenderers.Add(renderer);

        if (toggleState)
        {
            renderer.enabled = toggleState;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
