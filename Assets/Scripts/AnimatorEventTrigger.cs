using System;
using UnityEngine;

public class AnimatorEventTrigger : MonoBehaviour {
    [SerializeField]
    private Animator[] animators;

    [SerializeField]
    private GameObject particlePrefab;

    [SerializeField]
    private Transform particlePoint;

    [SerializeField]
    private ParticleSystem[] particles;

    [SerializeField]
    private GameObject[] activatedObjects;

    [SerializeField]
    private AudioSource audioSource;

    public Action AnimEvent;

    private string triggerSave;

    public void PlayTrigger(string trigger) {
        foreach (var animator in animators) {
            animator.SetTrigger(trigger);
        }
    }

    public void SetTrigger(string trigger) {
        triggerSave = trigger;
    }


    public void PlayTriggerIndex(int index) {
        animators[index].SetTrigger(triggerSave);
    }

    public void PlayParticle() {
        foreach (var system in particles) {
            system.Play();
        }
    }

    public void ActivateObjects() {
        foreach (var actObject in activatedObjects) {
            actObject.SetActive(true);
        }
    }

    public void DeactivateObjects() {
        foreach (var actObject in activatedObjects) {
            actObject.SetActive(false);
        }
    }

    public void PlaySoundEffect(AudioClip sound) {
        audioSource.PlayOneShot(sound);
    }

    public void CallEvent() {
        AnimEvent?.Invoke();
    }

    public void SpawnParticles(float duration) {
        GameObject particleInstance = Instantiate(particlePrefab, particlePoint.position, particlePrefab.transform.rotation);
        particleInstance.AddComponent<TimedParticles>().StartTimer(duration);
    }
}
