using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pandora.AI {
    [System.Serializable]
    public class AIParticles {
        public string tag = "";
        public ParticleSystem particle;
    }
    public class AIVisualsLib : MonoBehaviour {

        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private Light muzzleLight;
        [SerializeField] private GameObject shell;
        [SerializeField] private AudioClip[] shotSounds;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AIParticles[] particles;
        [SerializeField] private ParticleSystem defaultParticle;

        public void GunShot()
        {
            StartCoroutine(GunShotE());
        }
        IEnumerator GunShotE()
        {
            muzzleFlash.Play();
            muzzleLight.enabled = true;
            PlayShotSound();
            yield return new WaitForSeconds(0.05f);
            muzzleLight.enabled = false;
        }
        void PlayShotSound()
        {
            audioSource.clip = shotSounds[Random.Range(0, shotSounds.Length - 1)];
            audioSource.Play();
        }
        public ParticleSystem GetParticle(string tag) {
            foreach (AIParticles particleLib in particles)
            {
                if (particleLib.tag == tag)
                {
                    return particleLib.particle;
                }
            }
            return defaultParticle;
        }
    }
}