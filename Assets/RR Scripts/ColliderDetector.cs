using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetector : MonoBehaviour
{
    //TODO: FIND OUT HOW TO GET SLIDE SOUND TO BE CONTINUOUS

    //audioscript = script for playing hit sounds on collision and other audio.
    public AudioScript audioScript;

    //collidedObject is the object that the player ball has collided with.
    private GameObject collidedObject;

    //object array to store all the detected objects in the scene.
    public GameObject[] gameObjects;

    //This GameObject as a variable.
    public GameObject thisObj;

    //Object and player values of distance, to prevent a player from playing a sound if they aren't within a short player range.
    private Transform objTransform; //The transform of this object
    private Transform playerTransform; //The transform of the player ball

    //Bool canPlay is a boolean to check if a sound has been played too many times in the past x seconds, and block it from playing if canPlay is false.
    public bool canPlay = true;

    //Vars to check if object has a steadily decreasing y-value as an object that fell off the platform.
    private float currY;
    private float oldY;
    private bool hasFallen;

    //GameObject Platform is used to filter out the platform to prevent constant sliding noise where not needed.
    private GameObject Platform;

    //Check to see if contact has occurred within the last x seconds to prevent a stuttering slide sound.
    private int canSlide = 1;

    // Start is called before the first frame update
    void Start()
    {
        audioScript = GameObject.Find("AudioControl").GetComponent<AudioScript>();

        objTransform = this.transform;
        if (GameObject.Find("Player Ball") != null)
        {
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }
        oldY = this.GetComponent<Transform>().position.y;
        thisObj = gameObject;
        hasFallen = false;

        // Declare Platform GameObject here
        Platform = GameObject.Find("Platform").transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (thisObj == null)
        {
            thisObj = gameObject;
        }
        currY = this.GetComponent<Transform>().position.y;
        if (currY + 0.25 < oldY && hasFallen == false)
        {
            AudioScript.instance.playSoundWoosh(thisObj);
            hasFallen = true;
        }
        else if (hasFallen == false)
        {
            oldY = currY;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canPlay)
        {
            collidedObject = collision.gameObject;
            if (objTransform.position.z - playerTransform.position.z < 0.5f)
            {
                if (this.tag == "Player")
                {
                    audioScript.playSoundPlayer(collidedObject);
                }

                else
                {
                    audioScript.playSound(collidedObject);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider finishLine)
    {
        if (finishLine.tag == "FinishLine")
        {
            AudioScript.instance.playSoundPlayer(GameObject.Find("Finish Line"));
        }
    }

    //Check to see if the player is pushing an object, and play an object sliding sound. Spheres do not slide and so do not make this sound.
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Player Ball" && gameObject.GetComponent<SphereCollider>() == null && canSlide == 1)
        {
            AudioScript.instance.playSlide();
        }
    }
    //Check to see if the player is not pushing an object, and stop the sliding sound if it is not.
    private void OnCollisionExit(Collision collision)
    {
        canSlide = 0;
        StartCoroutine(slideWaitStop());
    }
    // USed to prevent slide sound stuttering.
    IEnumerator slideWaitStop()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        canSlide = 1;
        AudioScript.instance.stopSlide();
    }

    //Coroutine waitToDetect is used to prevent an overload of sounds all at once.
    public IEnumerator waitToDetect()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        canPlay = true;
    }
}