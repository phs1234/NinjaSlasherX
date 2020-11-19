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
            zFoxGameObjectLoader fol = go.GetComponent<zFoxGameObjectLoader>();
            
            if (fol) {
                if (fol.loaded) {
                    loadedAll = true;
                    break;
                }    
            }
        }

        if (loadedAll) {
            Destroy(gameObject);
            return;
        }

        loaded = true;

        if (!loaded_Awake) {
            loaded_Awake = true;
            LoadGameObject(LoadGameObjectList_Awake, loadedGameObjectList_Awake);
        }
    }

    void Start() {
        if (!loaded_Start) {
            loaded_Awake = true;
            LoadGameObject(LoadGameObjectList_Start, loadedGameObjectList_Start);
        }
    }

    void Update() {
        if (!loaded_Update) {
            loaded_Update = true;
            LoadGameObject(LoadGameObjectList_Update, loadedGameObjectList_Update);
        }
    }

    void FixedUpdate() {
        if (!loaded_FixedUpdate) {
            loaded_FixedUpdate = true;
            LoadGameObject(LoadGameObjectList_Update, loadedGameObjectList_FixedUpdate);
        }    
    }

    void LoadGameObject(GameObject[] loadGameObjectList, Dictionary<string, GameObject> loadedGameObjectList) {
        DontDestroyOnLoad(this);

        foreach (GameObject go in loadGameObjectList) {
            if (go) {
                if (loadedGameObjectList.ContainsKey(go.name)) {

                } else {
                    GameObject goInstance = Instantiate(go) as GameObject;
                    
                    goInstance.name = go.name;
                    goInstance.transform.parent = gameObject.transform;
                    loadedGameObjectList.Add(go.name, goInstance);
                }
            }
        } 
    }
}
