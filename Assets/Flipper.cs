using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject flipperObject = GameObject.FindWithTag("flipper");
        Transform flipper = GameObject.FindWithTag("flipper").transform;
        Flipper2 flipperScript = GameObject.FindWithTag("flipper").GetComponent<Flipper2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onTriggerEnter(GameObject flipperObject)
    {
        Vector3 targetDir = flipperObject.transform.position - transform.position;
        Flipper2.Rotate(targetDir, 0, 0, 90, Space.Self);
    }
}
