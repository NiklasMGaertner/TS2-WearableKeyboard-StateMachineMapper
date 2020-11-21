using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SingleInputMapping
{
    // constructor for testing! (create to see structure)
    public SingleInputMapping(int mID, bool tMode, string iAction, string ttMode)
    {
        this.mapID = mID;
        this.transferMode = tMode;
        this.inputAction = iAction;
        this.transferToMode = ttMode;
    }

    // identifier for the accessed mapping | each possible input of the Tap Strap 2 is mapped to an Integer
    public int mapID;
        
    // the input that should be used in the active mode of the specific tap
    public string inputAction;

    // boolean to indicate whether the state/ mode should be changed
    public bool transferMode;
       
    // the stateID of the state that is aimed to transfer to
    public string transferToMode;
}
