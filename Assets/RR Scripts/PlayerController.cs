using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;


public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    Rigidbody cameraRB;
    public Rigidbody particleRB;
    public GameObject cursor;

    public Camera myCamera;
    public float lerpSpeed;

    public float ballSpeed;
    public float cameraSpeed;
    public float bonusBallSpeed;

    public float minBallDistance;
    public float maxBallDistance;
    public float currentBallDistance;

    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraRB = myCamera.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!LevelManager.LM.isStarted)
        {
            return;
        }
        //if (Input.GetButton("Fire1"))
        //{
        //    HorizontalMovement();
        //}
        HorizontalMovement();
        ForwardMovment();
    }

    // Updates camera after physics update
    private void FixedUpdate()
    {
        if (!LevelManager.LM.isStarted)
        {
            return;
        }
        CameraMovment();
    }

    // Moves the ball side to side in accordance with the position of a ray cast
    void HorizontalMovement(){

        if (TobiiAPI.GetUserPresence() == UserPresence.Present)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray screenRay = Camera.main.ScreenPointToRay(TobiiAPI.GetGazePoint().Screen);
            RaycastHit hit;
            //float hitDistance = 0;
            if (Physics.Raycast(screenRay, out hit,100))
            {
                cursor.transform.position = new Vector3(hit.point.x, hit.point.y + .66f, hit.point.z);
                transform.position = Vector3.Lerp(transform.position, new Vector3(hit.point.x, .775f, cursor.transform.position.z), lerpSpeed * Time.deltaTime);
            }
        }

        //Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //if(Physics.Raycast(ray,out hit, 100))
        //{
        //    transform.position = Vector3.Lerp(transform.position, new Vector3(hit.point.x, transform.position.y, transform.position.z), lerpSpeed * Time.deltaTime);
        //}
    }

    // Moves the ball forward at a speed of variable "ballSpeed"
    void ForwardMovment()
    {
        float upDownSpeed = Input.GetAxis("Mouse Y") * bonusBallSpeed;
        // Calculate the distance between the ball and the camera
        currentBallDistance = Vector3.Distance(new Vector3(0, 0, myCamera.transform.position.z), new Vector3(0, 0, transform.position.z));

        // If the ball is within bounds the ball will move at "ballSpeed", otherwise the ball will move at the camera speed
        if (currentBallDistance < maxBallDistance && currentBallDistance > minBallDistance)
        {
            myRB.velocity = Vector3.forward * ballSpeed;
            if (Input.GetButton("Fire1"))
            {
                myRB.velocity = Vector3.forward * (ballSpeed + upDownSpeed);
            }
        }
        else
        {
            myRB.velocity = Vector3.forward * cameraSpeed;
            if (Input.GetButton("Fire1"))
            {
                if ((currentBallDistance >= maxBallDistance && upDownSpeed < 0) || (currentBallDistance <= minBallDistance && upDownSpeed > 0))
                {
                    myRB.velocity = Vector3.forward * (ballSpeed + upDownSpeed);
                }
            }
        }
    }

    // Moves the camera forward at a constant speed to the ball is not always in the center of the screen
    void CameraMovment()
    {
        cameraRB.velocity = Vector3.forward * cameraSpeed;
        // Match the particle systems to the camera.
        if (particleRB != null)
            particleRB.velocity = Vector3.forward * cameraSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // This destroys the player if they hit an enemy obstacle
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
            LevelManager.LM.gameOverPanel.SetActive(true);
            cameraRB.velocity = Vector3.zero;
            if (particleRB != null)
                particleRB.velocity = Vector3.zero;
            LevelManager.LM.isStarted = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // This will stop the player once they reach the finish line
        if(other.tag == "FinishLine")
        {
            LevelManager.LM.winPannel.SetActive(true);
            cameraRB.velocity = Vector3.zero;
            if (particleRB != null)
                particleRB.velocity = Vector3.zero;
            LevelManager.LM.isStarted = false;
        }
    }
}
