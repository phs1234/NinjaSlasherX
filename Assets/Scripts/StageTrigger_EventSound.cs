using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTrigger_EventSound : MonoBehaviour
{
    public string playGroup = "BGM";
    public string playAudio = "";
    public bool loop = true;
    public bool stopPlayGroup = true;

    void OnTriggerEnter2D_PlyaerEvent(Collider2D collision)
    {

        // 이걸 왜 하는 지 모르겠다.. 왜 !가 들어가지??
        if (stopPlayGroup) {
            if (!AppSound.instance.fm.FindAudioSource(playGroup, playAudio).isPlaying) {
                AppSound.instance.fm.FadeOutVolumeGroup(playGroup, playAudio, 0.0f, 1.0f, false);
            }
        }

        if (playAudio != "") {
            AppSound.instance.fm.SetVolume(playGroup, playAudio, SaveData.SoundBGMVolume);
            AppSound.instance.fm.PlayDontOverride(playGroup, playAudio, loop);
        }
    }
}
