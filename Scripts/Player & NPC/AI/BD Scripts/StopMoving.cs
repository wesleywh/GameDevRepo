using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Stop moving using the NavMesh")]
    [TaskCategory("Movement")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class StopMoving : NavMeshMovement {

        public override void OnStart()
        {
            base.OnStart();

            SetDestination(this.transform.position);
        }
    	
        public override TaskStatus OnUpdate()
        {
            if (HasArrived()) {
                return TaskStatus.Success;
            }

            SetDestination(this.transform.position);

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            base.OnReset();
        }
    }
}