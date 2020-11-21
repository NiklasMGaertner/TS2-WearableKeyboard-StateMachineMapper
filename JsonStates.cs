using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JsonStates
{
    // constructor for testing! (create to see structure)
    public JsonStates(List<SingleState> sStates)
    {
        this.singleStates = sStates;
    }

    // the List containing all states
    public List<SingleState> singleStates;
}
