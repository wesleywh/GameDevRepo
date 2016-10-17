using UnityEngine;
using System.Collections;
using TeamUtility.IO;			//Custome InputManager Manager

public enum DoorEventType {
	Open, Close, Break, Slideup, Slidedown, Locked, Unlock, None
}
[System.Serializable]
public class DoorSounds {
	public DoorEventType type = DoorEventType.Open;	//don't have more than 1 of the same type
	public AudioClip[] sounds;						//sounds to play for the door event type
}
public class DoorEvent : MonoBehaviour {
	[SerializeField] public bool doorSoundsOnly;
	[SerializeField] public DoorSounds[] allDoorSounds;
	[Space(20)]
	[SerializeField] private Animation anim;
	[SerializeField] private AudioSource soundSource;
	[SerializeField] private bool breakable;
	[SerializeField] private DoorEventType goToState;
	[SerializeField] private float goToAnimSpeed = 1.0f;
	[SerializeField] private DoorEventType goBackState;
	[SerializeField] private float goBackAnimSpeed = 1.0f;
	[SerializeField] private float animBreakSpeed = 1.0f;
	[SerializeField] private GameObject effectOnBreak;
	[SerializeField] private float eventDistance = 1.5f;
	[SerializeField] private bool isLocked = false;
	[SerializeField] private bool debugMode = false;
	private DoorEventType current;
	private bool doorBroken = false;

	void Start() {
		if (doorSoundsOnly == false) {
			if (effectOnBreak != null) {
				effectOnBreak.SetActive (false);
			}
			current = goToState;
			if (soundSource == null) {
				soundSource = this.GetComponent<AudioSource> ();
			}
		}
	}
	string returnClipName(DoorEventType type) {
		string clip = null;
		switch (type) {
		case DoorEventType.Close:
			clip = "door_close";
			break;
		case DoorEventType.Open:
			if (anim.GetClip ("door_open")) {
				clip = "door_open";
			} else {
				clip = "gate_open";
			}
			break;
		case DoorEventType.Slidedown:
			clip = "door_slip_down";
			break;
		case DoorEventType.Slideup:
			clip = "door_slide_up";
			break;
		default:
			break;
		}
		return clip;
	}
	void Update() {
		if (doorSoundsOnly == false) {
			if (InputManager.GetButtonDown ("Action") && doorBroken == false) {
				if (debugMode) {
					Debug.Log ("Going To Play = " + current);
				}
				GameObject player = ClosestTarget ();
				if (Vector3.Distance (this.transform.position, player.transform.position) <= eventDistance) {
					if (InputManager.GetButton ("Run") && InputManager.GetButton ("Vertical") && breakable) {
						doorBroken = true;
						anim ["door_break"].speed = animBreakSpeed;
						anim.Play ("door_break");
						if (effectOnBreak != null) {
							effectOnBreak.SetActive (true);
						}
					} else if(anim.isPlaying == false){
						if (isLocked) {
							PlayLockedSound (DoorEventType.Locked);
							return;
						}	
						if (returnClipName (current) != null) {
							if (debugMode) {
								Debug.Log ("Door is playing " + current);
							}
							anim [returnClipName (current)].speed = (current == goToState) ? goToAnimSpeed : goBackAnimSpeed;
							if (anim.isPlaying == false) {
								anim.Play(returnClipName (current));
								StartCoroutine (UpdateCurrentState ());
							}
						}
					}
				}
			}
		}
	}
	IEnumerator UpdateCurrentState() {
		yield return new WaitForSeconds(0.1f);
		if (current == goToState) {
			current = goBackState; 
		} else if (current == goBackState) {
			current = goToState;
		}
		if (debugMode) {
			Debug.Log ("Updated To = " + current);
		}
	}
	GameObject ClosestTarget() {
		GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
		GameObject closest = null;
		foreach(GameObject target in targets) {
			if(closest == null) {
				closest = target;
			}
			else if(Vector3.Distance(this.transform.position, closest.transform.position) >
				Vector3.Distance(this.transform.position, target.transform.position) )
			{
				closest = target;
			}
		}
		return closest;
	}
	public void UnlockDoor() {
		isLocked = false;
		PlayLockedSound (DoorEventType.Unlock);
	}
	void PlayLockedSound(DoorEventType type) {
		AudioClip[] selectedSounds = null;
		for (int i = 0; i < allDoorSounds.Length; i++) {
			if (allDoorSounds [i].type == type) {
				selectedSounds = allDoorSounds [i].sounds;
				break;
			}
		}
		if (selectedSounds != null) {
			soundSource.clip = selectedSounds [Random.Range (0, selectedSounds.Length)];
			soundSource.Play ();
		}
	}
//----------------------- FOR EVENT TRIGGERS
	public void PlayOpenSound() {
		PlayLockedSound (DoorEventType.Open);
	}
	public void PlayCloseSound() {
		PlayLockedSound (DoorEventType.Close);
	}
	public void PlaySlidUpSound() {
		PlayLockedSound (DoorEventType.Slideup);
	}
	public void PlaySlideDownSound() {
		PlayLockedSound (DoorEventType.Slidedown);
	}
	public void PlayBreakSound() {
		PlayLockedSound (DoorEventType.Break);
	}
	void OnDrawGizmos() {
		if (doorSoundsOnly == false) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (this.transform.position, eventDistance);
		}
	}
}
