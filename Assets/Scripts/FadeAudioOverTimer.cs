using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAudioOverTimer : MonoBehaviour
{
    public AudioSource source;
    public Vector2 audioLevels;
    public float length;
    public float delay;

    private void OnEnable()
    {
        source.volume = audioLevels.x;
        source.Play();
        StartCoroutine(VolumeReduce());
    }

    IEnumerator VolumeReduce()
    {
        yield return new WaitForSeconds(delay);
        float elapsedTime = 0f;
        while (elapsedTime < length) {
            source.volume = Mathf.Lerp(audioLevels.x, audioLevels.y, elapsedTime / length);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        source.volume = audioLevels.y;
    }
}
