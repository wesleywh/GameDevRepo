using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CyberBullet.Events {
    public class MusicManager : MonoBehaviour
    {
        public AudioSource source1;                   // assign via inspector
        public AudioSource source2;              // assign via inspector
        public float fadeTime = 4.0f;

        private Queue clips = new Queue();
        private Queue volumes = new Queue();
        private bool running = false;

        public void CrossFade(AudioClip clip, float maxVolume)
        {
            clips.Enqueue(clip);
            volumes.Enqueue(maxVolume);
            if (running == false)
            {
                StartCoroutine(CrossFadeClips());
            }
        }
        public void CrossFade(AudioClip clip)
        {
            clips.Enqueue(clip);
            volumes.Enqueue(0.5f);
            if (running == false)
            {
                StartCoroutine(CrossFadeClips());
            }
        }
        IEnumerator CrossFadeClips ()
        {
            AudioClip targetClip = (AudioClip)clips.Dequeue();
            float targetVolume = (float)volumes.Dequeue();

            AudioSource oldSource = (source1.isPlaying) ? source1 : source2;
            AudioSource newSource = (source1.isPlaying) ? source2 : source1;

            float startTime = Time.unscaledTime;
            float elapsed;
            float orgVol = oldSource.volume;

            newSource.clip = targetClip;
            newSource.Play();
            while ((Time.unscaledTime - startTime) < fadeTime)
            {
                elapsed = (Time.unscaledTime - startTime) / fadeTime;
                oldSource.volume = Mathf.Lerp(orgVol, 0f, elapsed); 
                newSource.volume = Mathf.Lerp(0f, targetVolume, elapsed);
                yield return null;
            }
            oldSource.Stop();
            if (clips.Count > 0)
            {
                StartCoroutine(CrossFadeClips());
            }
            else
            {
                running = false;
            }
            yield return null;
        }

    }
}