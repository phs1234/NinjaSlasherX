using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Title : MonoBehaviour {
    string jumpSceneName;

    void Start() {
        if (!SaveData.CheckGamePlayData()) {
            GameObject.Find("MenuButton_Continue").SetActive(false);
        } else {
            GameObject.Find("MenuButton_New").transform.localScale = Vector3.one * 1.0f;
        }

        zFoxFadeFilter.instance.FadeIn(Color.black, 1.0f);
    }

    void Button_Play(MenuObject_Button button) {
        SaveData.continuePlay = false;
        PlayerController.initParam = true;
        PlayerController.checkPointEnabled = false;

        zFoxFadeFilter.instance.FadeOut(Color.white, 1.0f);
        AppSound.instance.SE_MENU_OK.Play();
        jumpSceneName = "StageA";
        Invoke("SceneJump", 1.2f);
    }

    void Button_Continue(MenuObject_Button button) {
        SaveData.continuePlay = true;
        PlayerController.initParam = false;

        zFoxFadeFilter.instance.FadeOut(Color.white, 1.0f);
        jumpSceneName = SaveData.LoadContinueSceneName();
        Invoke("SceneJump", 1.2f);
    }

    void Button_HiScore(MenuObject_Button button) {
        zFoxFadeFilter.instance.FadeOut(Color.black, 0.5f);
        AppSound.instance.SE_MENU_OK.Play();
        jumpSceneName = "Menu_Hiscore";
        Invoke("SceneJump", 1.2f);
    }

    void Button_Option(MenuObject_Button button) {
        zFoxFadeFilter.instance.FadeOut(Color.black, 0.5f);
        AppSound.instance.SE_MENU_OK.Play();
        jumpSceneName = "Menu_Option";
        Invoke("SceneJump", 1.2f);
    }

    void SceneJump() {
        Application.LoadLevel(jumpSceneName);
    }
}
