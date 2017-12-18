using System.Reflection;
using UnityEngine;
using System.Linq;                  //convert dictionary to array
using System.Collections.Generic;   //For dictionary definition

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject {
    [TaskCategory("Basic/GameObject")]
    [TaskDescription("Call a components function that is on a gameobject with supplied parameters.")]

    public class CallComponent : Action {

        [Tooltip("Target Gameobject")]
        public SharedGameObject owner;
        [Tooltip("The component to call.")]
        public SharedString componentName;
        [Tooltip("The function to call on the component.")]
        public SharedString functionName;
        [Tooltip("Parameters to supply to the function.")]
        public SharedVariable[] parameters;
        [Tooltip("Would you like to capture this functions return value?")]
        public bool returnValue = false;
        [Tooltip("Store the return value float.")]
        public SharedFloat storeFloat;
        [Tooltip("Store the return value int.")]
        public SharedInt storeInt;
        [Tooltip("Store the return value gameobject.")]
        public SharedGameObject storeGameOjbect;
        [Tooltip("Store the return value gameobject.")]
        public SharedVector3 storeVector3;
        [Tooltip("Store the return value particlesystem.")]
        public SharedParticleSystem storeParticleSystem;

        public override TaskStatus OnUpdate()
        {
            //Get target gameobject
            GameObject holder = GetDefaultGameObject (owner.Value);

            //Build array of different types
            Dictionary<int, object> values = new Dictionary<int, object> ();
            for (int i = 0; i < parameters.Length; i++) {
                values.Add(i, parameters[i].GetValue());
            }
            var paramArray = values.Values.ToArray ();

            //Call function with built array
            if (returnValue == true)
            {
                if (storeFloat.Value != null && storeFloat.Value != 0)
                {
                    storeFloat.Value = (float)holder.GetComponent(componentName.Value).GetType().GetMethod(functionName.Value).
                                        Invoke(holder.GetComponent(componentName.Value), paramArray);
                }
                else if (storeInt.Value != null && storeInt.Value != 0)
                {
                    storeInt.Value = (int)holder.GetComponent(componentName.Value).GetType().GetMethod(functionName.Value).
                                     Invoke(holder.GetComponent(componentName.Value), paramArray);
                }
                else if (storeGameOjbect.Value != null)
                {
                    storeGameOjbect.Value = (GameObject)holder.GetComponent(componentName.Value).GetType().GetMethod(functionName.Value).
                                            Invoke(holder.GetComponent(componentName.Value), paramArray);
                }
                else if (storeVector3.Value != null && storeVector3.Value != Vector3.zero)
                {
                    storeVector3.Value = (Vector3)holder.GetComponent(componentName.Value).GetType().GetMethod(functionName.Value).
                                            Invoke(holder.GetComponent(componentName.Value), paramArray);
                }
                else if (storeParticleSystem.Value != null)
                {
                    storeParticleSystem.Value = (ParticleSystem)holder.GetComponent(componentName.Value).GetType().GetMethod(functionName.Value).
                        Invoke(holder.GetComponent(componentName.Value), paramArray);
                }
                else
                {
                    holder.GetComponent(componentName.Value).GetType().GetMethod(functionName.Value).
                    Invoke(holder.GetComponent(componentName.Value), paramArray);
                }
            }
            else
            {
                holder.GetComponent(componentName.Value).GetType().GetMethod(functionName.Value).
                Invoke(holder.GetComponent(componentName.Value), paramArray);
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            owner = null;
            componentName = "";
            functionName = "";
            parameters = null;
        }
    }
}