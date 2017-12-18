namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Modify the SharedInt variable to the specified object. Returns Success.")]
	public class ModifyInt : Action
	{
		public enum ModifyType {Subtract, Add, Multiply, Divide, Set}
		[Tooltip("The value to set the SharedInt to")]
		public int modifyBy;
		[RequiredField]
		[Tooltip("The SharedInt to set")]
		public SharedInt targetVariable;
		[RequiredField]
		[Tooltip("What to modify this like.")]
		public ModifyType modifyType;

		public override TaskStatus OnUpdate()
		{
			switch (modifyType) {
				case ModifyType.Add:
					targetVariable.Value += modifyBy;
					break;
				case ModifyType.Divide:
					targetVariable.Value /= modifyBy;
					break;
				case ModifyType.Multiply:
					targetVariable.Value *= modifyBy;
					break;
				case ModifyType.Subtract:
					targetVariable.Value -= modifyBy;
					break;
				case ModifyType.Set:
					targetVariable.Value = modifyBy;
					break;
			}

			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetVariable = 0;
			modifyBy = 0;
			modifyType = ModifyType.Subtract;
		}
	}
}