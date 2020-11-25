using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppSound : MonoBehaviour {
    public static AppSound instance = null;

    [System.NonSerialized] public zFoxSoundManager fm;

    [System.NonSerialized] public AudioSource BGM_LOGO;
    [System.NonSerialized] public AudioSource BGM_TITLE;
    [System.NonSerialized] public AudioSource BGM_HISCORE;
    [System.NonSerialized] public AudioSource BGM_HISCORE_RANKIN;
    [System.NonSerialized] public AudioSource BGM_STAGEA;
    [System.NonSerialized] public AudioSource BGM_STAGEB;
    [System.NonSerialized] public AudioSource BGM_STAGEB_ROOMSAKURA;
    [System.NonSerialized] public AudioSource BGM_BOSSA;
    [System.NonSerialized] public AudioSource BGM_BOSSB;
    [System.NonSerialized] public AudioSource BGM_ENDING;

    [System.NonSerialized] public AudioSource SE_MENU_OK;
    [System.NonSerialized] public AudioSource SE_MENU_CANCLE;

    [System.NonSerialized] public AudioSource SE_ATK_A1;
    [System.NonSerialized] public AudioSource SE_ATK_A2;
    [System.NonSerialized] public AudioSource SE_ATK_A3;
    [System.NonSerialized] public AudioSource SE_ATK_B1;
    [System.NonSerialized] public AudioSource SE_ATK_B2;
    [System.NonSerialized] public AudioSource SE_ATK_B3;
    [System.NonSerialized] public AudioSource SE_ATK_ARIAL;
    [System.NonSerialized] public AudioSource SE_ATK_SYURIKEN;

    [System.NonSerialized] public AudioSource SE_HIT_A1;
    [System.NonSerialized] public AudioSource SE_HIT_A2;
    [System.NonSerialized] public AudioSource SE_HIT_A3;
    [System.NonSerialized] public AudioSource SE_HIT_B1;
    [System.NonSerialized] public AudioSource SE_HIT_B2;
    [System.NonSerialized] public AudioSource SE_HIT_B3;

    [System.NonSerialized] public AudioSource SE_MOV_JUMP;

    [System.NonSerialized] public AudioSource SE_ITEM_KOBAN;
    [System.NonSerialized] public AudioSource SE_ITEM_HYOUTAN;
    [System.NonSerialized] public AudioSource SE_ITEM_MAKIMONO;
    [System.NonSerialized] public AudioSource SE_ITEM_OHBAN;
    [System.NonSerialized] public AudioSource SE_ITEM_KEY;

    [System.NonSerialized] public AudioSource SE_OBJ_EXIT;
    [System.NonSerialized] public AudioSource SE_OBJ_OPENDOOR;
    [System.NonSerialized] public AudioSource SE_OBJ_SWITCH;
    [System.NonSerialized] public AudioSource SE_OBJ_BOXBROKEN;

    [System.NonSerialized] public AudioSource SE_CHECKPOINT;
    [System.NonSerialized] public AudioSource SE_EXPLOSION;

    string sceneName = "non";

    void Awake() {
        fm = GameObject.Find("zFoxSoundManager").GetComponent<zFoxSoundManager>();

        fm.CreateGroup("BGM");
        fm.SoundFolder = "Sounds/BGM/";
        BGM_LOGO = fm.LoadResourceSound("BGM", "Logo");
        BGM_TITLE = fm.LoadResourceSound("BGM", "Title");
        BGM_HISCORE = fm.LoadResourceSound("BGM", "HiScore");
        BGM_HISCORE_RANKIN = fm.LoadResourceSound("BGM", "HiScore_Rankin");
        BGM_STAGEA = fm.LoadResourceSound("BGM", "StageA");
        BGM_STAGEB = fm.LoadResourceSound("BGM", "StageB");
        BGM_STAGEB_ROOMSAKURA = fm.LoadResourceSound("BGM", "StageB_RoomSakura");
        BGM_BOSSA = fm.LoadResourceSound("BGM", "BossA");
        BGM_BOSSB = fm.LoadResourceSound("BGM", "BossB");
        BGM_ENDING = fm.LoadResourceSound("BGM", "Ending");

        fm.CreateGroup("SE");
        fm.SoundFolder = "Sounds/SE/";

        SE_MENU_OK = fm.LoadResourceSound("SE", "SE_Menu_Ok");
        SE_MENU_CANCLE = fm.LoadResourceSound("SE", "SEMU_Menu_Cancel");

        SE_ATK_A1 = fm.LoadResourceSound("SE", "SE_ATK_A1");
        SE_ATK_A2 = fm.LoadResourceSound("SE", "SE_ATK_A2");
        SE_ATK_A3 = fm.LoadResourceSound("SE", "SE_ATK_A3");
        SE_ATK_B1 = fm.LoadResourceSound("SE", "SE_ATK_B1");
        SE_ATK_B2 = fm.LoadResourceSound("SE", "SE_ATK_B2");
        SE_ATK_B3 = fm.LoadResourceSound("SE", "SE_ATK_B3");
        SE_ATK_ARIAL = fm.LoadResourceSound("SE", "SE_ATK_Arial");
        SE_ATK_SYURIKEN = fm.LoadResourceSound("SE", "SE_ATK_Syuriken");

        SE_HIT_A1 = fm.LoadResourceSound("SE", "SE_HIT_A1");
        SE_HIT_A2 = fm.LoadResourceSound("SE", "SE_HIT_A2");
        SE_HIT_A3 = fm.LoadResourceSound("SE", "SE_HIT_A3");

        SE_HIT_B1 = SE_HIT_A1;
        SE_HIT_B2 = SE_HIT_A2;
        SE_HIT_B3 = SE_HIT_A3;

        SE_MOV_JUMP = fm.LoadResourceSound("SE", "SE_MOV_Jump");

        SE_ITEM_KOBAN = fm.LoadResourceSound("SE", "SE_Item_Koban");
        SE_ITEM_HYOUTAN = fm.LoadResourceSound("SE", "SE_Item_Hyoutan");
        SE_ITEM_MAKIMONO = fm.LoadResourceSound("SE", "SE_Item_Makimono");
        SE_ITEM_OHBAN = fm.LoadResourceSound("SE", "SE_Item_Ohban");
        SE_ITEM_KEY = fm.LoadResourceSound("SE", "SE_Item_Key");

        SE_OBJ_EXIT = fm.LoadResourceSound("SE", "SE_OBJ_Exit");
        SE_OBJ_OPENDOOR = fm.LoadResourceSound("SE", "SE_OBJ_OpenDoor");
        SE_OBJ_BOXBROKEN = fm.LoadResourceSound("SE", "SE_OBJ_BoxBroken");

        SE_CHECKPOINT = fm.LoadResourceSound("SE", "SE_CheckPoint");
        SE_EXPLOSION = fm.LoadResourceSound("SE", "SE_Explosion");

        instance = this;
    }

    void Update() {
        if (sceneName != SceneManager.GetActiveScene().name) {
            sceneName = SceneManager.GetActiveScene().name;

            fm.SetVolume("BGM", SaveData.SoundBGMVolume);
            fm.SetVolume("SE", SaveData.SoundSEVolume);

            if (sceneName == "Menu_Logo") {
                BGM_LOGO.Play();
            } else if (sceneName == "Menu_Title") {
                if (!BGM_TITLE.isPlaying) {
                    fm.Stop("BGM");
                    BGM_TITLE.Play();
                    fm.FadeInVolume(BGM_TITLE, SaveData.SoundBGMVolume, 1.0f, true);
                }
            } else if (sceneName == "Menu_Option" || sceneName == "Menu_HiScore" || sceneName == "Menu_Option") {

            } else if (sceneName == "StageA") {
                // fadein 과 fadeout이 바뀐 것 같다 
                fm.FadeOutVolumeGroup("BGM", BGM_STAGEA, 0, 1, false);
                fm.FadeInVolume(BGM_TITLE, 1.0f, 1.0f, true);
                BGM_STAGEA.loop = true;
                BGM_STAGEA.Play();
            } else if (sceneName == "StageB_Room") {
                Debug.Log("벚꽃이 내려와");
                fm.Stop("BGM");
                BGM_STAGEB_ROOMSAKURA.loop = true;
                BGM_STAGEB_ROOMSAKURA.Play();
            } else if (sceneName == "StageB_Room_A" || sceneName == "StageB_Room_B" || sceneName == "StageB_Room_C") {
                fm.Stop("BGM");
                BGM_BOSSA.loop = true;
                BGM_BOSSA.Play();
            } else if (sceneName == "StageB_Boss") {
                fm.Stop("BGM");
                BGM_BOSSB.loop = true;
                BGM_BOSSB.Play();
            } else if (sceneName == "StageZ_Ending") {
                fm.Stop("BGM");
                BGM_ENDING.Play();
            } else {
                if (!BGM_STAGEB.isPlaying) {
                    fm.Stop("BGM");
                    BGM_STAGEB.loop = true;
                    BGM_STAGEB.Play();
                }
            }
        }
    }
}
