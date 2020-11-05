using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zFoxGameObjectLoader : MonoBehaviour
{
    public GameObject[] LoadGameObjectList_Awake;
    public GameObject[] LoadGameObjectList_Start;
    public GameObject[] LoadGameObjectList_Update;
    public GameObject[] LoadGameObjectList_FixedUpdate;

    [System.NonSerialized] public Dictionary<string, GameObject> loadedGameObjectList_Awake = new Dictionary<string, GameObject>();
    [System.NonSerialized] public bool loaded_Awake = false;
    [System.NonSerialized] public Dictionary<string, GameObject> loadedGameObjectList_Start = new Dictionary<string, GameObject>();
    [System.NonSerialized] public bool loaded_Start = false;
    [System.NonSerialized] public Dictionary<string, GameObject> loadedGameObjectList_Update = new Dictionary<string, GameObject>();
    [System.NonSerialized] public bool loaded_Update = false;
    [System.NonSerialized] public Dictionary<string, GameObject> loadedGameObjectList_FixedUpdate = new Dictionary<string, GameObject>();
    [System.NonSerialized] public bool loaded_FixedUpdate = false;

    bool loaded = false;

    void Awake() {
        bool loadedAll = false;
        
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        foreach (GameObject go in gos) { 
            
        }
    }
}
