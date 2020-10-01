using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //scene index
    // readonly int sceneIdx =1; // 0 for experiment, 1 for retrieving world units for visual angle calculation

    // define variables
    private GameObject DisplayText;
    public GameObject[] fixatePoints;
    public GameObject[] RivalObjects;
    private GameObject EmailObject;
    public Camera[] Cameras;

    // integer variables for sequence of events
    int trial = 1;// for trial number (starts from 1)
    int RespIdentifier = 0;
    int BaselineSection = 1;

    // integer variables for 
    int returnValue = 0;
    int frame = 1;
    int interval = 18;

    readonly int[] testDuration = new int[2] { 15,60 };// specify array for different durations
    readonly int[] DepTestDur = new int[3] { 360, 180, 180 };
    readonly int[] DepPracDur = new int[3] { 30, 15, 15 };

    bool depStart = false;
    bool testEnd = false;// Baseline task completed
    bool depEnd = false; // end of deprivation period
    bool tracking = false;
    bool BRphase = false;
    bool instructflag = false;
    bool timesup = false;

    bool LeftTriggerOn = false;
    bool RightTriggerOn = false;

    private bool writtenFile = false; // initialise file writing to false
    private float timer = 0.0f; // timing variables
    private float timerV = 0.0f;
    private float timerH = 0.0f;

    private string fileDir;
    private string testTime;
    private string fileName;
    private System.IO.TextWriter file = null;
    private string HeadsetID;
    private string outputText;
    private string operatingSystem;

    //// this method creates the folder.
    public string NewFileName()
    {
        HeadsetID = SystemInfo.deviceUniqueIdentifier;
        operatingSystem = SystemInfo.operatingSystem;

        if (operatingSystem.Contains("Mac")) // i haven't got a mac that is compatible with Quest, so i'm not sure if this is needed. But just in case.
        {
            fileDir = Application.persistentDataPath + "/Data/";
        }
        else
        {
            fileDir = Application.persistentDataPath + "\\Data\\";
        }

        testTime = System.DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
        fileName = fileDir + "_SN_" + HeadsetID + "_" + testTime + "_" + ".txt"; // concatenate the strings and variables together
        file = new System.IO.StreamWriter(fileName, true);

        return fileName;
    }

    //// Start is called before the first frame update
    void Start()
    {
        //SceneManager.LoadScene(sceneIdx); // I added this recently, hence the odd use in the experimental code

        // assign values to variables. For GO, it's assigning the gameobject to the variable
        DisplayText = GameObject.Find("Text");

        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().lineSpacing = 15;
        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "There are 2 parts to this task.\nIn Part 1, maintain fixation and report the dominant line orientation in the central circle." +
                                                                        "\n\nPull and Hold: \nThe LEFT trigger when the HORIZONTAL lines are more salient, or\nthe RIGHT trigger " +
                                                                        "when the VERTICAL lines are more salient.\n\nPush the RIGHT Thumbstick Upwards to begin Part 1.";
        DisplayText.gameObject.GetComponent<MeshRenderer>().enabled = true;

        // render texture and turn off video components for L and R eyes first
        var videoPlayerL = RivalObjects[0].GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayerL.playOnAwake = false;

        var videoPlayerR = RivalObjects[1].GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayerR.playOnAwake = false;

        Renderer rendL = RivalObjects[0].GetComponent<Renderer>();
        rendL.material.mainTexture = Resources.Load("gratingL") as Texture;

        Renderer rendR = RivalObjects[1].GetComponent<Renderer>();
        rendR.material.mainTexture = Resources.Load("gratingR") as Texture;

        // center gameobjects in front of camera
        //var camX =  Cameras[0].transform.position.x;
        //var camY = Cameras[0].transform.position.y;

        //RivalObjects[0].transform.position = new Vector3(camX/2, camY/2, transform.position.z);
        //RivalObjects[1].transform.position = new Vector3(camX/2, camY/2, transform.position.z);

        //fixatePoints[0].transform.position = new Vector3(camX / 2, camY / 2, transform.position.z);

        // disable email object first
        EmailObject = GameObject.Find("EmailObject");
        EmailObject.gameObject.SetActive(false);

        // disable fixation point 
        fixatePoints[0].SetActive(false);//made two fixation points because of layer use (to improve)
                                         //fixatePoints[1].SetActive(false);

        // turn on or off appropriate cameras
        Cameras[0].enabled = true;
        Cameras[1].enabled = false;
        Cameras[2].enabled = false;

        NewFileName();// turn on file recording method
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") >= 0.5f && !tracking)
        {

            if (!testEnd)
            {
                BRphase = true;
                StartCoroutine(RivalryTest(trial)); // start BR co-routine
                tracking = true; // do not initiate co-routines during tracking
            }

            if (testEnd && !depEnd)
            {
                trial += 1;

                if (trial < 7)
                {
                    BRphase = true;
                    BaselineSection = 1;

                    StopCoroutine(RivalryTest(trial)); // stop previous rivalry co-routine
                    StartCoroutine(RivalryTest(trial));
                    tracking = true; // do not initiate co-routines during tracking
                }

                if (trial >= 7 & trial < 13)
                {

                    BaselineSection = 0;
                    StopCoroutine(RivalryTest(trial)); // stop previous rivalry co-routine
                    depStart = true; BRphase = false;
                    StartCoroutine(DepTest(trial));
                    tracking = true; // do not initiate co-routines during tracking


                }

                if (trial == 13)
                {
                    Application.Quit();

                }

            }

            if (depEnd && depStart) // think about it...
            {
                BRphase = true;
                StopCoroutine(DepTest(trial));
                StartCoroutine(RivalryTest(trial));
                tracking = true; // do not initiate co-routines during tracking
            }

        }


        if (tracking && BRphase && !instructflag)
        {
            if (!timesup)
            {
                // for right trigger
                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") >= 0.5f && !RightTriggerOn)
                {
                    RightTriggerOn = true;
                    timerV = Time.time;
                }

                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") == 0.0f)
                {
                    if (RightTriggerOn)
                    {
                        float timeVertical = Time.time - timerV;
                        RespIdentifier = 2;
                        outputText = timeVertical + "," + RespIdentifier + "," + trial + "," + BaselineSection;
                        file.WriteLine(outputText);// writes next line in text file
                        RightTriggerOn = false;
                    }
                }

                // for left trigger

                if (Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") >= 0.5f && !LeftTriggerOn)
                {
                    timerH = Time.time;
                    LeftTriggerOn = true;
                }


                if (Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") == 0.0f)
                {
                    if (LeftTriggerOn)
                    {
                        float timeHorizontal = Time.time - timerH;
                        RespIdentifier = 1;
                        outputText = timeHorizontal + "," + RespIdentifier + "," + trial + "," + BaselineSection;
                        file.WriteLine(outputText);// writes next line in text file
                        LeftTriggerOn = false;
                    }
                }


            }
            else // catch last button press
            {
                if (LeftTriggerOn)
                {
                    float timeHorizontal = Time.time - timerH;
                    RespIdentifier = 1;
                    outputText = timeHorizontal + "," + RespIdentifier + "," + trial + "," + BaselineSection;
                    file.WriteLine(outputText);// writes next line in text file
                    LeftTriggerOn = false;
                }

                if (RightTriggerOn)
                {

                    float timeVertical = Time.time - timerV;
                    RespIdentifier = 2;
                    outputText = timeVertical + "," + RespIdentifier + "," + trial + "," + BaselineSection;
                    file.WriteLine(outputText);// writes next line in text file
                    RightTriggerOn = false;
                }
            }



        }



        if (tracking && !BRphase && !instructflag)
        {

            frame += 1;


            if (frame % interval == 0 && fixatePoints[0].activeSelf)
            {
                int ranInt = Random.Range(0, 20);
                returnValue = fixatePoints[0].GetComponent<ChangeFixationColour>().TurnYellow(ranInt); // return one value is enough
            }


            timer = Time.time; // extract time
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") >= 0.5f)// if participant pulls trigger
            {
                RespIdentifier = 3;// indicate perceived colour change
            }

            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") == 0.0f)// if participant doesn't pull trigger
            {
                RespIdentifier = 0;// indicate perceived colour change
            }

            // regardless of whether or not participant pulled trigger, record data
            outputText = timer + "," + RespIdentifier + "," + trial + "," + returnValue;
            file.WriteLine(outputText);// writes next line in text file


        }


        // exit button for piloting and debugging phases
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") >= 0.5f && Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical") >= 0.5f)
        {
            Application.Quit();
        }

    }



    //// BR co-routine ////

    private IEnumerator RivalryTest(int trial) // rivalry task
    {
        instructflag = true;
        yield return instructflag;
       

        // vary instructions displayed based on
        if (trial < 4)// if it's the baseline practice trials
        {
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Part 1 - Practice\n\nAs long as the HORIZONTAL lines are more salient:\nPull and Hold LEFT trigger" +
                                                                                    " \n\nAs long as the VERTICAL lines are more salient:\nPull and Hold RIGHT trigger";
        }

        if (trial >= 4 && trial < 7) // if it's the baseline main trials
        {
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Part 1 - Main\n\nAs long as the HORIZONTAL lines are more salient:\nPull and Hold LEFT trigger" +
                                                                                    " \n\nAs long as the VERTICAL lines are more salient:\nPull and Hold RIGHT trigger";
        }

        if (trial >= 7 && trial < 10) // if it's the post-deprivation trials
        {
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Part 2 - Practice\n\nTrack the orientation of the central circle again!\n\nMore salient HORIZONTAL lines:\nLEFT trigger" +
                                                                                    " \n\nMore salient VERTICAL lines:\nRIGHT trigger";
        }

        if (trial >= 10)
        {
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Part 2 - Main\n\nTrack the orientation of the central circle again!\n\nMore salient HORIZONTAL lines:\nLEFT trigger" +
                                                                                    " \n\nMore salient VERTICAL lines:\nRight trigger";
        }

        if (trial >= 7)
        {
            yield return new WaitForSeconds(10);
        }
        else // if within baseline phase
        {
            yield return new WaitForSeconds(6);
        }



        // Present BR stimuli
        instructflag = false;
        yield return instructflag;
        timesup = false;
        yield return timesup;

        Cameras[0].enabled = false;
        Cameras[1].enabled = true;
        Cameras[2].enabled = true;


        if (trial < 4 || trial >= 7 && trial < 10) // if its a practice phase
        {
            yield return new WaitForSeconds(testDuration[0]);
        }
        if ((trial >= 4 && trial < 7) || trial >= 10) // if its a main task phase
        {
            yield return new WaitForSeconds(testDuration[1]);
        }
        
        timesup = true; // must be defined before instruct flag or it won't go into the BR loop
        yield return timesup;

        // End of BR presentation
        instructflag = true;
        yield return instructflag;
       

        yield return new WaitForSeconds(1);
        Cameras[0].enabled = true;
        Cameras[1].enabled = false;
        Cameras[2].enabled = false;

        if (trial < 12) // for part 1   
        {
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Trial completed.\n\nPush the RIGHT Thumbstick Upwards to proceed.";
        }

        if (trial == 12)
        {
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "All tasks completed.\n\nPush the RIGHT Thumbstick Upwards to save your responses and quit the program.";
        }


        testEnd = true;
        tracking = false;
        depEnd = false;
        yield return testEnd;
        yield return tracking;
        yield return depEnd;

    }


    //// Deprivation co-routine

    private IEnumerator DepTest(int trial) // rivalry task
    {
        instructflag = true;
        yield return instructflag;

        if (trial < 10)
        {
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Part 2 - Practice\n\nPull and Release the RIGHT trigger when the central dot turns YELLOW.\n\n";
        }

        if (trial >= 10)
        {
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Part 2 - Main\n\nPull and Release the RIGHT trigger when the central dot turns YELLOW.\n\n";
        }

        yield return new WaitForSeconds(6);


        instructflag = false;
        yield return instructflag;

        // present deprivation stimuli
        // RivalObjects[1].transform.Rotate(0.0f, 0.0f,-90.0f, Space.Self); // rotate rival object (because it was rotated to generate rivalry)

        Renderer rendL = RivalObjects[0].GetComponent<Renderer>();
        //rendL.material.mainTexture = Resources.Load("Leftvidtex") as Texture;
        Renderer rendR = RivalObjects[1].GetComponent<Renderer>();
        //rendR.material.mainTexture = Resources.Load("Rightvidtex") as Texture;

        var videoPlayerR = RivalObjects[1].GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayerR.Play();
        var videoPlayerL = RivalObjects[0].GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayerL.Play();

        yield return new WaitForSeconds(1);
        fixatePoints[0].gameObject.SetActive(true);

        Cameras[0].enabled = false;
        Cameras[1].enabled = true;
        Cameras[2].enabled = true;

        // vary deprivation duration depending on practice or experimental task
        if (trial < 10)
        {
            int DurIdx = trial - 7;
            yield return new WaitForSeconds(DepPracDur[DurIdx]);
        }

        if (trial >= 10)
        {
            int DurIdx = trial - 10;
            yield return new WaitForSeconds(DepTestDur[DurIdx]);
        }

        ///// Interval between deprivation and test phase /////
        videoPlayerL.Stop();
        videoPlayerR.Stop();

        instructflag = true;
        yield return instructflag;

        Cameras[0].enabled = true;
        Cameras[1].enabled = false;
        Cameras[2].enabled = false;

        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "50% of the current trial completed.\n\nPush the RIGHT Thumbstick Upwards to continue."; ;

        //RivalObjects[1].transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
        depEnd = true;// signal end of deprivation phase
        tracking = false; // reset tracking
        yield return depEnd;// return depEnd state
        yield return tracking;
        //fixatePoints[0].gameObject.SetActive(false);
        fixatePoints[0].gameObject.SetActive(false);

        rendL.material.mainTexture = Resources.Load("gratingL") as Texture;
        rendR.material.mainTexture = Resources.Load("gratingR") as Texture;

    }



    private void OnApplicationQuit()
    {
        if (!writtenFile & file != null)
        {
            file.Close();
            writtenFile = true;
        }

        EmailObject.gameObject.SetActive(true);
        EmailObject.GetComponent<SendEmail>().Emailer(fileName);
    }



}




