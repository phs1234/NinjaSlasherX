using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject_DogPile : MonoBehaviour
{
    public GameObject[] enemyList;
    public GameObject[] destoryObjectList;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckEnemy", 0.0f, 1.0f);
    }

    void CheckEnemy() {
        bool flag = true;

        foreach (GameObject enemy in enemyList) {
            if (enemy != null) {
                flag = false;
            }
        }

        if (flag)
        {
            foreach (GameObject destroyObject in destoryObjectList)
            {
                destroyObject.AddComponent<Effect_FadeObject>();
                destroyObject.SendMessage("FadeStart");
                Destroy(destroyObject, 1.0f);
            }

            CancelInvoke("CheckEnemy");
        }
    }
}
