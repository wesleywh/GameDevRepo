using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberBullet.AI {
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
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
                muzzleLight.enabled = true;
            }
            PlayShotSound();
            yield return new WaitForSeconds(0.05f);
            if (muzzleFlash != null)
            {
                muzzleLight.enabled = false;
            }
        }
        void PlayShotSound()
        {
            if (audioSource == null)
                return;
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
            if (defaultParticle == null)
                return null;
            else 
                return defaultParticle;
        }
        public void PlayParticle(ParticleSystem particle, RaycastHit hit)
        {
            if (particle != null)
            {
                ParticleSystem spawned = Instantiate(particle, hit.point, Quaternion.LookRotation(hit.normal)) as ParticleSystem;
                spawned.Play();
                Destroy(spawned.transform.gameObject, 2.0f);
            }
        }
        public void SpawnParticle(string tag, RaycastHit hit)
        {
            PlayParticle(GetParticle(tag), hit);
        }
        public void PlayMuzzleFlash()
        {
            StartCoroutine(GunShotE());
        }
    }
}