using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimationScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Scale the x and y between 100 and 300
        float x = 200 + Mathf.Sin(Time.time * 5) * 100;
        float y = 200 + Mathf.Sin(Time.time * 5) * 100;

        // Set the scale
        transform.localScale = new Vector3(x, y, transform.localScale.z);
    }
}
