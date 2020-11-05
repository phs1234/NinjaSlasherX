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
        if (stopPlayGroup) {
            if (!AppSound.instance.fm.FindAudioSource(playGroup, playAudio).isPlaying) {
                AppSound.instance.fm.FadeOutVolumeGroup(playGroup, playAudio, 0.0f, 1.0f, false);
            }
        }

        if (playAudio != "") {
            AppSound.instance.fm.SetVolume(playGroup, playAudio, 1.0f);
            AppSound.instance.fm.PlayDontOverride(playGroup, playAudio, loop);
        }
    }
}
