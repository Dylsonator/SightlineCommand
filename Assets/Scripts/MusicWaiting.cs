using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicWaiting : MonoBehaviour
{
    public List<AudioClip> Songs = new List<AudioClip>();
    public AudioSource AudioSource;

    private void Start()
    {
        StartCoroutine(Song1());
    }
    IEnumerator Song1()
    {
        AudioSource.PlayOneShot(Songs[0]);
        yield return new WaitForSeconds(160);
        StartCoroutine(Song2());
    }
    IEnumerator Song2()
    {
        AudioSource.PlayOneShot(Songs[1]);
        yield return new WaitForSeconds(174);
        StartCoroutine(Song3());
    }
    IEnumerator Song3()
    {
        AudioSource.PlayOneShot(Songs[2]);
        yield return new WaitForSeconds(136);
        StartCoroutine(Song1());
    }     
}
