using UnityEngine;
using System.Collections;
using System;

[Serializable]
class FootStepMaterial {
	public string materialName = "";
	public int number = 0;
	public AudioClip[] soundsToPlay = null;
}

[RequireComponent(typeof(AudioSource))]
public class FootstepHandler : MonoBehaviour {
	[Space(10, order=0)]
	[Header("Give movement sounds to an object",order=1)]
	[Space(-10, order=2)]
	[Header("based on the material that object is on.",order=3)]
	[Space(-10,order=4)]
	[Header("(number only used if terrain is used)",order=5)]
	[Space(10,order=6)]
	[SerializeField] private FootStepMaterial[] materialList;
	[SerializeField] private float minInterval = 0.1f;
	[SerializeField] private float maxVelocity = 8.0f;
	[SerializeField] private float groundCheckHeight = 1.0f; 
	[HideInInspector]
	[SerializeField] public bool useTerrain = false;
	[SerializeField] private float bias = 1.1f;
	[SerializeField] public bool useFootStepVolumeSet = false;
	[Range(0.0F, 1.0F)]
	[SerializeField] private float setVolume = 0.0f;
	[SerializeField] public bool manuallySetIntervals = false;
	[SerializeField] private float walkInterval = 0.5f;
	[SerializeField] private float runInterval = 0.1f;

	private float speed = 0.0f;
	private Vector3 lastPos = Vector3.zero;
	private bool grounded;
	private string hitSurfaceName;
	private AudioClip[] sounds;
	private float interval = 0;
	private float timer = 0;
	private String standingMaterial = "";

	void Start() 
	{
		InvokeRepeating("IsGrounded",0.05f,0.05f);
		StartCoroutine (PlayFootStepSounds ());
	}
	void IsGrounded()
	{
		RaycastHit hit;
		grounded = Physics.Raycast(this.transform.position, - Vector3.up, out hit, groundCheckHeight + 0.1f);
		if (grounded == true && useTerrain == false) 
		{
			standingMaterial = hit.transform.gameObject.GetComponent<MeshRenderer>().material.name;
		}
	}
	void Update()
	{
		timer += Time.deltaTime;

		if (useTerrain == true) 
		{
			hitSurfaceName = TerrainSurface.GetMainTexture (transform.position);
		} 
		speed = Vector3.Distance(lastPos, this.transform.position) / Time.deltaTime;
		lastPos = this.transform.position;

		if (grounded && speed > 0.2f && timer > interval) {
			if(useTerrain == true) {
				foreach(FootStepMaterial material in materialList) 
				{
					if(material.materialName == hitSurfaceName) 
					{
						sounds = material.soundsToPlay;
					}
				}
			}
			else
			{
				foreach(FootStepMaterial material in materialList)
				{
					if(material.materialName == standingMaterial || material.materialName+" (Instance)" == standingMaterial)
					{
						sounds = material.soundsToPlay;
					}
				}
			}
		}
	}

	IEnumerator PlayFootStepSounds () {
		while (true) {
			float vel = this.GetComponent<Rigidbody>().velocity.magnitude;
			if (grounded && speed > 0.2f && sounds != null && this.GetComponent<AudioSource>().isPlaying == false) {
				AudioClip footstep = sounds [UnityEngine.Random.Range (0, sounds.Length)];

				if(useFootStepVolumeSet == true) {
					this.GetComponent<AudioSource>().volume = setVolume;
				}
				this.GetComponent<AudioSource>().clip = footstep;
				if(this.GetComponent<AudioSource>().isPlaying == false){
					this.GetComponent<AudioSource>().Play ();
				}
				float interval = 0.0f;
				if (manuallySetIntervals == false) {
					interval = minInterval * (maxVelocity + bias) / (vel + bias);
				} 
				else {
					switch (this.GetComponent<AIBehavior> ().memory.currentState) {
						case "Idle":
						case "Wander":
						case "Walking":
						case "Patrol":
						case "Suspicious":
						case "Investigate":
						case "Searching":
							interval = walkInterval;
							break;
						case "Attacking":
						case "Hostile":
						case "FindCover":
						case "RunningToCover":
							interval = runInterval;
							break;
						default:
							interval = walkInterval;
							break;
					}
				}
				yield return new WaitForSeconds (interval);
			} 
			else {
				yield return 0;
			}
		}
	}
}