using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SingleState
{
    // constructor for testing! (create to see structure)
    public SingleState(string sID, List<SingleInputMapping> sIM)
    {
        this.stateID = sID;
        this.inputMapping = sIM;
    }

    // identifier for the state | at least one state "default" (first state accessed)
    public string stateID;
    
    // list of all actions in the state
    public List<SingleInputMapping> inputMapping;

}
