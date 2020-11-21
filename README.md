# TS2-WearableKeyboard-StateMachineMapper

## Functions

These classes can be used in conjunction with the [Tap Strap 2 Unity plugin](https://github.com/TapWithUs/tap-unity-plugin)
in order to expand the number of outputs that can be accessed with the Tap Strap 2.
To be specific, these classes simulate the framework for a state machine.

## Code

The code is arranged in the following classes.
InputStateHandler.cs contains the logic of the state machine.
The classes JsonStates.cs / SingleState.cs / SingleInputMapping.cs are used as objects 
of which instances are created to contain all information of the state machine.

### InputStateHandler.cs

The interface for usage (return of the state machine) is in the actionProcessing method.

The structure of the state machine is saved to the instances of the object classes in the InputStateHandler.cs class in the InitialReadData method at the Start(). 
The information for this structure is extracted from a file in JSON format.
This file has to be included in the path specified. 

The structure of the file in JSON format should be the following (with example values):
```
{"singleStates": [
    {
        "inputMapping": [
            {
                "transferMode": true,
                "transferToMode": "A",
                "mapID": 1,
                "inputAction": ""
            },
            {
                "transferMode": false,
                "transferToMode": "",
                "mapID": 2,
                "inputAction": "b"
            }
        ],
        "stateID": "default"
    },
    {
        "inputMapping": [{
            "transferMode": false,
            "transferToMode": "default",
            "mapID": 1,
            "inputAction": "a"
        }],
        "stateID": "A"
    }
]}

```
In this case there are two states, with two possible taps in the "default" state and one in the "A" state.
For more information look at classe below.

### JsonStates.cs

Object class containing 
* the list of all states (SingleState.cs).

### SingleState.cs 

Object class containing 
* a stateID (string) for identification 
* a list of possible inputs (SingleInputMapping.cs).

### SingleInputMapping.cs 

Object class containing 
* a mapID (int) for identification
* an inputAction (string) the string that should be returned in the active state/mode with thehardware input received
* a transferMode (boolean) containg the information whether or not with this action the state/mode should be changed
* a transferToState (string) containing the next state accessed in case of transferMode

## Use

* Download the [Tap Strap 2 Unity plugin](https://github.com/TapWithUs/tap-unity-plugin) and become familiar with it.

Testing if it works for you
* Include the above mentioned classes in the Assets/TAP/Test directory of the Tap Strap 2 Unity plugin.
* Include a comptible file at the filepath specified in the InputStateHandler.cs class. In case you did not change it the stadard path accessed is .../<User>/AppData/LocalLow/<Company>/<ProjectName>/input_states.json

When working correctly:
* remove / put into comments debug logs
* use actionProcessing method of the InputStateHandler.cs as an interface for further processing
