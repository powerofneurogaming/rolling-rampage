using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveAlong : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player Ball");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.x, player.transform.position.z);
    }
}
