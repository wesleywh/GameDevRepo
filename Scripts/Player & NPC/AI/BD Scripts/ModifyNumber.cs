namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
    [TaskCategory("Basic/Math")]
    [TaskDescription("Sets an int value")]
    public class ModifyNumber : Action
    {
        [Tooltip("The int value to set")]
        public SharedFloat floatValue;
        [Tooltip("Modify that int value by this")]
        public SharedFloat modifyValue;
        [Tooltip("Reset this value to specified reset when intValue is above this number.")]
        public SharedFloat resetOn;
        [Tooltip("Reset this value to specified reset when intValue is above this number.")]
        public SharedFloat resetTo;
        [Tooltip("Reset this value to specified reset when intValue is above this number.")]
        public SharedFloat ReturnValue;

        public override TaskStatus OnUpdate()
        {
            floatValue.Value = floatValue.Value + modifyValue.Value;
            if (floatValue.Value > resetOn.Value)
            {
                floatValue.Value = resetTo.Value;
            }
            ReturnValue.Value = floatValue.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            ReturnValue.Value = 0;
        }
    }
}