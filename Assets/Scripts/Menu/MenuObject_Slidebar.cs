using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuObject_Slidebar : MonoBehaviour {
    public GameObject scriptObject;
    public string label;

    public GameObject slideObject;
    public GameObject anchorStart;
    public GameObject anchorEnd;

    public bool scrollMode = false;
    public bool slideMoveX = true;
    public bool slideMoveY = false;

    public float slideMoveAccelX = 1.0f;
    public float slideMoveAccelY = 1.0f;
    public float slideBreakX = 0.0f;
    public float slideBreakY = 0.0f;

    [System.NonSerialized] public Vector2 cursorPosition = Vector2.zero;

    Vector3 movSt;
    Vector3 movNow;
    Vector2 slideSize;

    void Start() {
        slideObject.transform.position = new Vector3(anchorStart.transform.position.x, anchorStart.transform.position.y, slideObject.transform.position.z);
        slideSize.x = anchorEnd.transform.position.x - anchorStart.transform.position.x;
        slideSize.y = anchorEnd.transform.position.y - anchorStart.transform.position.y;

        // 이건 뭐지?
        if (scrollMode) {
            anchorStart.transform.position -= new Vector3(slideSize.x, slideSize.y, 0.0f);
            anchorEnd.transform.position -= new Vector3(slideSize.x, slideSize.y, 0.0f);
        }
    }

    public void Init() {
        if (scriptObject != null) {
            scriptObject.SendMessage("Slidebar_Init", this);
        }
    }

    void Update() {
        if (scrollMode) {                               // 터치 검사 
            if (Input.touchCount > 0) {
                if (Physics2D.OverlapPoint(GetScreenPosition(Input.GetTouch(0).position)) != null) {
                    switch (Input.GetTouch(0).phase) {
                        case TouchPhase.Began:
                            movSt = GetScreenPosition(Input.GetTouch(0).position);
                            break;
                        case TouchPhase.Moved:
                            MoveSlide(GetScreenPosition(Input.GetTouch(0).position) - movSt);
                            movSt = GetScreenPosition(Input.mousePosition);
                            break;
                        case TouchPhase.Ended:
                            break;
                    }
                }
            } else if (Input.GetMouseButton(0)) {           // 마우스 검사
                if (Physics2D.OverlapPoint(GetScreenPosition(Input.mousePosition)) != null) {
                    if (Input.GetMouseButtonDown(0)) {
                        movSt = GetScreenPosition(Input.mousePosition);
                    }

                    if (Input.GetMouseButton(0)) {
                        MoveSlide(GetScreenPosition(Input.mousePosition) - movSt);
                        movSt = GetScreenPosition(Input.mousePosition);
                    }

                    if (Input.GetMouseButtonUp(0)) {
                    }
                }
            } else {
                MoveSlide(new Vector2(movNow.x * slideBreakX, movNow.y * slideBreakY));
            }
        } else {
            if (Input.touchCount > 0) {
                switch (Input.GetTouch(0).phase) {
                    case TouchPhase.Began:
                    case TouchPhase.Moved:
                        SetSlide(GetScreenPosition(Input.GetTouch(0).position));
                        break;
                }
            } else if (Input.GetMouseButton(0)) {
                SetSlide(GetScreenPosition(Input.mousePosition));                
            }
        }

        CheckSlide();
    }

    Vector3 GetScreenPosition(Vector3 touchPos) {
        touchPos.z = transform.position.z - Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(touchPos);
    }

    void MoveSlide(Vector2 mov) {
        movNow = mov;
        
        mov.x *= slideMoveX ? slideMoveAccelX : 0.0f;
        mov.y *= slideMoveY ? slideMoveAccelY : 0.0f;

        slideObject.transform.position += (Vector3)mov;

        if (scriptObject != null) {
            scriptObject.SendMessage("Slidebar_Drag", this);
        }
    }

    void SetSlide(Vector2 pos) {
        Collider2D col2D = Physics2D.OverlapPoint(pos);
        if (col2D != null) {
            if (col2D.transform.parent == transform) {
                float x = 0.0f;
                float y = 0.0f;

                if (slideSize.x != 0.0f) {
                    x = (pos.x - anchorStart.transform.position.x) / slideSize.x;
                }

                if (slideSize.y != 0.0f) {
                    y = (pos.y - anchorStart.transform.position.y) / slideSize.y;
                }

                SetPosition(new Vector2(x, y));
            }
        }

        if (scriptObject != null) {
            scriptObject.SendMessage("Slidebar_Drag", this);
        }
    }

    public void SetPosition(Vector2 pos) {
        cursorPosition = pos;
        
        float x = anchorStart.transform.position.x + slideSize.x * cursorPosition.x;
        float y = anchorStart.transform.position.y + slideSize.y * cursorPosition.y;

        slideObject.transform.position = new Vector3(x, y, 0.0f);
        CheckSlide();
    }

    void CheckSlide() {
        if (slideObject.transform.position.x < anchorStart.transform.position.x) {
            slideObject.transform.position = new Vector3(
                anchorStart.transform.position.x, 
                slideObject.transform.position.y, 
                slideObject.transform.position.z);
        }

        if (slideObject.transform.position.x > anchorEnd.transform.position.x) {
            slideObject.transform.position = new Vector3(
                anchorEnd.transform.position.x,
                slideObject.transform.position.y,
                slideObject.transform.position.z);
        }

        if (slideObject.transform.position.y > anchorStart.transform.position.y) {
            slideObject.transform.position = new Vector3(
                slideObject.transform.position.x,
                anchorStart.transform.position.y,
                slideObject.transform.position.z);
        }

        if (slideObject.transform.position.y < anchorEnd.transform.position.y) {
            slideObject.transform.position = new Vector3(
                slideObject.transform.position.x,
                anchorEnd.transform.position.y,
                slideObject.transform.position.z);
        }

        Vector3 ofsPos = slideObject.transform.position - anchorStart.transform.position;

        cursorPosition = Vector2.zero;

        if (slideSize.x != 0.0f) {
            cursorPosition.x = ofsPos.x / slideSize.x;
        }

        if (slideSize.y != 0.0f) {
            cursorPosition.y = ofsPos.y / slideSize.y;
        }
        
        // 이건 뭐지.. 
        if (scrollMode) {
            cursorPosition = Vector2.one - cursorPosition;
        }

        cursorPosition.x = Mathf.Clamp01(cursorPosition.x);
        cursorPosition.y = Mathf.Clamp01(cursorPosition.y);
    }
}
