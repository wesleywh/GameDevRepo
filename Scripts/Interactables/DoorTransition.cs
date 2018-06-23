using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using CyberBullet.GameManager;

namespace CyberBullet.Interactables
{
    [System.Serializable]
    public class DoorHideOrShow {
        public GameObject[] hide = null;
        public GameObject[] show = null;
    }
    [RequireComponent(typeof(Animation))]
    public class DoorTransition : MonoBehaviour {
        [Header("Note: This requires a trigger to work")]

        [SerializeField] private Animation anim;
        [SerializeField] private AudioClip sound = null;
        [SerializeField] private AudioSource source = null;
        [SerializeField] private AnimationClip door_animation;
        [SerializeField] private float move_speed = 1.0f;
        [SerializeField] private float wait_time = 0.2f;
        [SerializeField] private float end_distance = 0.1f;
        [SerializeField] private Transform point1 = null;
        [SerializeField] private Transform point2 = null;
        [SerializeField] private DoorHideOrShow point1To2;
        [SerializeField] private DoorHideOrShow point2To1;
        [SerializeField] private bool isLocked = false;
        [SerializeField] private int unlockItemId = 9999999;
        [SerializeField] private bool destroyItemOnUnlock = false;
        [SerializeField] private string lockedMessage = "This door is locked.";
        [SerializeField] private AudioClip lockedSound = null; 
        [SerializeField] private AudioClip openSound = null;
        [SerializeField] private AudioSource soundSource = null;

        private bool playerMoving = false;
        private bool isPlaying = false;
        private GameObject player = null;
        private GameObject playercam = null;
        private Transform start;
        private Transform end;

        private PlayerManager playerManager;
        private InventoryManagerNew inventory;
        private GUIManager gui;

        //slerp movement
        private Vector3 s_point = Vector3.zero;

        void Start()
        {
            playerManager = dontDestroy.currentGameManager.GetComponent<PlayerManager>();
            inventory = dontDestroy.currentGameManager.GetComponent<InventoryManagerNew>();
            gui = dontDestroy.currentGameManager.GetComponent<GUIManager>();
            if (!anim)
            {
                anim = GetComponent<Animation>();
            }
            if (!source)
            {
                source = GetComponent<AudioSource>();
            }
        }
        void OnTriggerEnter()
        {
            if (InputManager.GetButton("Action"))
            {
                if (CheckLockStatus() == false)
                {
                    WalkThroughDoor();
                }
            }
        }
        void OnTriggerStay()
        {
            if (InputManager.GetButton("Action"))
            {
                if (CheckLockStatus() == false)
                {
                    WalkThroughDoor();
                }
            }
        }

        void Update()
        {
            if (playerMoving)
            {
                float step = move_speed * Time.deltaTime;
                player.transform.position = Vector3.MoveTowards(player.transform.position, end.position, step);
//                playercam.transform.rotation = Quaternion.Euler(end.position.x, end.position.y + 0.5f, end.position.z);
                if (Vector3.Distance(player.transform.position, end.position) <= end_distance)
                {
                    playerMoving = false;
                }
            }
        }

        void PlayDoorSound(AudioClip Sound)
        {
            if (Sound == null)
                return;
            soundSource = (soundSource == null) ? dontDestroy.currentGameManager.GetComponent<AudioSource>() : soundSource;
            soundSource.clip = Sound;
            soundSource.Play();
        }
        bool CheckLockStatus()
        {
            bool retVal = false;
            if (isLocked == true && inventory.HasItem(unlockItemId) == false)
            {
                gui.SetPopUpText(lockedMessage);
                if (lockedSound != null)
                {
                    PlayDoorSound(lockedSound);
                }
                retVal = true;
            }
            else if (isLocked == true && inventory.HasItem(unlockItemId) == true)
            {
                isLocked = false;
                if (destroyItemOnUnlock == true)
                {
                    inventory.DestroyItem(unlockItemId);
                }
            }
            return retVal;
        }
        public void WalkThroughDoor()
        {
            if (isPlaying == false)
            {
                isPlaying = true;
                PlayDoorSound(openSound);
                StartCoroutine(PlayTransition());
            }
        }
        IEnumerator PlayTransition()
        {
            playerManager.SetPlayerControllable(false);
            player = GameObject.FindGameObjectWithTag("Player");
            start = GetPosition("start");
            end = GetPosition("end");
            foreach (GameObject item in GetObjects(start,"show"))
            {
                item.SetActive(true);
            }
            player.transform.position = start.position;
            player.transform.rotation = start.rotation;//Quaternion.Euler(start.position.x, player.transform.position.y, start.position.z);
            playercam = GameObject.FindGameObjectWithTag("CameraHolder");
            s_point = start.position;
//            playercam.transform.rotation = Quaternion.LookRotation(player.transform.position - (-end.position));
            playercam.transform.rotation = Quaternion.LookRotation(end.position - start.position);
//            playercam.transform.rotation = Quaternion.Euler(start.position.x, player.transform.position.y, start.position.z);
            anim.clip = door_animation;
            anim.Play();
            if (sound)
            {
                source.clip = sound;
                source.Play();
            }
            yield return new WaitForSeconds(wait_time);
            playerMoving = true;
            yield return new WaitForSeconds(anim.clip.length-wait_time);
            foreach (GameObject item in GetObjects(start,"hide"))
            {
                item.SetActive(false);
            }
            isPlaying = false;
            playerManager.SetPlayerControllable(true);
        }
    	
		public void UnlockDoor()
		{
			isLocked = false;
		}

        Transform GetPosition(string point)
        {
            Transform position;
            if (point == "start")
            {
                if (Vector3.Distance(player.transform.position, point1.position) <
                    Vector3.Distance(player.transform.position, point2.position))
                {
                    position = point1;
                }
                else
                {
                    position = point2;
                }
            }
            else
            {
                if (Vector3.Distance(player.transform.position, point1.position) <
                    Vector3.Distance(player.transform.position, point2.position))
                {
                    position = point2;
                }
                else
                {
                    position = point1;
                }
            }
            return position;
        }
        GameObject[] GetObjects(Transform start_point,string type)
        {
            GameObject[] returns;
            if (type == "show")
            {
                if (start_point == point1)
                {
                    returns = point1To2.show;
                }
                else
                {
                    returns = point2To1.show;
                }
            }
            else
            {
                if (start_point == point1)
                {
                    returns = point1To2.hide;
                }
                else
                {
                    returns = point2To1.hide;
                }
            }
            return returns;
        }
    }
}