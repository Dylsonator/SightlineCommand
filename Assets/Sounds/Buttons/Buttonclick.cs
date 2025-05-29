using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttonclick : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] sounds;
    public AudioSource source;

    public void clickbutton()
    {
        source.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
    }
}
