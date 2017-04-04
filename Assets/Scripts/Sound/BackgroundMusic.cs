using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {

    public AudioSource audioSource;

    [Range(0, 1)]
    public float volume;

    public void StartBackgroundMusic()
    {
        audioSource.Play();
        StartCoroutine(startBackgroundMusic());
    }

    IEnumerator startBackgroundMusic()
    {
        /* Starting volume should be zero */
        audioSource.volume = 0;
        while(audioSource.volume < volume)
        {
            /* Exponential increase of volume */
            audioSource.volume = Mathf.MoveTowards(audioSource.volume,volume,Time.deltaTime*0.25f);
            yield return new WaitForEndOfFrame();
        }
    }

}
