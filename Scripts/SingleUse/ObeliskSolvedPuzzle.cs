using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(AudioSource))]
public class ObeliskSolvedPuzzle : MonoBehaviour {
	[SerializeField] private AnimationClip animClip;
	[SerializeField] private AudioClip soundClip;
	[SerializeField] private string itemToGive = "";

	public void SolvedPuzzle() 
	{
		if(animClip) 
		{
			this.GetComponent<Animation>().clip = animClip;
			this.GetComponent<Animation>().Play();
		}
		if(soundClip) 
		{
			this.GetComponent<AudioSource>().clip = soundClip;
			this.GetComponent<AudioSource>().Play();
		}
		if(itemToGive != "") 
		{
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>().AddToInventory(itemToGive);
		}
	}
}
