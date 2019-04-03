using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;


public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    public Rigidbody cameraRB;
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
        BallCameraDistance();
        ForwardMovment();

    }

    // if the function uses phisics then use this update
    private void FixedUpdate()
    {
        if (!LevelManager.LM.isStarted)
        {
            return;
        }
        CameraMovment();
    }

    // moves the ball side to side in acordance with the position of a ray cast
    void HorizontalMovement(){

        if (TobiiAPI.GetUserPresence() == UserPresence.Present)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray screenRay = Camera.main.ScreenPointToRay(TobiiAPI.GetGazePoint().Screen);
            RaycastHit hit;
            //float hitDistance = 0;
            if (Physics.Raycast(screenRay, out hit,100))
            {
                Vector3 temp = new Vector3(hit.point.x, hit.point.y + .66f, hit.point.z);
                cursor.transform.position = temp;
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

    // moves the ball forward at a speed of veriable "ballSpeed"
    void ForwardMovment()
    {
        // if the ball is with these bonds then i want the ball to move at "ballSPeed" otherwise i want the ball to move at the camera speed
        if (currentBallDistance < maxBallDistance && currentBallDistance > minBallDistance)
        {
            myRB.velocity = Vector3.forward * ballSpeed;
            if (Input.GetButton("Fire1"))
            {
                float upDownSpeed = Input.GetAxis("Mouse Y") * bonusBallSpeed;
                myRB.velocity = Vector3.forward * (ballSpeed + upDownSpeed);
            }
        }
        else
        {
            myRB.velocity = Vector3.forward * cameraSpeed;
            if (Input.GetButton("Fire1"))
            {
                float upDownSpeed = Input.GetAxis("Mouse Y")* bonusBallSpeed;
                if(currentBallDistance >= maxBallDistance)
                {
                    if(upDownSpeed < 0)
                    {
                        myRB.velocity = Vector3.forward * (ballSpeed + upDownSpeed);
                    }
                }
                if(currentBallDistance<= minBallDistance)
                {
                    if(upDownSpeed>0)
                    {
                        myRB.velocity = Vector3.forward * (ballSpeed + upDownSpeed);
                    }
                }
            }
        }
    }

    // moves the camera forward at a constant speed to the ball is not always in the center of the screen
    void CameraMovment()
    {
        cameraRB.velocity = Vector3.forward * cameraSpeed;
        // match the particle systems to the camera.
        particleRB.velocity = Vector3.forward * cameraSpeed;
}

//calculate the distance between the ball and the camera
void BallCameraDistance()
    {
        currentBallDistance = Vector3.Distance(new Vector3(0,0, myCamera.transform.position.z), new Vector3(0,0, transform.position.z));
    }

    private void OnCollisionEnter(Collision collision)
    {
        //this is destroy the player if they hit an enemy obstical
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
            LevelManager.LM.gameOverPanel.SetActive(true);
            cameraRB.velocity = Vector3.zero;
            particleRB.velocity = Vector3.zero;
            LevelManager.LM.isStarted = false;
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        //this is going to stop the player once they are over the finish line
        if(other.tag == "FinishLine")
        {
            LevelManager.LM.winPannel.SetActive(true);
            cameraRB.velocity = Vector3.zero;
            particleRB.velocity = Vector3.zero;
            LevelManager.LM.isStarted = false;
        }
    }
}
