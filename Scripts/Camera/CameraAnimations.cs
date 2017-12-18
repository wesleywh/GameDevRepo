using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using Pandora.Controllers;

namespace Pandora.Cameras {
    public class CameraAnimations : MonoBehaviour {
        public Animation anim;
        public MovementController mc;
        public string calm = "camera_sway";
        public string sprint = "camera_run";
    	
    	void Start () {
            anim = (anim == null) ? GetComponent<Animation>() : anim;
            mc = (mc == null) ? transform.root.GetComponent<MovementController>() : mc;
    	}
    	
    	void FixedUpdate () {
            float inputX = (mc.moveLocked == false) ? InputManager.GetAxis("Horizontal") : 0;
            float inputY = (mc.moveLocked == false) ? InputManager.GetAxis("Vertical") : 0;
            if (InputManager.GetButton("Run") && ((inputX > 0.1f || inputX < -0.1f) || (inputY > 0.1f || inputY < -0.1f)))
            {
                StartCoroutine(PlayRunAnimation());
            }
    	}

        IEnumerator PlayRunAnimation()
        {
            if (anim != null && anim.clip.name == calm)
            {
                anim.Play(sprint);
                yield return new WaitForSeconds(sprint.Length);
                anim.Play(calm);
            }
            else
            {
                yield return null;
            }
        }
    }
}