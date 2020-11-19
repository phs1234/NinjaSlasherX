using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum zFOXUID_TYPE { 
    NUMBER,
    GUID
}

public class zFoxUID : MonoBehaviour {
    public zFOXUID_TYPE type = zFOXUID_TYPE.NUMBER;
    public string uid = "(non)";
}
