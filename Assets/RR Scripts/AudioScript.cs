using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioScript : Singleton<AudioScript>
{
    //This value is the volume as set by the PlayerPrefs with the volume float.
    private float volume;
    //Each AudioSource is a separate clip that may need to play at the same time
    public AudioSource GameMusic;

    //"sounds" is an array of Audioclips.
    public AudioClip[] soundArray;

    //Special list of sounds for collisions with smaller objects
    public AudioClip[] tapSounds = new AudioClip[3];

    //Integer for choosing a random number among 3 light tap sounds;
    public int tapInt;

    //Higher Priority set of the sounds for player interaction;
    public AudioSource playSoundsPlayer;

    //Dedicated sound channel for woosh sounds of an object falling off the platform and a variable to make sure only one is played at a time.
    public AudioSource playWoosh;
    bool canPlayWoosh;

    //Integer for selecting wwhich audio source to play from out of the 10 audiosources.
    public int sourceInt;

    //Special audiosource to play a sliding sound when a player is pushing an oject.
    public AudioSource audioSlide;

    //Bool to check if object is being pushed by player. If true, player is pushing object, object sliding sound is playing.
    bool beingPushed = false;

    //Store the object size of an object that the player has collided with here.
    Vector3 objSize;

    //get the speed of the player here.
    public float playerLerpSpeed;

    // soundID is a sound clip. sourceID is the ID of the audiosource.
    public int soundID;
    public int sourceID;

    //Objects to Detect if same star has been run over, and not play the star collect sound a second time.
    private GameObject oldObject;

    //object array to store all the detected objects in the scene.
    public GameObject[] gameObjects;

    //keep track of how many audioclips are playing.
    private int playing;

    //bool to stop all sounds from playing once the player croses the finish line or dies to en enemy object
    bool allStop = false;

    private GameObject playerBall;

    private GameObject platform;
    private GameObject platformCube;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        volume = PlayerPrefs.GetFloat("volume", 1);
        // These sounds play only in specific circumstances.
        playSoundsPlayer = gameObject.AddComponent<AudioSource>();

        playSoundsPlayer.priority = 0;
        playSoundsPlayer.loop = false;

        tapSounds[0] = (AudioClip)Resources.Load("Sounds/hitTap1", typeof(AudioClip));
        tapSounds[1] = (AudioClip)Resources.Load("Sounds/hitTap2", typeof(AudioClip));
        tapSounds[2] = (AudioClip)Resources.Load("Sounds/hitTap3", typeof(AudioClip));

        audioSlide = gameObject.AddComponent<AudioSource>();
        audioSlide.clip = (AudioClip)Resources.Load("Sounds/objSlide", typeof(AudioClip));
        audioSlide.loop = true;

        soundArray = Resources.LoadAll<AudioClip>("Sounds") as AudioClip[];

        for (int i = 0; i < soundArray.Length; i++)
        {
            Debug.Log("Sound " + i + ": " + soundArray[i].name);
        }

        // The music is always playing.
        GameMusic = gameObject.AddComponent<AudioSource>();
        GameMusic.clip = Resources.Load("Music/title") as AudioClip;
        GameMusic.loop = true;
        GameMusic.Play();

        //On woosh, set this to false, wait X, set to true again.
        canPlayWoosh = true;

        playWoosh = gameObject.AddComponent<AudioSource>();

        //Identify loaded scene, run method OnSceneLoaded every time a new scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //On scene load, find prefabs and player ball and add the ColliderDetector script to them.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        allStop = false;
        Debug.Log("The Player Ball is: " + GameObject.Find("Player Ball"));
        GameObject.Find("Player Ball").AddComponent<ColliderDetector>();
        playerBall = GameObject.Find("Player Ball");

        platform = GameObject.Find("Platform");
        platformCube = platform.transform.GetChild(0).gameObject;
        Debug.Log("The Platform is: " + platformCube);

        //find all GameObjects, and attach the ColliderDetector to it.
        gameObjects = Object.FindObjectsOfType<GameObject>() as GameObject[];
        Debug.Log("Total GameObjects in scene = " + gameObjects.Length + ".");

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].GetComponent<Collider>() != null && gameObjects[i].GetComponent<ColliderDetector>() == null && gameObjects[i] != platformCube)
            {
                gameObjects[i].AddComponent<ColliderDetector>();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (playerBall != null)
        {
            playerLerpSpeed = playerBall.GetComponent<PlayerController>().lerpSpeed;
        }
    }

    // Play the sound based on the lerp speed of the player during a collision, except for objects tagged enemy.
    public void playSoundPlayer(GameObject collidedObject)
    {
        Debug.Log("playerLerpSpeed = " + playerLerpSpeed);

        //stop all sounds on crossing the finish line.
        if (collidedObject.tag == "FinishLine")
        {
            playSoundsPlayer.mute = playSoundsPlayer.mute;
            playWoosh.mute = playWoosh.mute;
            audioSlide.mute = audioSlide.mute;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].GetComponent<AudioSource>() != null)
                {
                    gameObjects[i].GetComponent<AudioSource>().mute = gameObjects[i].GetComponent<AudioSource>().mute;
                }
                allStop = true;
            }
        }
        else
        {
            objSize = collidedObject.GetComponent<Collider>().bounds.size;
        }
        //play enemy hit sound and stop all other sounds on hitting an enmy object.
        if (collidedObject.tag == "Enemy")
        {
            playSoundsPlayer.PlayOneShot(soundArray[2] as AudioClip);
            playSoundsPlayer.mute = playSoundsPlayer.mute;
            playWoosh.mute = playWoosh.mute;
            audioSlide.mute = audioSlide.mute;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].GetComponent<AudioSource>() != null && collidedObject.tag != "Enemy")
                {
                    gameObjects[i].GetComponent<AudioSource>().mute = gameObjects[i].GetComponent<AudioSource>().mute;
                }
            }
            allStop = true;
        }
        else if (collidedObject.name == "Bonus Star")
        {
            if (oldObject != collidedObject)
            {
                playSoundsPlayer.PlayOneShot(soundArray[1] as AudioClip);
                oldObject = collidedObject;
            }
        }
        else
        {
            playSound(collidedObject);
        }

        if (collidedObject.GetComponent<Transform>().position.y < 0)
        {
            playSoundWoosh(collidedObject);
        }
    }

    public void playSound(GameObject collidedObject)
    {
        if (allStop == false)
        {
            //Add audiosource to collided object to play sound from source of collision.
            if (collidedObject.GetComponent<AudioSource>() == null)
            {
                collidedObject.AddComponent<AudioSource>();
                collidedObject.GetComponent<AudioSource>().maxDistance = 2;
                collidedObject.GetComponent<AudioSource>().volume = 50;
            }

            //If the collided object can be seen in the current camera view, play the sounds.
            if (collidedObject.GetComponent<Renderer>().isVisible == true)
            {
                if ((objSize.x <= 2.0f && objSize.y <= 2.0f && objSize.z <= 2.0f) || playerLerpSpeed <= .5f)
                {
                    tapInt = Random.Range(0, 3);
                    collidedObject.GetComponent<AudioSource>().PlayOneShot(tapSounds[tapInt] as AudioClip);
                    playing++;
                }
                else if (playerLerpSpeed <= .75f)
                {
                    collidedObject.GetComponent<AudioSource>().PlayOneShot(soundArray[4] as AudioClip);
                }

                else if (playerLerpSpeed <= 1.0f)
                {
                    collidedObject.GetComponent<AudioSource>().PlayOneShot(soundArray[3] as AudioClip);
                }

                if (playing > 60)
                {
                    StartCoroutine(startWait());
                }

                if (collidedObject.GetComponent<Transform>().position.y < 0.0f)
                {
                    playSoundWoosh(collidedObject);
                }
            }
            //StartCoroutine(audioSourceStop(collidedObject));
        }
    }
    public void playSoundWoosh(GameObject collidedObject)
    {
        if (allStop == false)
        {
            if (canPlayWoosh == true)
            {
                playWoosh.PlayOneShot(soundArray[8] as AudioClip);
                canPlayWoosh = false;
                StartCoroutine(wooshLimit());
            }
        }
    }

    //If the player is in contact with an object and pushing it, a slide sound will play. When contact ends, the slide sound will stop.
    public void playSlide()
    {
        if (allStop == false)
        {
            if (!beingPushed)
            {
                if (gameObject.GetComponent<Transform>().localScale.x > 1.9f || gameObject.GetComponent<Transform>().localScale.y > 1.9f || gameObject.GetComponent<Transform>().localScale.z > 1.9f)
                {
                    audioSlide.volume = 0.75f;
                    audioSlide.Play();
                    beingPushed = true;
                }
                else
                {
                    audioSlide.volume = 0.5f;
                    audioSlide.Play();
                    beingPushed = true;
                }
            }
        }
    }
    public void stopSlide()
    {
        if (beingPushed)
        {
            audioSlide.Stop();
            beingPushed = false;
        }
    }

    //Coroutine waitToDetect is used to prevent an overload of sounds all at once.
    IEnumerator startWait()
    {
        Debug.Log("The start wait has been triggered!");
        yield return new WaitForSecondsRealtime(0.25f);
        playing = 0;
    }

    //Wait X.X seconds, then stop an unused audiosource on a collided gameobject.
    IEnumerator audioSourceStop(GameObject collidedObject)
    {
        yield return new WaitForSecondsRealtime(1.0f);
        collidedObject.GetComponent<AudioSource>().Stop();
        collidedObject.GetComponent<ColliderDetector>().canPlay = false;
        collidedObject.GetComponent<ColliderDetector>().waitToDetect();
    }

    IEnumerator wooshLimit()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        canPlayWoosh = true;
    }

    /*
     * //Play sound based on SoundID
    public void soundPlay(int sourceID, int soundID)
    {
        if (soundID == -1)
        {
            tapSounds[sourceID].Stop();
        }
        else if (tapSounds[sourceID].isPlaying == true)
        {
        }
        else
        {
            tapSounds[sourceID].clip = soundArray[soundID];
            tapSounds[sourceID].Play();
            Debug.Log("sourceID = " + sourceID + ", soundID = " + soundID);
        }
    }

    //Stop the music, and get the current state of gameplay upon hitting the submit button.
    public void setState(int stateSet)
    {
        gameState = stateSet;
        GameMusic.Stop();
        StartCoroutine(winLose(gameState));
    }

    public IEnumerator winLose(int gameState)
    {
        //If gameState == 1, the player made a mistake and lost the game.
        // Else if gameState == 2, the player submitted a correct build wall and won.

        if (gameState == 1)
        {
            GameMusic.clip = Resources.Load("Sounds/Music/GameLose") as AudioClip;
            GameMusic.loop = false;
            GameMusic.Play();
            Debug.Log("The player lost");
        }
        else if (gameState == 2)
        {
            GameMusic.clip = Resources.Load("Sounds/Music/GameWin") as AudioClip;
            GameMusic.loop = false;
            GameMusic.Play();
            Debug.Log("The player won!");
        }
        Debug.Log("Beginning Check");
        yield return new WaitForSeconds(GameMusic.clip.length);
        GameMusic.clip = Resources.Load("Sounds/Music/GameOverMusic") as AudioClip;
        GameMusic.loop = true;
        GameMusic.Play();
        Debug.Log("GameMisoc is " + GameMusic + ".");
    }
    */
}
