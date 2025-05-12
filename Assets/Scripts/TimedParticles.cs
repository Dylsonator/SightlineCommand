using System.Collections;
using UnityEngine;

public class TimedParticles : MonoBehaviour {
    public void StartTimer(float seconds) {
        StartCoroutine(DestroyAfterSeconds(seconds));
    }

    private IEnumerator DestroyAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
