using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.Events;

public class Fish_Boss : MonoBehaviour {

    private bool attacking = false;
    private Transform attack_point;
    private float timer = 0;
    public float jump_speed = 1f;
    public float jump_height = 2.0f;
    public GameObject look_at;
    public int yell_prob = 3;
    private Vector3 start_pos = Vector3.zero;

    public AudioSource sSource = null;
    public AudioClip[] water_splash;
    public AudioClip[] water_land;
    public AudioClip[] yells;
    public GameObject spit_attack_item;
    public Transform spit_attack_point;
    public float spit_speed = 3.0f;

    private bool spitAttack = false;
    void Update()
    {
        if (attacking == true)
        {
            if (Vector3.Distance(this.transform.position, attack_point.position) <= 0.1f)
            {
                attacking = false;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
                Vector3 cur_pos = Vector3.Lerp(start_pos, attack_point.position, timer / jump_speed);
                cur_pos.y += jump_height * Mathf.Sin(Mathf.Clamp01(timer / jump_speed) * Mathf.PI);
                transform.position = cur_pos;
                if (Vector3.Distance(transform.position, attack_point.position) < 0.05f)
                {
                    attacking = false;
                    PlayWaterLand();
                }
                Quaternion targetRotation = Quaternion.LookRotation(look_at.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, timer / jump_speed);
            }
        }
        if (spitAttack == true)
        {
            if (Vector3.Distance(this.transform.position, attack_point.position) <= 0.1f)
            {
                spitAttack = false;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
                Vector3 cur_pos = Vector3.Lerp(start_pos, attack_point.position, timer / jump_speed);
                transform.position = cur_pos;
                if (Vector3.Distance(transform.position, attack_point.position) < 0.05f)
                {
                    spitAttack = false;
                    StartCoroutine(WaitSpitAttack());
                }
                Quaternion targetRotation = Quaternion.LookRotation(look_at.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, timer / jump_speed);
            }
        }
    }
    public void MakeJumpAttack(GameObject start, List<GameObject> attack_points)
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (GameObject item in attack_points)
        {
            tempList.Add(item);
        }
        tempList.Remove(start);
        attack_point = tempList[Random.Range(0, tempList.Count-1)].transform;
        start_pos = transform.position;
        attacking = true;
        timer = 0;
        PlayWaterSplash();
        int prob = Random.Range(0, yell_prob+1);
        if (prob == yell_prob)
        {
            PlayYell();
        }
    }
    public void SpitAttack(GameObject start, List<GameObject> attack_points)
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (GameObject item in attack_points)
        {
            tempList.Add(item);
        }
        tempList.Remove(start);
        attack_point = tempList[Random.Range(0, tempList.Count-1)].transform;
        start_pos = transform.position;
        spitAttack = true;
        timer = 0;
        PlayWaterSplash();
        int prob = Random.Range(0, yell_prob+1);
        if (prob == yell_prob)
        {
            PlayYell();
        }
    }

    GameObject GetClosest(GameObject[] points)
    {
        GameObject final = null;
        float closet = UnityEngine.Mathf.Infinity;
        for (int i=0; i < points.Length; i++)
        {
            if (final == null)
            {
                final = points[i];
            }
            else
            {
                UnityEngine.Vector3 direction = points[i].transform.position - final.transform.position;
                float sqrToTarget = direction.sqrMagnitude;
                if (sqrToTarget < closet)
                {
                    closet = sqrToTarget;
                    final = points[i];
                }
            }
        }
        return final;
    }
       
    IEnumerator WaitSpitAttack()
    {
        yield return new WaitForSeconds(Random.Range(0.5f,1.0f));
        GameObject spit = (GameObject)Instantiate(spit_attack_item, spit_attack_point.position, spit_attack_point.rotation);
        GameObject player = GameObject.FindGameObjectWithTag("PlayerCamera");
        if (spit.GetComponent<Rigidbody>())
            spit.GetComponent<Rigidbody>().AddForce((player.transform.position - spit_attack_point.position) * spit_speed);
        Destroy(spit, 3.0f);
    }
    void PlayWaterSplash()
    {
        sSource.clip = water_splash[Random.Range(0, water_splash.Length)];
        sSource.Play();
    }
    void PlayWaterLand()
    {
        sSource.clip = water_land[Random.Range(0, water_land.Length)];
        sSource.Play();
    }
    public void PlayYell()
    {
        sSource.clip = yells[Random.Range(0, yells.Length)];
        sSource.Play();
    }
}
