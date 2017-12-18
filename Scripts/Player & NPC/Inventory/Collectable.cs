using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using Pandora.GameManager;

namespace Pandora.Items {
    public class Collectable : MonoBehaviour {

        public int id = 9999999;
        public float float_var = 0;
        public float pickupDistance = 5.0f;
        public bool showPickupDistance = true;
        [SerializeField] private bool moveToCamera = true;
        public float smoothTime = 0.3f;
        public float destroyTime = 1.0f;
        private GameObject player = null;
        private InventoryManagerNew invMg = null;
        private bool canPickup = true;
        private bool move = false;
        private GameObject dropPointObject = null;
        private Vector3 velocity = Vector3.zero;
        private bool rigidDisabled = false;
        private bool initiatedDestroy = false;
        void Start()
        {
            dropPointObject = GameObject.FindGameObjectWithTag("ItemDropPoint");
            player = GameObject.FindGameObjectWithTag("Player");
            invMg = (invMg == null) ? GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManagerNew>() : invMg;
        }
        void Update()
        {
            if (player && Vector3.Distance(player.transform.position, transform.position) < pickupDistance && 
                InputManager.GetButton("Action") && 
                canPickup == true)
            {
                invMg.float_var = float_var;
                if (invMg.AddItem(id))
                {
                    canPickup = false;
                    if (moveToCamera == false)
                    {
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        move = true;
                    }
                }
            }
            if (move == true)
            {
                if (rigidDisabled == false)
                    DisableRigidBodyAndColliders();
                MoveToCamera();
                if (initiatedDestroy == false)
                {
                    initiatedDestroy = true;
                    StartCoroutine(DestroyThis());
                }
            }
        }
        IEnumerator DestroyThis()
        {
            yield return new WaitForSeconds(destroyTime);
            Destroy(this.gameObject);
        }
        void MoveToCamera()
        {
            this.transform.position = Vector3.SmoothDamp(transform.position, dropPointObject.transform.position, ref velocity, smoothTime);
        }
        void DisableRigidBodyAndColliders()
        {
            rigidDisabled = true;
            if (this.GetComponent<Rigidbody>())
                this.GetComponent<Rigidbody>().isKinematic = true;
            Collider[] cols = this.GetComponents<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].enabled = false;
            }
        }
        void OnDrawGizmosSelected()
        {
            if (showPickupDistance)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, pickupDistance);
            }
        }
        
    }
}