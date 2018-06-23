using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberBullet.Controllers;

namespace CyberBullet.Interactions {
    public class ParticleDamage : MonoBehaviour {

        public float particle_damage = 10.0f;
        void OnParticleCollision(GameObject go)
        {
            if (go.transform.root.GetComponent<Health>())
            {
                go.transform.root.GetComponent<Health>().ApplyDamage(particle_damage, gameObject);
            }
        }
    }
}