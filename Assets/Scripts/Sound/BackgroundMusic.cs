using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {

    public AudioSource audioSource;

    [Range(0, 1)]
    public float volume;

    void Start()
    {
        StartBackgroundMusic();    
    }

    public void StartBackgroundMusic()
    {
        StartCoroutine(startBackgroundMusic());
    }

    IEnumerator startBackgroundMusic()
    {
        /* Starting volume should be zero */
        audioSource.volume = 0;
        audioSource.Play();
        while(audioSource.volume < volume)
        {
            /* Exponential increase of volume */
            audioSource.volume = Mathf.MoveTowards(audioSource.volume,volume,Time.deltaTime*0.25f);
            yield return new WaitForEndOfFrame();
        }
    }

}
