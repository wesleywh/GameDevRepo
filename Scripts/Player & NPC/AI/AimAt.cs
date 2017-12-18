using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pandora.AI {
    public class AimAt : MonoBehaviour {
        public float resetSeconds = 1;
        public Transform target;
        public Vector3 offset;
        public HumanBodyBones bone_selection;
        Animator anim;
        Transform chest;

        private float timer = 0;
    	void Start () {
            anim = GetComponent<Animator>();
            chest = anim.GetBoneTransform(bone_selection);
    	}
    	
    	// Update is called once per frame
    	void LateUpdate () {
            if (target != null)
            {
                timer = 0;
                chest.LookAt(target.position);
                chest.rotation = chest.rotation * Quaternion.Euler(offset);
            }
            else if (timer <= resetSeconds)
            {
                timer += Time.deltaTime;
                chest.LookAt(target.position);
                chest.rotation = chest.rotation * Quaternion.Euler(offset);
            }
    	}

        public void UpdateOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
    }
}