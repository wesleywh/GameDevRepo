using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pandora.AI;
using Panda.AI;
using Pandora.Controllers;
using System.Linq;

public class Keyframes : MonoBehaviour {
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private float melee_angle = 130f;
    [SerializeField] private float melee_range = 2.0f;
    [SerializeField] private Transform melee_position = null;
    [SerializeField] private string[] enemy_tags;
    [SerializeField] private float melee_damage = 5.0f;
    [SerializeField] private AudioSource voice_source;
    [SerializeField] private AudioClip[] voice_sounds;
    [SerializeField] private AudioSource punch_source;
    [SerializeField] private AudioClip[] punch_sounds;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Debugging")]
    [SerializeField] private bool showPunchRay;
    void Start()
    {
        melee_position = (melee_position == null) ? transform : melee_position;
        melee_angle = (GetComponent<AIMemory>()) ? GetComponent<AIMemory>().fieldOfView : melee_angle;
        melee_range = (GetComponent<AIMemory>()) ? GetComponent<AIMemory>().melee_range : melee_range;
    }

    public void SendDamageInRange()
    {
        GameObject enemy = null;
        if (isPlayer == false)
        {
//            AIMemory memory = GetComponent<AIMemory>();
            enemy = AIHelpers.FindClosestEnemy(enemyLayers, melee_position, true, melee_range, melee_angle);
            if (enemy != null && enemy.transform.root.GetComponent<Health>())
            {
                enemy.transform.root.GetComponent<Health>().ApplyDamage(melee_damage, gameObject);
            }
        }
        else if (isPlayer == true)
        {
            Camera cam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
            Ray ray = cam.ScreenPointToRay (new Vector3(Screen.height/2, Screen.width/2,0));
            RaycastHit hit; 
            if (Physics.Raycast(ray, out hit, melee_range, enemyLayers))
            {
                if (hit.transform.root.GetComponent<Health>())
                {
                    punch_source.clip = punch_sounds[Random.Range(0, punch_sounds.Length - 1)];
                    punch_source.Play();
                    hit.transform.root.GetComponent<Health>().ApplyDamage(melee_damage, gameObject);
                }
            }
        }
    }

    public void PlayVoiceSound()
    {
        voice_source.clip = voice_sounds[Random.Range(0, voice_sounds.Length-1)];
        voice_source.Play();
    }
}
