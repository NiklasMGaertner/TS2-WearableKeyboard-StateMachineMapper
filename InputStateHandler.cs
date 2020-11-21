using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputStateHandler : MonoBehaviour
{
    // -------------------------------------------------------------------------------------------------
    // original Tap Strap 2 attributes START
    // (code from TapInputTest)
    public Text LogText;

    private ITapInput tapInputManager;

    private bool mouseHIDEnabled;

    private string connectedTapIdentifier = "";
    //
    // original Tap Strap 2 attributes END
    // -------------------------------------------------------------------------------------------------

    // -------------------------------------------------------------------------------------------------
    // added attributes START
    // 

    // filename and path for used Json 
    // (.../<User>/AppData/LocalLow/<Company>/<ProjectName> folder)
    string filename = "input_states.json";
    string path;

    // used for testing (save produce Json)
    // JsonStates testJsonStates;
    // used for testing (save produce Json)
    // SingleState testSingleState;

    //
    JsonStates inputStates;
    // removed constructor (was used for testing structure)
    // JsonStates inputStates = new JsonStates();


    // all states read
    JsonStates outputStates;

    // current state
    SingleState currentState;
    // SingleState currentState = new SingleState();

    // 
    // Added attributes END
    // -------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        InitialReadData();
        tapStrapStuffOnStart();
    }

    // -------------------------------------------------------------------------------------------------
    // CHANGED Lines of code of the Tap Strap 2 Unity plugin (some slightly adapted - see comments)
    // START

    // all lines of code executed in the original Tap Strap 2 Unity plugin on start
    void tapStrapStuffOnStart()
    {
        tapInputManager = TapInputManager.Instance;

        tapInputManager.OnTapInputReceived += onTapped;
        tapInputManager.OnTapConnected += onTapConnected;
        tapInputManager.OnTapDisconnected += onTapDisconnected;
        tapInputManager.OnMouseInputReceived += onMoused;
        tapInputManager.OnAirGestureInputReceived += onAirGestureInputReceived;
        tapInputManager.OnTapChangedAirGestureState += onTapChangedState;
        tapInputManager.OnRawSensorDataReceived += onRawSensorDataReceived;
        tapInputManager.EnableDebug();
        tapInputManager.SetDefaultControllerWithMouseHIDMode(true);
        mouseHIDEnabled = false;
    }

    private void Log(string text)
    {
        if (LogText != null)
        {
            LogText.text += string.Format("{0}\n", text);
        }
        Debug.Log(text);
    }

    void onMoused(string identifier, int vx, int vy, bool isMouse)
    {
        Log("onMoused" + identifier + ", velocity = (" + vx + "," + vy + "), isMouse " + isMouse);
    }

    void onTapped(string identifier, int combination)
    {
        bool[] arr = TapCombination.toFingers(combination);
        Log("onTapped : " + identifier + ", " + combination);

        // added modeIdentifier - turn received Tap combination into an integer (used for mapID)
        int modeIdentifier = 0;
        if (arr[0]) { modeIdentifier += 1; }
        if (arr[1]) { modeIdentifier += 2; }
        if (arr[2]) { modeIdentifier += 4; }
        if (arr[3]) { modeIdentifier += 8; }
        if (arr[4]) { modeIdentifier += 16; }

        actionProcessing(modeIdentifier);
    }

    void onAirGestureInputReceived(string tapIdentifier, TapAirGesture gesture)
    {
        // added modeIdentifier - turn received Tap combination into an integer (used for mapID)
        int modeIdentifier = 31;
        switch (gesture)
        {
            case TapAirGesture.OneFingerUp:
                modeIdentifier += 1;
                break;
            case TapAirGesture.TwoFingersUp:
                modeIdentifier += 2;
                break;
            case TapAirGesture.OneFingerDown:
                modeIdentifier += 3;
                break;
            case TapAirGesture.TwoFingersDown:
                modeIdentifier += 4;
                break;
            case TapAirGesture.OneFingerLeft:
                modeIdentifier += 5;
                break;
            case TapAirGesture.TwoFingersLeft:
                modeIdentifier += 6;
                break;
            case TapAirGesture.OnefingerRight:
                modeIdentifier += 7;
                break;
            case TapAirGesture.TwoFingersRight:
                modeIdentifier += 8;
                break;
            case TapAirGesture.IndexToThumbTouch:
                modeIdentifier += 9;
                break;
            case TapAirGesture.MiddleToThumbTouch:
                modeIdentifier += 10;
                break;
            default:
                break;
        }
        actionProcessing(modeIdentifier);
        // Log("OnAirGestureInputReceived: " + tapIdentifier + ", " + gesture.ToString());
    }

    //
    // changed lines END
    // -------------------------------------------------------------------------------------------------

    // -------------------------------------------------------------------------------------------------
    // Additional code I added START
    //

    // processes hardware input to new inputs/ outputs
    void actionProcessing(int modeIdentifier)
    {
        // look for mapID connected to received input
        int iterateMapID = 0;
        SingleInputMapping mapAction = new SingleInputMapping(0, false, null, null);
        foreach (SingleInputMapping im in currentState.inputMapping)
        {
            iterateMapID = im.mapID;
            if (iterateMapID == modeIdentifier)
            {
                mapAction = im;
                break;
            }
        }
        if (mapAction.inputAction != null)
        {
            // This is where the to be used input action has been successfully determined
            // INTERFACE for application should then be created here!
            Debug.Log("Called action: " + mapAction.inputAction);
        }
        // transfer to the mode specified in the mapping
        if (mapAction.transferMode)
        {
            ReadData(mapAction.transferToMode);
        }
    }

    
    // accessed when the state mod should be change - looks for new/next mode
    // and saves it to currentState variable
    void ReadData(string transferToState)
    {
        string iterateStateID;
        List<SingleState> singleStateList = outputStates.singleStates;
        foreach (SingleState n in singleStateList)
        {
            iterateStateID = n.stateID;
            if (iterateStateID.Equals(transferToState))
            {
                currentState = n;
                Log("Mode changed to " + currentState.stateID);
                break;
            }

        }
        if (currentState == null)
        {
            Debug.Log("No <" + transferToState + "> state found in given Json.");
        }
    }

    // read file and determine current state
    void InitialReadData()
    {
        // path is set here
        path = Application.persistentDataPath + "/" + filename;
        Debug.Log(path);
        try
        {
            if (System.IO.File.Exists(path))
            {
                string jsonData = System.IO.File.ReadAllText(path);
                outputStates = JsonUtility.FromJson<JsonStates>(jsonData);
                // Debug.Log(inputStates.singleStates);
                List<SingleState> singleStateList = outputStates.singleStates;
                string currentStateID;
                foreach (SingleState n in singleStateList)
                {
                    currentStateID = n.stateID;
                    if (currentStateID.Equals("default"))
                    {
                        currentState = n;
                        Debug.Log("State found successfull.");
                        break;
                    }

                }
                if (currentState == null)
                {
                    Debug.Log("No <default> state found in given Json.");
                }
            }
            else
            {
                Debug.Log("File does not exist.");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Unknown exception: " + e.Message);
        }
    }

    //
    // Additional code I added END
    // -------------------------------------------------------------------------------------------------

    // -------------------------------------------------------------------------------------------------
    // Unused code START
    // 

    // Update is called once per frame
    //void Update()
    //{
    // testing with keyboard
    // keyboardTesting()

    //}

    //void keyboardTesting()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //saveData();
    //}
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //actionProcessing(1);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad0))
    //    {
    //actionProcessing(2);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad1))
    //    {
    //actionProcessing(3);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad2))
    //    {
    //actionProcessing(4);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad3))
    //   {
    //actionProcessing(5);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad4))
    //    {
    //actionProcessing(6);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad5))
    //    {
    //actionProcessing(7);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad6))
    //    {
    //actionProcessing(8);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad7))
    //    {
    //actionProcessing(9);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad8))
    //    {
    //actionProcessing(10);
    //}
    //    if (Input.GetKeyDown(KeyCode.Keypad9))
    //    {
    //actionProcessing(11);
    //}
    //}

    //void saveData()
    //{
    //createJson();
    //testIt();
    //string testJson = JsonUtility.ToJson(inputStates, true);
    //System.IO.File.WriteAllText(path, testJson);
    //}

    //void createJson()
    //{
    // List<SingleInputMapping> testSingleList = new List<SingleInputMapping>();
    //List<SingleState> testJsonList = new List<SingleState>();
    //testJsonList.Add(new SingleState("default", createSingleInputMapping()));
    //testJsonList.Add(new SingleState("state B", createSingleInputMapping()));
    //testJsonStates = new JsonStates(testJsonList);
    //}

    //List<SingleInputMapping> createSingleInputMapping()
    //{
    //List<SingleInputMapping> testMapping = new List<SingleInputMapping>();
    //testMapping.Add(new SingleInputMapping(0, true, "0", ""));
    //    for (int i = 1; i < 32; i++)
    //    {
    //testMapping.Add(new SingleInputMapping(i, false, i.ToString(), "-"));
    //}
    //    return testMapping;
    //}

    // delete this later or put into comment!
    //void testIt()
    //{
    //inputStates = testJsonStates;
    //}

    //
    // Unused code END
    // -------------------------------------------------------------------------------------------------


    // -------------------------------------------------------------------------------------------------
    // Unedited original Tap Strap 2 Unity plugin code START
    //

    void onTapConnected(string identifier, string name, int fw)
    {
        Debug.Log("onTapConnected : " + identifier + ", " + name + ", FW: " + fw);
        Log("onTapConnected : " + identifier + ", " + name);
        this.connectedTapIdentifier = identifier;
    }

    void onTapDisconnected(string identifier)
    {
        Debug.Log("UNITY TAP CALLBACK --- onTapDisconnected : " + identifier);
        Log("UNITY TAP CALLBACK --- onTapDisconnected : " + identifier);
        if (identifier.Equals(this.connectedTapIdentifier))
        {
            this.connectedTapIdentifier = "";
        }
    }

    void onTapChangedState(string tapIdentifier, bool isAirGesture)
    {
        // Log("onTapChangedState: " + tapIdentifier + ", " + isAirGesture.ToString());

    }

    void onRawSensorDataReceived(string tapIdentifier, RawSensorData data)
    {
        //RawSensorData Object has a timestamp, type and an array points(x,y,z).
        if (data.type == RawSensorData.DataType.Device)
        {
            // Fingers accelerometer.
            // Each point in array represents the accelerometer value of a finger (thumb, index, middle, ring, pinky).
            Vector3 thumb = data.GetPoint(RawSensorData.iDEV_THUMB);

            if (thumb != null)
            {
                // Do something with thumb.x, thumb.y, thumb.z
            }
            // Etc... use indexes: RawSensorData.iDEV_THUMB, RawSensorData.iDEV_INDEX, RawSensorData.iDEV_MIDDLE, RawSensorData.iDEV_RING, RawSensorData.iDEV_PINKY
        }
        else if (data.type == RawSensorData.DataType.IMU)
        {
            // Refers to an additional accelerometer on the Thumb sensor and a Gyro (placed on the thumb unit as well).
            Vector3 gyro = data.GetPoint(RawSensorData.iIMU_GYRO);
            if (gyro != null)
            {
                // Do something with gyro.x, gyro.y, gyro.z
            }
            // Etc... use indexes: RawSensorData.iIMU_GYRO, RawSensorData.iIMU_ACCELEROMETER
        }
        // -------------------------------------------------
        // -- Please refer readme.md for more information --
        // -------------------------------------------------
    }

    //
    // Unedited original Tap Strap 2 Unity plugin code END
    // -------------------------------------------------------------------------------------------------


}
