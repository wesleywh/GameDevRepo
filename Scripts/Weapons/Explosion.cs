using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pandora.Controllers;

namespace Pandora {
    namespace Interactables {
        public class Explosion : MonoBehaviour {

        	[SerializeField] float delayDestroyObject = 1.0f;
        	[SerializeField] float delayExplosion = 3.0f;
        	[SerializeField] float checkRadius = 10.0f;
        	[SerializeField] float hitRadius = 5.0f;
        	[SerializeField] float power = 5f;
        	[SerializeField] float upwardForce = 0f;
        	[SerializeField] float damage = 50.0f;
        	[SerializeField] ParticleSystem[] explosionEffect;
        	[SerializeField] AudioClip[] explosion;
        	[SerializeField] AudioSource overrideSource = null;
        	[SerializeField] Light explosionLight = null;
        	private bool explode = false;
        	private bool exploded = false;
        	private float timer = 0.0f;
        	private AudioSource asource;

        	void Start() {
        		if (!overrideSource)
        			asource = this.GetComponent<AudioSource> ();
        		else
        			asource = overrideSource;

        		if (explosionLight)
        			explosionLight.enabled = false;
        	}
        	// Update is called once per frame
        	void Update () {
        		timer = (exploded == false) ? timer + Time.deltaTime : 0;
        		if (timer >= delayExplosion)
        			explode = true;
        		if (explode == true) {
        			explode = false;
        			exploded = true;
        			StartCoroutine(DisplayLight ());
        			PlayParticleEffect ();
        			PlayExposionSound ();
        			ApplyHitEffects ();
        			Destroy (this.gameObject, delayDestroyObject);
        		}
        	}

        	IEnumerator DisplayLight() {
        		if (explosionLight) {
        			explosionLight.enabled = true;
        			yield return new WaitForSeconds (0.1f);
        			explosionLight.enabled = false;
        		}
        	}

        	void PlayParticleEffect() {
        		if (explosionEffect.Length > 0) {
        			foreach (ParticleSystem effect in explosionEffect) {
        				GameObject explosion = Instantiate (effect.transform.gameObject, this.transform.position, Quaternion.identity);
        				explosion.GetComponent<ParticleSystem> ().Play ();
        				Destroy (explosion, 3.0f);
        			}
        		}
        	}

        	void PlayExposionSound() {
        		if (explosion.Length > 0 ) {
        			asource.clip = explosion [Random.Range (0, explosion.Length)];
        			asource.Play();
        		}
        	}

        	void ApplyHitEffects() {
        		Vector3 explosionPos = transform.position;
        		Collider[] colliders = Physics.OverlapSphere (explosionPos, checkRadius);
        		foreach (Collider hit in colliders) 
        		{
        			if(hit == null)
        				continue;
        			if (hit.tag == "Player") {
        				GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<CameraShake> ().ShakeCamera (7 , 1);
        			}
        			if (hit.GetComponent<Rigidbody>())
        			{
        				hit.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, hitRadius, upwardForce);
        			}
        			if (hit.GetComponent<Health> () && Vector3.Distance(hit.transform.position, this.transform.position) <= hitRadius) {
                        hit.transform.root.GetComponent<Health> ().ApplyDamage (damage);
        			}
        		}
        	}
        }
    }
}