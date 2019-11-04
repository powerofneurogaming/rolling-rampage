using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSide : MonoBehaviour
{
    public float max;
    public float min;
    public float speed;
    public float pauseBetweenReverse;

    bool reverse;
    Vector3 startLoc;
    float currentPause;
    float currentMove = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        startLoc = transform.position;
        currentMove = -min / (max - min) * speed;
    }

    // Update is called once per frame
    void Update()
    {
        currentPause += Time.deltaTime;
        if (currentPause > pauseBetweenReverse)
        {
            currentMove += Time.deltaTime * (reverse ? -1 : 1);
            transform.position = Vector3.Lerp(startLoc + (transform.right * min), startLoc + (transform.right * max), currentMove / speed);
            if (reverse ? currentMove / speed <= 0 : currentMove / speed >= 1)
            {
                reverse = !reverse;
                currentPause = 0.0f;
            }
        }
    }
}
