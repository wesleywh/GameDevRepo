using UnityEngine;
using System.Collections;
using TeamUtility.IO;				//Custom Input Manager

namespace Pandora {
    namespace Controllers {
        public class BreathingController : MonoBehaviour {
        	[SerializeField] private float sprintBreathingDelay = 1f;
        	[SerializeField] private float transitionTime = 2.0f;
        	[SerializeField] private AudioClip[] hardBreathing;
        	[SerializeField] private AudioClip[] hardToCalmBreathing;
        	[SerializeField] private AudioClip[] effort;
        	[SerializeField] private AudioSource audioSource;
            [SerializeField] private SwimController swimController;
            [SerializeField] private MovementController mc = null;
        	private float sprintTimer = 0.0f;
        	private float transition = 0.0f;
        	private bool wasRunning = false;
        	void Start()
        	{
                mc = (mc == null) ? this.transform.root.GetComponent<MovementController>() : mc;
        		if (audioSource == null) 
        		{
        			audioSource = this.GetComponent<AudioSource> ();
        		}
                swimController = (swimController == null) ? GetComponent<SwimController>() : swimController;
        	}
        	void Update()
        	{
                if (swimController && swimController.swimming == true) {
        			sprintTimer = 0;
        			transition = 0;
        			wasRunning = false;
        			return;
        		}
        		if (GetComponent<MovementController>().crouching == false 
                    && InputManager.GetButton ("Run") && InputManager.GetAxis ("Vertical") > 0.5f
                    && mc.aimWalk == false) //sprinting
        		{ 
        			sprintTimer += Time.deltaTime;
        		} 
        		else 
        		{
        			sprintTimer = (sprintTimer <= 0)? 0 : sprintTimer - (Time.deltaTime * 2);
        			if (sprintTimer > 0) 
        			{
        				transition += Time.deltaTime;
        				if (transition > transitionTime) 
        				{
        					transition = 0;
        					sprintTimer = 0;
        				}
        			}
        		}

        		if (sprintTimer > sprintBreathingDelay) 
        		{
        			
        			wasRunning = true;
        			if (audioSource.isPlaying == false) 
        			{
        				audioSource.clip = hardBreathing [UnityEngine.Random.Range (0, hardBreathing.Length)];
        				audioSource.Play ();
        			}
        		}
        		if (wasRunning && sprintTimer == 0) 
        		{
        			wasRunning = false;
        			StartCoroutine (PlayTransitionBreathing ());
        		}
        	}
        	IEnumerator PlayTransitionBreathing()
        	{
        		yield return new WaitForSeconds (audioSource.clip.length);
        		audioSource.clip = hardToCalmBreathing [UnityEngine.Random.Range (0, hardToCalmBreathing.Length)];
        		audioSource.Play ();
        	}
        	public void PlayEffortVoice()
        	{
        		audioSource.clip = effort [UnityEngine.Random.Range (0, effort.Length)];
        		audioSource.Play ();
        	}
        }
    }
}