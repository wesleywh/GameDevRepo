using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

namespace Pandora.Interactables
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
        private bool playerMoving = false;
        private bool isPlaying = false;
        private GameObject player = null;
        private GameObject playercam = null;
        private Transform start;
        private Transform end;

        void OnStart()
        {
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
                if (isPlaying == false)
                {
                    isPlaying = true;
                    StartCoroutine(PlayTransition());
                }
            }
        }
        void OnTriggerStay()
        {
            if (InputManager.GetButton("Action"))
            {
                if (isPlaying == false)
                {
                    isPlaying = true;
                    StartCoroutine(PlayTransition());
                }
            }
        }

        void Update()
        {
            if (playerMoving)
            {
                float step = move_speed * Time.deltaTime;
                player.transform.position = Vector3.MoveTowards(player.transform.position, end.position, step);
                if (Vector3.Distance(player.transform.position, end.position) <= end_distance)
                {
                    playerMoving = false;
                }
            }
        }

        IEnumerator PlayTransition()
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Pandora.GameManager.PlayerManager>().SetPlayerControllable(false);
            player = GameObject.FindGameObjectWithTag("Player");
            start = GetPosition("start");
            end = GetPosition("end");
            foreach (GameObject item in GetObjects(start,"show"))
            {
                item.active = true;
            }
            player.transform.position = start.position;
            player.transform.rotation = start.rotation;//Quaternion.Euler(start.position.x, player.transform.position.y, start.position.z);
            playercam = GameObject.FindGameObjectWithTag("PlayerCamera");
            playercam.transform.rotation = Quaternion.Euler(start.position.x, player.transform.position.y, start.position.z);
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
                item.active = false;
            }
            isPlaying = false;
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Pandora.GameManager.PlayerManager>().SetPlayerControllable(true);
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