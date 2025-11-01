using UnityEngine;

public class SkipButton : MonoBehaviour
{
    void Update(){
        if (Input.GetButtonDown("P1_B1"))
        {
            Jukebox.SetPlaybackPosition(SongLoader.LoadedSong.Info.SongStart - 2);
            Maestro.StartTime = (float)AudioSettings.dspTime - (SongLoader.LoadedSong.Info.SongStart - 2 + Maestro.GlobalOffset);
            gameObject.SetActive(false);
            SFXManager.PlaySelectSound();
        }
    }
}