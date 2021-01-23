using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        transform.rotation =  Quaternion.LookRotation(new Vector3(2, 1, 0), Vector3.down);
    }

    // Update is called once per frame
    void Update() {

    }
}
