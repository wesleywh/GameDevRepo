using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameDevRepo {
    namespace Controllers {
        [Serializable]
        public class SoundClipOptions {
        	public AudioClip[] soundClips = null;
        	[Range(0,1)]
        	public float volume = 1.0f;
        	public bool loop = false;
        }
        [Serializable]
        public class FootStepSoundClips {
            public string materialName = "";
            public AudioClip[] soundClips = null;
            [Range(0,1)]
            public float volume = 1.0f;
        }

        [RequireComponent(typeof(AudioSource))]
        public class PlayerSoundController : MonoBehaviour {

            #region Variables
        	[SerializeField] private AudioSource ambiance = null;
        	[SerializeField] private AudioSource actions = null;
        	[SerializeField] private MovementController mc = null;
        	[Header("Swimming")]
        	public SoundClipOptions aboveWaterAmbiance = null;
        	public SoundClipOptions underwaterAmbiance = null;
        	public SoundClipOptions enterWater = null;
        	public SoundClipOptions exitWater = null;
            [Header("General")]
            public SoundClipOptions[] effort = null;
            [Header("FootSteps")]
            public FootStepSoundClips[] footsteps = null;
             
        	//for swimming
        	private bool playingAboveSounds = false;
        	private bool playedUnderWater = false;
        	private bool isSwimming = false;
            #endregion

        	void Start () {
        		if (actions == null)
        			actions = this.GetComponent<AudioSource> ();
        		if (ambiance == null)
        			ambiance = this.GetComponent<AudioSource> ();
        		if (mc == null)
        			mc = this.GetComponent<MovementController> ();
        	}

        	void Update () {
        		//for swimming
        		if (isSwimming != mc.swimming && mc.swimming == true) {
        			isSwimming = mc.swimming;
        			PlayEnterWater ();
        		} else {
        			isSwimming = mc.swimming;
        		}
        		if (mc.underWater == true && playedUnderWater == false) {
        			EnableUnderwaterAmbiance ();
        		} 
        		if (playedUnderWater == true && mc.underWater == false) {
        			playingAboveSounds = false;
        			PlayExitWater ();
        		}
        		if (playingAboveSounds == false && mc.swimming == true && mc.underWater == false) {
        			playingAboveSounds = true;
        			EnableAboveWaterAmbiance ();
        		} else {
        			isSwimming = mc.swimming;
        		}

        		//for general
        		if (mc.swimming == false && mc.underWater == false) {
        			removeAllWaterSounds ();
        		}
        	}
            #region WaterSounds
            private void removeAllWaterSounds() {
        		if (ambiance.volume > 0) {
        			ambiance.volume -= Time.deltaTime;
        		}
        	}
        	private void EnableAboveWaterAmbiance() {
        		if (aboveWaterAmbiance.soundClips.Length < 1) {
        			ambiance.Stop ();
        			return;
        		}
        		ambiance.clip = aboveWaterAmbiance.soundClips [UnityEngine.Random.Range (0, aboveWaterAmbiance.soundClips.Length)];
        		ambiance.volume = aboveWaterAmbiance.volume;
        		ambiance.loop = aboveWaterAmbiance.loop;
        		ambiance.Play ();
        	}
        	private void EnableUnderwaterAmbiance() {
        		playedUnderWater = true;
        		if (underwaterAmbiance.soundClips.Length < 1) {
        			ambiance.Stop ();
        			return;
        		}
        		ambiance.clip = underwaterAmbiance.soundClips [UnityEngine.Random.Range (0, underwaterAmbiance.soundClips.Length)];
        		ambiance.volume = underwaterAmbiance.volume;
        		ambiance.loop = underwaterAmbiance.loop;
        		ambiance.Play ();
        	}
        	private void PlayEnterWater() {
        		if (enterWater.soundClips.Length < 1)
        			return;
        		actions.clip = enterWater.soundClips [UnityEngine.Random.Range (0, enterWater.soundClips.Length)];
        		actions.volume = enterWater.volume;
        		actions.loop = enterWater.loop;
        		actions.Play ();
        	}
        	private void PlayExitWater() {
        		playedUnderWater = false;
        		if (exitWater.soundClips.Length < 1)
        			return;
        		actions.clip = exitWater.soundClips [UnityEngine.Random.Range (0, exitWater.soundClips.Length)];
        		actions.volume = exitWater.volume;
        		actions.loop = exitWater.loop;
        		actions.Play ();
        	}
            #endregion
            #region FootSteps
            public void PlayFootStep(string material) {
                if (String.IsNullOrEmpty(material))
                    return;
                foreach (FootStepSoundClips step in footsteps)
                {
                    if (step.materialName == material)
                    {
                        actions.clip = step.soundClips[UnityEngine.Random.Range(0, step.soundClips.Length)];
                        actions.loop = false;
                        actions.volume = step.volume;
                        actions.Play();
                        break;
                    }
                }
            }
            #endregion
        }
    }
}