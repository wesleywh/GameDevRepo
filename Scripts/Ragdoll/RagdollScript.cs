using UnityEngine;
using System.Collections;

//A very simple script to show how to turn a Mecanim character into a ragdoll
//when the user presses space, assuming that the Unity Ragdoll Wizard has
//been used to add the ragdoll RigidBody components


public class RagdollScript : MonoBehaviour {
	//Helper to set the isKinematc property of all RigidBodies in the children of the 
	//game object that this script is attached to
	void SetKinematic(bool newValue)
	{
		//Get an array of components that are of type Rigidbody
		Rigidbody[] bodies=GetComponentsInChildren<Rigidbody>();

		//For each of the components in the array, treat the component as a Rigidbody and set its isKinematic property
		foreach (Rigidbody rb in bodies)
		{
			rb.isKinematic=newValue;
		}
	}
	// Use this for initialization
	void Start () {
		//Set all RigidBodies to kinematic so that they can be controlled with Mecanim
		//and there will be no glitches when transitioning to a ragdoll
		SetKinematic(true);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SetKinematic(false);
			GetComponent<Animator>().enabled=false;
		}
	}
}
