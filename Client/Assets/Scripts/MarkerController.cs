using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerController : FaceCamera
{

    int remainingLife;
    InterventionController interventionController;

    public void SetInterventionController(InterventionController interventionController)
    {
        this.interventionController = interventionController;
    }

    public void SetLife(int millis)
    {
        remainingLife = millis;
    }

    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if (interventionController)
        {
            remainingLife -= (int)(Time.deltaTime * 1000);
            if (remainingLife <= 0)
            {
                interventionController.RemoveMarker(gameObject);
                Destroy(gameObject);
            }
        }
        base.LookAtCamera(Vector3.up);
    }
}
