using UnityEngine;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Sends an event to the behavior tree, returns success after sending the event.")]
	[TaskIcon("{SkinColor}SendEventIcon.png")]
    public class BroadcastEvent : Action 
    {

		[Tooltip("The GameObject of the behavior tree that should have the event sent to it. If null use the current behavior")]
        public LayerMask targets;
		[Tooltip("The event to send")]
		public SharedString eventName;
        [Tooltip("How far to broadcast this event.")]
        public SharedFloat magnitude = 5;

        private List<GameObject> objects;
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
            var colliders = Physics.OverlapSphere(transform.position, magnitude.Value, targets.value);
            for (int i = 0; i < colliders.Length; ++i) {
                objects.Add(colliders[i].gameObject);
            }
		}
		
		public override TaskStatus OnUpdate()
		{
            foreach (GameObject target in objects)
            {
                if (target.GetComponent<BehaviorTree>())
                {
                    target.GetComponent<BehaviorTree>().SendEvent(eventName.Value);
                }
                else
                {
                    target.transform.root.GetComponent<BehaviorTree>().SendEvent(eventName.Value);
                }
            }
			
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			// Reset the properties back to their original values
            targets = 1 << LayerMask.NameToLayer("Ignore Raycast");
			eventName = "";
            magnitude = 0;
		}
	}
}