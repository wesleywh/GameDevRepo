using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.SharedVariables {

	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Extract the vector3 from a gameobject.")]

	public class ExtractGameObjectVector3 : Action
	{
		[Tooltip("The value to set the SharedString to")]
		public SharedGameObject targetValue;
		[RequiredField]
		[Tooltip("The SharedString to set")]
		public SharedVector3 targetVariable;

		public override TaskStatus OnUpdate()
		{
            targetVariable.Value = targetValue.Value.transform.position;

			return TaskStatus.Success;
		}

		public override void OnReset()
		{
            targetValue = null;
            targetVariable = Vector3.zero;
		}
	}
}
