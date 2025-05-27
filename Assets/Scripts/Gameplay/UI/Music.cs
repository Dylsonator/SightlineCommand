using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour //made by Dylan
{
    private int MuteSounds = 0;
    public AudioSource MusicSource;
    private void Start()
    {
        MuteSounds = PlayerPrefs.GetInt("MuteCheck");
        MusicChange();
    }
    public void MusicChange()
    {
        if (MuteSounds == 1)
        {
            MusicSource.Pause(); //keeps a stored value to either mute or unmute music          
        }
        if (MuteSounds == 0) {
            MusicSource.Play();
        }
    }

    public void MUTE(bool Check)
    {
        Check = !Check;
        if (Check == true)
        {
            PlayerPrefs.SetInt("MuteCheck", 1);
            MuteSounds = PlayerPrefs.GetInt("MuteCheck");//using playerprefs to check music is muted or not
        }
        if (Check == false) {
            PlayerPrefs.SetInt("MuteCheck", 0);
            MuteSounds = PlayerPrefs.GetInt("MuteCheck");
        }
        Debug.Log(PlayerPrefs.GetInt("MuteCheck"));
    }
}
