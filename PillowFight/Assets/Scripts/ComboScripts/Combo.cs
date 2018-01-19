
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Combo {
    public string comboName = "Default";
    public enum BUTTON {
        Jump,
        Slap,
        Throw
    }
    public BUTTON[] buttons = new BUTTON[3];

    [Range(0, 5)]
    public int pillowAmount;
}

