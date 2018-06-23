using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CyberBullet.Controllers;

namespace CyberBullet {
    namespace UI {
        public class UIHealthBarManager : MonoBehaviour {
        	[Tooltip("Will find 'player' tag if not specified")]
        	[SerializeField] private GameObject player;
        	[SerializeField] private GameObject redBar;
        	[SerializeField] private GameObject yellowBar;
        	[SerializeField] private GameObject percentText;
        	private float currentHealth;
        	private float pastHealth;
        	private float percent;
        	private float timer = 0.0f;
        	// Use this for initialization
        	void Start () {
        		if (player == null) {
        			player = GameObject.FindGameObjectWithTag ("Player");
        		}
        		if (player.GetComponent<Health> ()) {
                    pastHealth = player.GetComponent<Health> ().GetHealth ();
        		}
        	}
        	
        	// Update is called once per frame
        	void Update () {
        		if (GameObject.FindGameObjectWithTag ("Player")) {
        			player = GameObject.FindGameObjectWithTag ("Player");
        		} else {
        			return;
        		}
                currentHealth = (player.GetComponent<Health> ())?player.GetComponent<Health> ().GetHealth () : 0;
        		
        		if (currentHealth != pastHealth) {
        			if (timer > 2) {
        				if (currentHealth < pastHealth) {
        					pastHealth -= Time.deltaTime * 16;
        				} else {
        					pastHealth = currentHealth;
        				}
        			} else {
        				timer += Time.deltaTime;
        			}
        		} else {
        			timer = 0;
        		}
        		if (pastHealth < 0) {
        			pastHealth = 0;
        		}
        		percent = currentHealth / 100;
        		redBar.GetComponent<Image> ().fillAmount = percent;
        		yellowBar.GetComponent<Image> ().fillAmount = pastHealth / 100; 
        		percent *= 100;
        		percentText.GetComponent<Text> ().text = percent.ToString()+"%";
        	}
        }
    }
}