using UnityEngine;
using System.Collections;
using Pandora.Cameras;
using Pandora.UI;

namespace Pandora.Controllers {
    public class CameraController : MonoBehaviour {
    	//[SerializeField] private GameObject mouseLookScript = null;		//legacy
    	[SerializeField] private Animator anim;
    	[SerializeField] private float moveSpeed = 1.0f;
    	[SerializeField] private Transform final;
    	[SerializeField] private Transform original;
    	private float length = 0.0f;
    	private bool lastState = false;
    	private float startTime = 0.0f;

    	void Start() {
    		if (final != null) {
    			length = Vector3.Distance (original.transform.position, final.transform.position);
    		}
    	}
    	void Update() {
    		if (lastState != anim.GetBool ("OnWall")) {
    			lastState = anim.GetBool ("OnWall");
    			startTime = Time.time;
    		}
    		if (anim.GetBool ("OnWall") == true) {
    			this.transform.GetComponent<MouseLook> ().enabled = false;
    			this.transform.LookAt (original.transform.position);
    			float disc = (Time.time - startTime) * moveSpeed;
    			float fracLength = disc / length;
    			this.transform.position = Vector3.Lerp (this.transform.position, final.transform.position, fracLength);
    		} else if (anim.GetBool ("OnWall") == false && GameObject.FindGameObjectWithTag("GameManager").transform.GetChild(1).GetComponent<OpenMenu>().IsMenuOpen() == false){
    			//mouseLookScript.GetComponent<MouseLook> ().enabled = true;
    			//float disc = (Time.time - startTime) * moveSpeed;
    			//float fracLength = disc / length;
    			this.transform.position = original.transform.position;//Vector3.Lerp (this.transform.position, original.transform.position, fracLength);
    		}
    	}
    }
}
