using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.SharedVariables {

	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Check if two gameobjects are the same.")]

    public class CompareGameObjects : Decorator
	{
        [Tooltip("If you want to compare the root gameobjects only")]
        public SharedBool rootOnly;
		[Tooltip("The first gameobject")]
		public SharedGameObject target1;
		[RequiredField]
		[Tooltip("The second gameobject")]
        public SharedGameObject target2;

		public override TaskStatus OnUpdate()
		{
            Debug.Log(target1.Value.transform.root.gameObject.tag+" == "+target2.Value.transform.root.gameObject.tag);
            if (rootOnly.Value == false && GameObject.ReferenceEquals(target1.Value, target2.Value))
            {
                return TaskStatus.Success;
            }
            else if (rootOnly.Value == true && GameObject.ReferenceEquals(target1.Value.transform.root.gameObject, target2.Value.transform.root.gameObject))
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
		}

		public override void OnReset()
		{
            target1 = null;
            target2 = null;
		}
	}
}
