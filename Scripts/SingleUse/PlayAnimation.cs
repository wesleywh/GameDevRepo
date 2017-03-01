using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour {
	[SerializeField] private AnimationClip animClip = null;
	[SerializeField] private float speed = 0.0f;
	public void PlayTargetAnimation() {
		this.GetComponent<Animation>().clip = animClip;
		if(speed > 0)
			GetComponent<Animation> () [animClip.name].speed = speed;
		GetComponent<Animation>().Play();
	}
}
