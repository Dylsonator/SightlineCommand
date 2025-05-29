using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Music : MonoBehaviour //made by Dylan
{
    private int MuteSounds = 0;
    private float Volume;
    public AudioSource MusicSource;
    public Toggle muteToggle;
    public Slider Slider;
    private void Start()
    {
        
        MuteSounds = PlayerPrefs.GetInt("MuteCheck");
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        MusicChange();
        
    }
    private void Update()
    {
        ChangeVolume();
    }
    public void MusicChange()
    {
        if (muteToggle != null)
        {
            if (MuteSounds == 1)
            {
                MusicSource.Pause(); //keeps a stored value to either mute or unmute music          
                muteToggle.isOn = false;
            }
            if (MuteSounds == 0)
            {
                MusicSource.Play();
                muteToggle.isOn = true;
            }
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
    public void ChangeVolume()
    {
        if (Slider != null)
        {
            AudioListener.volume = Slider.value;
            PlayerPrefs.SetFloat("Volume", Slider.value);
        }
    }
}
