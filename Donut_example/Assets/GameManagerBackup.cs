using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBackup : MonoBehaviour
{

    // define variables
    private GameObject DisplayText;
    public GameObject[] fixatePoints;
    public GameObject[] Cameras;
    public GameObject[] RivalObjects;

    int trial = 0;// for trial number
    int Percent = 0;//for displaying number of runs completed

    int[] testDuration = new int[2] { 15, 60 };// specify array for different durations
    int[] DepTestDur = new int[3] { 360, 180, 180 };
    int[] DepPracDur = new int[3] { 25, 20, 20 };

    bool BaseStart = false;// Baseline task hasn't started
    bool BaseEnd = false;// Baseline task completed
    bool depStart = false;// dep task hasn't started
    bool depEnd = false; // end of deprivation period
    bool ExpEnd = false; // End of experiment

    bool LeftTriggerOn = false;
    bool RightTriggerOn = false;

    private float timer = 0.0f; // timing variables
    private float timerV = 0.0f;
    private float timerH = 0.0f;

    //// Start is called before the first frame update
    void Start()
    {
        // assign values to variables. For GO, it's assigning the gameobject to the variable
        DisplayText = GameObject.Find("Text");

        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().lineSpacing = 15;
        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Level 1\nFixate on the dot and report the dominant line orientation in the central circle." +
                                                                        "\n\nPull and Hold the: \nRIGHT trigger when the HORIZONTAL lines are more salient \nLEFT trigger " +
                                                                        "when the VERTICAL lines are more salient.\n\nWhen you are ready, pull both triggers to proceed.";
        DisplayText.gameObject.GetComponent<MeshRenderer>().enabled = true;

        // render texture and turn off video components for L and R eyes first

        //var videoPlayerL = RivalObjects[0].GetComponent<UnityEngine.Video.VideoPlayer>();
        //videoPlayerL.playOnAwake = false;


        //var videoPlayerR = RivalObjects[1].GetComponent<UnityEngine.Video.VideoPlayer>();
        //videoPlayerR.playOnAwake = false;

        // disable fixation point 
        fixatePoints[0].SetActive(false);//made two fixation points because of layer use (to improve)
        fixatePoints[1].SetActive(false);

        // turn on or off appropriate cameras
        Cameras[0].SetActive(true);
        Cameras[1].SetActive(false);
        Cameras[2].SetActive(false);

    }


    // Update is called once per frame
    void Update()
    {
        ///// Start different co-routines based on the state of progress //////


        /////if both triggers are pulled...
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") >= 0.2f && Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") >= 0.2f)
        {

            if (!BaseStart && trial == 0)// if this is the first BR trial
            {
                StartCoroutine(RivalryTest(trial, BaseStart, depStart, depEnd)); // start BR co-routine

            }
            else if ((!BaseStart && trial > 0) || (BaseStart && !BaseEnd))// if this is the 2 or third BR trial in prac session OR main session
            {
                StopCoroutine(RivalryTest(trial, BaseStart, depStart, depEnd)); // stop previous rivalry co-routine
                StartCoroutine(RivalryTest(trial, BaseStart, depStart, depEnd)); // start new BR co-routine

            }
            else if (BaseEnd && !depEnd)
            {
                StopCoroutine(RivalryTest(trial, BaseStart, depStart, depEnd)); // stop previous rivalry co-routine
                StartCoroutine(DepTest(trial, depStart, depEnd));
            }
            else if (depEnd && !ExpEnd)
            {
                StopCoroutine(DepTest(trial, depStart, depEnd));
                StartCoroutine(RivalryTest(trial, BaseStart, depStart, depEnd)); // stop previous rivalry co-routine
            }
            else if (ExpEnd)
            {
                Application.Quit();
            }

        }




        // collect button presses for deprivation phase
        if (!depEnd && depStart)
        {
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") >= 0.2f)
            {
                timer = Time.time; Debug.Log(timer);
                /// code to concatenate
            }

        }

        // collect button presses for all BR test phases  
        // I hate this current method, but I can't seem to get triggers to work like as buttons 
        // and OVRinput doesn't work for me (sad face)

        if (depEnd || BaseStart)// if deprivation ended or if baseline task starts (recording only done in main task)
        {
            //// detect trigger pulled////
            if (Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") >= 0.2f)
            {
                LeftTriggerOn = true;
                timerV = Time.time;

            }

            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") >= 0.2f)
            {
                timerH = Time.time;
                RightTriggerOn = true;
            }

            //// detect trigger releases////
            if (Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") == 0.0f)
            {
                if (LeftTriggerOn)
                {
                    float timeVertical = Time.time - timerV;
                    LeftTriggerOn = false;
                }
            }


            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") == 0.0f)
            {
                if (RightTriggerOn)
                {
                    float timeVertical = Time.time - timerV;
                    RightTriggerOn = false;
                }
            }

        }

        ///// to quit application (for experimenter use, comment for actual app)

        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") >= 0.5f)
        {
            Application.Quit();
        }





    }



    //// BR co-routine ////

    private IEnumerator RivalryTest(int trial, bool BaseStart, bool depStart, bool depEnd) // rivalry task
    {
        // present reminders for 10 seconds (good for providing a gap between dep and BR phases too)
        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Reminder while task loads:\n\nMore salient HORIZONTAL lines:\nRIGHT trigger" +
                                                                        " \n\nMore salient VERTICAL lines:\nLEFT trigger";
        yield return new WaitForSeconds(5);

        Renderer rendL = RivalObjects[0].gameObject.GetComponent<Renderer>();
        rendL.material.mainTexture = Resources.Load("grating") as Texture;

        Renderer rendR = RivalObjects[1].gameObject.GetComponent<Renderer>();
        rendR.material.mainTexture = Resources.Load("grating") as Texture;
        RivalObjects[0].gameObject.GetComponent<MeshRenderer>().enabled = true;
        RivalObjects[1].gameObject.GetComponent<MeshRenderer>().enabled = true;


        // turn on stereo cameras and turn off main camera
        Cameras[0].SetActive(false);
        Cameras[1].SetActive(true);
        Cameras[2].SetActive(true);

        if ((!BaseStart) || !depStart) // if Baseline hasn't started or if dep hasn't started (all Prac)
        {
            yield return new WaitForSeconds(testDuration[0]);
        }
        else // during Baseline phase or assessment phase after dep End
        {
            yield return new WaitForSeconds(testDuration[1]);
        }

        // After tracking, turn on instructions again
        Cameras[0].SetActive(true);
        Cameras[1].SetActive(false);
        Cameras[2].SetActive(false);

        RivalObjects[0].gameObject.SetActive(false);
        RivalObjects[1].gameObject.SetActive(false);


        if (trial < 3) // if trial number is less than 3 (0, 1 and 2 are valid trials, because the increment is done after we finish trial 0, etc)
        {

            if (depEnd) // if this was tracking post-dep in main and prac task (when completed trials <3)
            {
                depEnd = false; // re-define as false, so we can enter the DepTest co-routine again
            }

            Percent = 100 * (trial + 1) / 3; // calculate percentage completed
            trial = trial + 1; // increment trial
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = Percent.ToString() + "% of current stage completed.\n\nPull both triggers to continue.";
            yield return trial; // return trial value
            yield return depEnd; // return depEnd value

        }
        else if (trial == 3) // if participant has done 3 trials
        {

            if (!BaseStart) // if this was the practice
            {
                DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "End of Practice.\n\n Pull both triggers to proceeed to the main task.";
                BaseStart = true; // signal to start baseline task
                trial = 0; // re-initialise trial number
                yield return BaseStart; // return Basestart (akin to [x]=somefun(x) in MATLAB)
                yield return trial; // return trial value
            }
            else if (BaseStart && !depEnd) // if this was the baseline task and not the tracking after deprivation
            {
                DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Time to load Level 2!\n\n Detect the rapid color changes of the central dot." +
                                                                                "\n\nPull the RIGHT trigger whenever the dot turns YELLOW. " +
                                                                                "\n\nPull both triggers to proceed.";
                BaseEnd = true; // signal end of baseline task
                trial = 0; // re-initialise trial number
                yield return BaseEnd; // return BaseEnd
                yield return trial; // return trial value

            }
            else if (!depStart && depEnd) // if this is the tracking after deprivation (during Prac)
            {
                DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "End of Practice.\n\n Pull both triggers to proceeed to the main task.";
                depStart = true; // signal to start dep baseline task
                depEnd = false; // re-define to false to enter DepTest co-routine again
                trial = 0; // re-initialise trial number
                yield return depStart; // return depStart (akin to [x]=somefun(x) in MATLAB)
                yield return trial; // return trial value
                yield return depEnd; // return depEnd value

            }
            else if (depStart && depEnd) // if this was the main task post-dep tracking, return ExpEnd as true to quit program
            {
                DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "All levels completed!\n\n Pull both triggers to quit the program.";
                ExpEnd = true;
                yield return ExpEnd;

            }

        }


    }


    //// Deprivation co-routine

    private IEnumerator DepTest(int trial, bool depStart, bool depEnd) // rivalry task
    {

        DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "\n\nReminder:" + "\n\nPull the RIGHT trigger ONCE when the central dot turns YELLOW.";

        yield return new WaitForSeconds(10);


        //// BEGIN DEPRIVATION PHASE/////

        // activate videoplayer component of Gameobject

        //var videoPlayerL = RivalObjects[0].GetComponent<UnityEngine.Video.VideoPlayer>();
        //var videoPlayerR = RivalObjects[1].GetComponent<UnityEngine.Video.VideoPlayer>();

        //videoPlayerL.isLooping = true;// set videos to loop
        //videoPlayerR.isLooping = true;

        //videoPlayerL.Play(); // now play video
        //videoPlayerR.Play();

        // turn on the right cameras
        Cameras[0].SetActive(false);
        Cameras[1].SetActive(true);
        Cameras[2].SetActive(true);

        // turn on the fixation points
        fixatePoints[0].SetActive(true);
        fixatePoints[1].SetActive(true);

        // vary deprivation duration depending on practice or experimental task
        if (!depStart)
        {
            yield return new WaitForSeconds(DepPracDur[trial]);
        }
        else if (depStart)
        {
            yield return new WaitForSeconds(DepTestDur[trial]);
        }

        ///// Interval between deprivation and test phase /////
        //videoPlayerR.Stop();
        //videoPlayerL.Stop();
        Cameras[0].SetActive(true);
        Cameras[1].SetActive(false);
        Cameras[2].SetActive(false);
        fixatePoints[0].SetActive(false);
        fixatePoints[1].SetActive(false);


        if (trial < 3)
        {
            trial = trial + 1;
            DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Track the orientation of the central circle again!\n\nMore salient HORIZONTAL lines: Pull and Hold RIGHT trigger" +
                                                                            " \n\nMore salient VERTICAL lines: Pull and Hold LEFT trigger" + "\n\nPull both triggers to proceed.";

            depEnd = true;// signal end of deprivation phase
            yield return trial;
            yield return depEnd;// return depEnd state
        }
        else if (trial == 3)// if participant has completed three trials
        {

            if (!depStart) // if main dep task hasn't started (end of practice)
            {
                DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "End of Practice.\n\n Pull both triggers to proceeed to the main task.";
                depStart = true;
                depEnd = true;
                trial = 0; //re-initialise trial number (only for end of practice)

                yield return trial;
                yield return depStart;
                yield return depEnd;

            }
            else if (depStart) // if dep main task has already started and we finished the third dep phase
            {
                DisplayText.gameObject.GetComponent<TMPro.TextMeshPro>().text = "Track the orientation of the central circle again!\n\nMore salient HORIZONTAL lines: Pull and Hold RIGHT trigger" +
                                                                           " \n\nMore salient VERTICAL lines: Pull and Hold LEFT trigger" + "\n\nPull both triggers to proceed.";

                depEnd = true;// signal end of deprivation phase
                yield return depEnd;// return depEnd state
            }

        }


    }


}




