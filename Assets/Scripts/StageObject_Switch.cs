using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject_Switch : MonoBehaviour
{
    public bool switchOn = false;
    public bool onlyOnce = false;
    public bool switchTurnMode = true;
    public bool switchObjectsTurnMode = true;
    public float resetTime = 0.0f;
    public bool switchObjectsInit = true;
    public GameObject[] switchOnObjects;
    public GameObject[] switchOffObjects;
    public GameObject[] switchInstantiateObjects;
    public bool switchInstantiateObjectsDestroy = true;

    [System.NonSerialized] public int switchTurnCount = 0;

    GameObject goLever;
    float switchTurnTime;
    GameObject[] switchInstantiateObjectsList;

    private void Awake()
    {
        goLever = transform.Find("Stage_Switch_Lever_1").gameObject;
        switchTurnTime = 0.0f;
        switchInstantiateObjectsList = new GameObject[switchInstantiateObjects.Length];

        SwitchOnGraphics(switchOn);
        if (switchObjectsInit) {
            SwitchOnObjects(switchOn, false);
        }
    }

    private void Update()
    {
        if (resetTime > 0.0f && switchTurnTime > 0.0f) {
            if (switchTurnTime + resetTime <= Time.fixedTime) {
                SwitchOnWork(!switchOn);
                switchTurnTime = 0.0f;
            }
        }
    }

    void SwitchOnGraphics(bool sw) {
        if (goLever) { 
            float r = (sw) ? 90.0f: 0.0f;
            goLever.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, r);
        }
    }

    void SwitchOnObjects(bool sw, bool turn) {
        if (!turn)
        {
            if (switchTurnMode)
            {
                foreach (GameObject switchOnObject in switchOnObjects)
                {
                    if (switchOnObject) switchOnObject.SetActive(sw);
                }

                foreach (GameObject switchOffObject in switchOffObjects)
                {
                    if (switchOffObject) switchOffObject.SetActive(!sw);
                }
            }
            else
            {
                foreach (GameObject switchOnObject in switchOnObjects)
                {
                    if (switchOnObject) switchOnObject.SetActive(true);
                }

                foreach (GameObject switchOffObject in switchOffObjects)
                {
                    if (switchOffObject) switchOffObject.SetActive(false);
                }
            }
        }
        else {
            foreach (GameObject switchOnObject in switchOnObjects)
            {
                if (switchOnObject) switchOnObject.SetActive(!switchOnObject.activeSelf);
            }

            foreach (GameObject switchOffObject in switchOffObjects)
            {
                if (switchOffObject) switchOffObject.SetActive(!switchOffObject.activeSelf);
            }
        }

        for (int i = 0; i < switchInstantiateObjects.Length; i++) {
            if (switchInstantiateObjects[i]) {
                if (sw)
                {
                    switchInstantiateObjectsList[i] = Instantiate(switchInstantiateObjects[i]) as GameObject;
                }
                else {
                    if (switchInstantiateObjectsDestroy) {
                        Destroy(switchInstantiateObjectsList[i]);
                    }
                }
            }
        }
    }

    void SwitchOnWork(bool sw)
    {
        switchOn = sw;
        SwitchOnGraphics(sw);
        SwitchOnObjects(sw, switchObjectsTurnMode);
        switchTurnTime = Time.fixedTime;
        switchTurnCount++;
    }

    public void SwitchOn(bool sw)
    {
        if (onlyOnce && switchTurnCount > 0) return;
        if (resetTime > 0.0f && switchTurnTime > 0.0f) return;
        SwitchOnWork(sw);
    }

    public void SwitchTurn()
    {
        SwitchOn(!switchOn);
    }
}
