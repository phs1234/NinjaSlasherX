using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity2DSortingLayer : MonoBehaviour
{
    public string sortingLayerName = "Front";
    public int sortingOrder = 0;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Renderer>().sortingLayerName = sortingLayerName;
        GetComponent<Renderer>().sortingOrder = sortingOrder;
    }
}
