using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class PlayerController1 : MonoBehaviour
{
    Rigidbody myRB;
    GameObject cursor;
    GameObject myCamera;

    public float ballSpeed;
    public float cameraSpeed;
    public bool tobiiOn;

    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myCamera = GameObject.Find("Main Camera");
        cursor = GameObject.Find("black cursor");
    }

    void Update()
    {
        if (LevelManager.LM.isStarted)
        {
            PlayerMovement();
        }
    }

    // Updates camera after physics update
    private void FixedUpdate()
    {
        if (LevelManager.LM.isStarted)
        {
            // Moves the camera forward at a constant speed to the ball is not always in the center of the screen
            myCamera.transform.position += Vector3.forward * cameraSpeed * Time.deltaTime;
        }
    }

    // Moves the ball side to side in accordance with the position of a ray cast
    void PlayerMovement(){
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray screenRay = Camera.main.ScreenPointToRay(tobiiOn ? TobiiAPI.GetGazePoint().Screen : (Vector2)Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(screenRay, out hit,100))
        {
            cursor.transform.position = new Vector3(hit.point.x, hit.point.y + .66f, hit.point.z);
            myRB.AddForce((new Vector3(hit.point.x, .775f, cursor.transform.position.z) - transform.position) * 50);
        }

        // If the ball is within bounds the ball will move at "ballSpeed", otherwise the ball will move at the camera speed
        myRB.AddForce(Vector3.forward * 100 * cameraSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // This destroys the player if they hit an enemy obstacle
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
            LevelManager.LM.gameOverPanel.SetActive(true);
            LevelManager.LM.isStarted = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // This will stop the player once they reach the finish line
        if(other.tag == "FinishLine")
        {
            LevelManager.LM.winPannel.SetActive(true);
            LevelManager.LM.isStarted = false;
        }
    }
}
