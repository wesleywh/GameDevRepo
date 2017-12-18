using UnityEngine;
using System.Reflection;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject {

	[TaskCategory("Basic/GameObject")]
	[TaskDescription("Enable/Disable a component that is attached to the target gameobject.")]

	public class EnableComponent : Action
	{
		[Tooltip("Target Gameobject")]
		public SharedGameObject owner;
		[Tooltip("The component name to enable.")]
        public SharedString componentName;
        public SharedBool state;

		public override TaskStatus OnUpdate()
		{
            MonoBehaviour script = (MonoBehaviour)GetDefaultGameObject (owner.Value).GetComponent (componentName.Value);
            script.enabled = state.Value;

            return TaskStatus.Success;
		}

		public override void OnReset()
		{
			owner = null;
			componentName = "";
		}
	}
}
