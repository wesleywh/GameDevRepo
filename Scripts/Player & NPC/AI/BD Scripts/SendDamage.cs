using UnityEngine;
using System.Collections.Generic;
using Pandora.Controllers;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Trigger damage on target based on conditions.")]
    [TaskCategory("Basic")]
    public class SendDamage : Action {

        [Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The tag of the object that we are searching for")]
        public SharedString targetTag;
        [Tooltip("If true, the object must be within line of sight to be within distance. For example, if this option is enabled then an object behind a wall will not be within distance even though it may " +
            "be physically close to the other object")]
        public SharedBool lineOfSight;
        [Tooltip("The distance that the object needs to be within")]
        public SharedFloat magnitude = 5;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("The raycast offset relative to the pivot position")]
        public SharedVector3 offset;
        [Tooltip("The target raycast offset relative to the pivot position")]
        public SharedVector3 targetOffset;
        [Tooltip("The target raycast offset relative to the pivot position")]
        public SharedFloat damageToApply;

        private List<GameObject> objects;
        // distance * distance, optimization so we don't have to take the square root
        private float sqrMagnitude;

        public override void OnStart()
        {
            sqrMagnitude = magnitude.Value * magnitude.Value;

            if (objects != null) {
                objects.Clear();
            } else {
                objects = new List<GameObject>();
            }
            // if objects is null then find all of the objects using the layer mask or tag
            if (targetObject.Value == null) {
                if (!string.IsNullOrEmpty(targetTag.Value)) {
                    var gameObjects = GameObject.FindGameObjectsWithTag(targetTag.Value);
                    for (int i = 0; i < gameObjects.Length; ++i) {
                        objects.Add(gameObjects[i]);
                    }
                } else {
                    var colliders = Physics.OverlapSphere(transform.position, magnitude.Value, objectLayerMask.value);
                    for (int i = 0; i < colliders.Length; ++i) {
                        objects.Add(colliders[i].gameObject);
                    }
                }
            } else {
                objects.Add(targetObject.Value);
            }
        }
    	
    	// Update is called once per frame
        public override TaskStatus OnUpdate()
        {
            if (transform == null || objects == null)
                return TaskStatus.Failure;	
            
            Vector3 direction;
            // check each object. All it takes is one object to be able to return success
            for (int i = 0; i < objects.Count; ++i) {
                if (objects[i] == null) {
                    continue;
                }
                direction = objects[i].transform.position - (transform.position + offset.Value);
                // check to see if the square magnitude is less than what is specified
                if (Vector3.SqrMagnitude(direction) < sqrMagnitude) {
                    // the magnitude is less. If lineOfSight is true do one more check
                    if (lineOfSight.Value) {
                        if (MovementUtility.LineOfSight(transform, offset.Value, objects[i], targetOffset.Value, false, ignoreLayerMask.value)) {
                            // the object has a magnitude less than the specified magnitude and is within sight. Set the object and return success
                            objects[i].transform.root.GetComponent<Health>().ApplyDamage(damageToApply.Value);
                            return TaskStatus.Success;
                        }
                    } else {
                        // the object has a magnitude less than the specified magnitude. Set the object and return success
                        objects[i].transform.root.GetComponent<Health>().ApplyDamage(damageToApply.Value);
                        return TaskStatus.Success;
                    }
                }
            }
            // no objects are within distance. Return failure
            return TaskStatus.Failure;
    	}

        public override void OnReset()
        {
            targetObject = null;
            targetTag = string.Empty;
            objectLayerMask = 0;
            magnitude = 5;
            lineOfSight = true;
            ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
            offset = Vector3.zero;
            targetOffset = Vector3.zero;
        }
    }
}