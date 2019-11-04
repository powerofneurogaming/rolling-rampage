using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using Tobii.GameIntegration;

public class TobiiControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TobiiAPI.GetUserPresence() == UserPresence.Present)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray screenRay = Camera.main.ScreenPointToRay(TobiiAPI.GetGazePoint().Screen);
            float hitDistance = 0;
            if (plane.Raycast(screenRay, out hitDistance))
            {
                Vector3 temp = new Vector3(screenRay.GetPoint(hitDistance).x, screenRay.GetPoint(hitDistance).y + .66f, screenRay.GetPoint(hitDistance).z);
                transform.position = temp;
                Debug.Log(transform.position + " " + hitDistance);
            }
        }

        //float x = TobiiAPI.GetGazePoint().x;
    }
}
