using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pandora.Controllers;

namespace Pandora {
    namespace Interactables {
        public class DamageModifer : MonoBehaviour {

            public enum ModifyType {Multiply, Subtract, Add}
            public ModifyType modifyType = ModifyType.Multiply;
            public float amount = 2.0f;

            public void SendModifiedDamage(float damage, GameObject sender = null)
            {
                switch(modifyType)
                {
                    case ModifyType.Multiply:
                        this.transform.root.GetComponent<Health>().ApplyDamage(damage * amount, sender);
                        break;
                    case ModifyType.Add:
                        this.transform.root.GetComponent<Health>().ApplyDamage(damage + amount, sender);
                        break;
                    case ModifyType.Subtract:
                        this.transform.root.GetComponent<Health>().ApplyDamage(damage - amount, sender);
                        break;
                }
            }
        }
    }
}