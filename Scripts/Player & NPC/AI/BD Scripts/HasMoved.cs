using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Stop moving using the NavMesh")]
    [TaskCategory("Movement")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]

    public class HasMoved : Action {
        
        [Tooltip("The GameObject your checking if it has moved.")]
        public SharedGameObject target;

        private bool hasMoved = false;
        private Vector3 lastPosition = Vector3.zero;
    	
    	// Update is called once per frame
        public override TaskStatus OnUpdate() {
            if (target.Value.transform.position != lastPosition)
            {
                hasMoved = true;
                lastPosition = target.Value.transform.position;
            }
            else
            {
                hasMoved = false;
            }
            if (hasMoved == false)
            {
                return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Success;
            }
    	}
    }
}
