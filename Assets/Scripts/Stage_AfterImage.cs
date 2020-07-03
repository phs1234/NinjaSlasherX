using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_AfterImage : MonoBehaviour
{
    public SpriteRenderer spriteSrc;
    public bool afterImageEnabled;

    // Start is called before the first frame update
    void Start()
    {
        afterImageEnabled = true;       // 이러면 public 이 의미가 있나? 
        StartCoroutine("AfterImageUpdate");
    }

    IEnumerator AfterImageUpdate() {
        while (true) {
            while (afterImageEnabled) {
                SpriteRenderer spriteCopy = Instantiate(spriteSrc) as SpriteRenderer;
                spriteCopy.transform.position = spriteSrc.transform.position;
                spriteCopy.transform.localScale = spriteSrc.transform.parent.transform.localScale;
                spriteCopy.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                spriteCopy.sortingLayerName = "Char";
                spriteCopy.sortingOrder = 1;
                spriteCopy.GetComponent<Stage_Shadow>().enabled = false;
                SpriteRenderer[] spList = spriteCopy.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sp in spList) {
                    if (sp.name == "Shadow") {
                        sp.enabled = false;
                    }
                }

                Destroy(spriteCopy.gameObject, 0.3f);
                yield return new WaitForSeconds(0.05f);
             }
            yield return new WaitForSeconds(1.0f);
        }
    }
}
