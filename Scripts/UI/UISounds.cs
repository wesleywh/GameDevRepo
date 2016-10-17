using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class UISounds : MonoBehaviour {

	[SerializeField] private AudioSource soundController;
	[SerializeField] private AudioClip mouseEnter;
	[SerializeField] private AudioClip mouseExit;
	[SerializeField] private AudioClip mouseClick1;
	[SerializeField] private AudioClip mouseClick2;
	[SerializeField] private AudioClip mouseClick3;
	[SerializeField] private AudioClip lockClick1;
	[SerializeField] private AudioClip lockClick2;
	[SerializeField] private AudioClip lockClick3;
	[SerializeField] private AudioClip puzzleEnter;
	[SerializeField] private AudioClip puzzleExit;

	// Use this for initialization
	void Start () {
		soundController = this.GetComponent<AudioSource> ();
	}
	public void PuzzleEnterSound() {
		soundController.clip = puzzleEnter;
		soundController.Play ();
	}
	public void PuzzleExitSound() {
		soundController.clip = puzzleExit;
		soundController.Play ();
	}
	public void MouseLockClick1() {
		soundController.clip = lockClick1;
		soundController.Play ();
	}
	public void MouseLockClick2() {
		soundController.clip = lockClick2;
		soundController.Play ();
	}
	public void MouseLockClick3() {
		soundController.clip = lockClick3;
		soundController.Play ();
	}
	public void MouseEnterSound() {
		soundController.clip = mouseEnter;
		soundController.Play ();
	}
	public void MouseExitSound() {
		soundController.clip = mouseExit;
		soundController.Play ();
	}
	public void MouseClick1Sound(){
		soundController.clip = mouseClick1;
		soundController.Play ();
	}
	public void MouseClick2Sound(){
		soundController.clip = mouseClick2;
		soundController.Play ();
	}
	public void MouseClick3Sound(){
		soundController.clip = mouseClick3;
		soundController.Play ();
	}
}
