using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks{
	[TaskCategory("Basic/Animator")]
	[TaskDescription("Returns success if the specified parameter is matching the state.")]
	[TaskIcon("{SkinColor}ConditionalEvaluatorIcon.png")]
    public class BoolParamValueSet : Decorator {
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;
		[Tooltip("The name of the parameter")]
		public SharedString paramaterName;
        [Tooltip("The parameter state to check for.")]
        public SharedBool state;

		private Animator animator;
		private GameObject prevGameObject;

		// The status of the child after it has finished running.
		private TaskStatus executionStatus = TaskStatus.Inactive;
		private bool checkConditionalTask = true;
		private bool conditionalTaskFailed = false;
    
		public override void OnStart()
		{
			var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (currentGameObject != prevGameObject) {
				animator = currentGameObject.GetComponent<Animator>();
				prevGameObject = currentGameObject;
			}
		}
		
		public override TaskStatus OnUpdate()
		{
            if (animator == null)
            {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }
            else if (animator.GetBool(paramaterName.Value) == state.Value)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
		}

		public override void OnReset()
		{
			targetGameObject = null;
			paramaterName = "";
		}
	}
}