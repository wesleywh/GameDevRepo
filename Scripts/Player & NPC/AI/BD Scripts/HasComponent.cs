using UnityEngine;
using Pandora.Controllers;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject
{
    [TaskCategory("Basic/GameObject")]
    [TaskDescription("Returns success if the specific gameobject has the target component name.")]
    public class HasComponent : Action
    {
        [Tooltip("Check the root of this gameobject only.")]
        public SharedBool rootOnly;
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The type of component")]
        public SharedString type;

        public override TaskStatus OnUpdate()
        {
            if (rootOnly.Value == true && targetGameObject.Value.transform.root.transform.GetComponent(type.Value))
            {
                return TaskStatus.Success;
            }
            else if(rootOnly.Value == false && GetDefaultGameObject(targetGameObject.Value).GetComponent(type.Value))
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            rootOnly = false;
            targetGameObject = null;
            type.Value = "";
        }
    }
}