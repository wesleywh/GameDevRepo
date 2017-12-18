using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityPhysics {

    [TaskCategory("Basic/Physics")]
    [TaskDescription("Casts a ray from origin to target with specified offset and inaccuracy.Value.")]
    public class InaccurateRaycast : Action 
    {
        [Tooltip("How inaccurate this raycast is. (Position + or - inaccuracy.Value.)")]
        public SharedFloat inaccuracy;
        [Tooltip("Up offset of desired hit point")]
        public SharedFloat offset;
        [Tooltip("The target your trying to hit with the raycast")]
        public SharedGameObject target;
        [Tooltip("Always get the position based off the root of the origin gameobject.")]
        public SharedBool targetRoot;
        [Tooltip("Starts the ray at the position. Only used if originGameObject is null")]
        public SharedGameObject origin;
        [Tooltip("Always get the position based off the root of the origin gameobject.")]
        public SharedBool originRoot;
        [Tooltip("Selectively ignore colliders")]
        public LayerMask layerMask = -1;
        [Tooltip("How inaccurate this raycast is. (Position + or - inaccuracy.Value.)")]
        public SharedFloat range;

        [Tooltip("Stores the hit object of the raycast")]
        public SharedGameObject storeHitGameObject;
        [Tooltip("Stores the hit point of the raycast")]
        public SharedVector3 storeHitPoint;
        [SharedRequired]
        [Tooltip("Stores the hit normal of the raycast")]
        public SharedVector3 storeHitNormal;

        [Tooltip("Draw raycast lines for visual debugging.")]
        public SharedBool debug;

        public override TaskStatus OnUpdate()
        {
            try {
                Vector3 tr;
                if(targetRoot.Value == true)
                {
                    tr = target.Value.transform.root.transform.position;
                }
                else {
                    tr = target.Value.transform.position;
                }

                Vector3 or;
                if (originRoot.Value == true)
                {
                    or = origin.Value.transform.root.transform.position;
                }
                else {
                    or = origin.Value.transform.position;
                }
                tr = (offset.Value == 0) ? tr : tr + (Vector3.up * offset.Value);
                Vector3 direction = (tr - or);
                RaycastHit hit;
                direction = new Vector3(direction.x+Random.Range(-inaccuracy.Value,inaccuracy.Value),
                                        direction.y+Random.Range(-inaccuracy.Value,inaccuracy.Value), 
                                        direction.z+Random.Range(-inaccuracy.Value,inaccuracy.Value));
                if (Physics.Raycast(or, direction, out hit, range.Value, layerMask)) //draw raycast straight to object from eyes
                {
                    if (debug.Value == true)
                    {
                        Debug.DrawRay(or, direction, Color.red, 0.5f);
                    }
                    storeHitGameObject.Value = hit.transform.gameObject;
                    storeHitPoint.Value = hit.point;
                    storeHitNormal.Value = hit.normal;
                    return TaskStatus.Success;
                }
                else if (debug.Value == true)
                {
                    Debug.DrawRay(or, direction, Color.blue, 1.0f);
                }
                return TaskStatus.Failure;
            }
            catch {
                return TaskStatus.Failure;
            }
        }

        public override void OnReset()
        {
            origin = null;
            layerMask = -1;
            target = null;
            offset = 0;
            inaccuracy.Value = 0;
        }
    }
}