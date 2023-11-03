using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class AmbienceCrossfade : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public float fadeTime = 1.0f;

    [System.Serializable]
    public class FadeEvent : UnityEvent<float> { }

    public FadeEvent OnFadeStart;
    public UnityEvent OnFadeComplete;

    public void Start()
    {
        audioSource1.Play();
    }

    //CALL THIS TO SWITCH BETWEEN MONSTER TRACK AND NORMAL TRACK
    public void SwitchTracks()
    {
        if (audioSource1.isPlaying)
        {
            FadeToTrack2();
        } else
        {
            FadeToTrack1();
        }
    }



    private void FadeToTrack2()
    {
        StartCoroutine(FadeToTrack(audioSource1, audioSource2, fadeTime));
    }

    private void FadeToTrack1()
    {
        StartCoroutine(FadeToTrack(audioSource2, audioSource1, fadeTime));
    }

    IEnumerator FadeToTrack(AudioSource oldTrack, AudioSource newTrack, float fadeTime)
    {
        float t = 0.0f;
        newTrack.Play();
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            oldTrack.volume = Mathf.Lerp(0.65f, 0.0f, t / fadeTime);
            newTrack.volume = Mathf.Lerp(0.0f, 0.65f, t / fadeTime);
            OnFadeStart.Invoke(t / fadeTime);
            yield return null;
        }
        oldTrack.Stop();
        OnFadeComplete.Invoke();
    }

    //for testing only
    
    private void Update()
    {
        // Check for a key press (e.g., "Space" key).
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Call your function here.
            SwitchTracks();
        }
    }
}