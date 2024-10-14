using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FaceCamera : MonoBehaviour
{

    public GameObject mainCam;

    // Start is called before the first frame update
    public void Start()
    {
        mainCam = Camera.main.gameObject;
    }

    // Update is called once per frame
    public void Update()
    {
        LookAtCamera(Vector3.up);   
    }

    public void LookAtCamera(Vector3 up)
    {
        transform.LookAt(mainCam.transform, up);
    }
}
