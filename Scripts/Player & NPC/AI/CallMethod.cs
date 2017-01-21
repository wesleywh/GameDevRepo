using UnityEngine;
using System.Collections;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using System.Reflection;
using System;

[RAINAction]
public class CallMethod : RAINAction
{
	public CallMethod()
	{
		actionName = "CallMethod";
	}
	//The name of the component which has the method to call. This should be attached to the AI.Body gameObject. 
	public Expression TargetComponent;

	//The name of the method to call.     
	public Expression TargetMethod; 

	//cache the method signature to avoid expensive reflection calls.
	private MethodInfo _methodInfo;
	private object _component;

	//cache the target names so that we can test if they change
	private string _previousTargetComponent = "";
	private string _previousTargetMethod = "";

	public override void Start(AI ai)
	{
		//test if target has changed
		bool targetComponentChanged = string.IsNullOrEmpty(_previousTargetComponent)  ||
			_previousTargetComponent !=  TargetComponent.ExpressionAsEntered;
		bool targetMethodChanged = string.IsNullOrEmpty(_previousTargetMethod)  || 
			_previousTargetMethod !=  TargetMethod.ExpressionAsEntered;

		if (targetComponentChanged || targetMethodChanged || _component == null || _methodInfo == null)
		{
			//get target component type
			Type componentType = Type.GetType(TargetComponent.ExpressionAsEntered);

			if (componentType != null)
			{
				//get the target method signature
				_methodInfo = componentType.GetMethod(TargetMethod.ExpressionAsEntered);

				//get the target component
				_component = ai.Body.gameObject.GetComponent(TargetComponent.ExpressionAsEntered);
				
				//update target names
				_previousTargetMethod = TargetMethod.ExpressionAsEntered;
				_previousTargetComponent = TargetComponent.ExpressionAsEntered;
			}
		}
	}

	public override RAINAction.ActionResult Execute(AI ai)
	{
		if (_methodInfo == null || _component == null)
		{
			Debug.LogError("RAINAction.CallMethod has failed: Cannot find method to call.");

			return ActionResult.FAILURE;
		}

		//invoke the methods
		_methodInfo.Invoke(_component, null);

		return ActionResult.SUCCESS;
	}

//	public Expression numberOfShots = new Expression();
//	private int amount = 1;

//    public override ActionResult Execute(RAIN.Core.AI ai)
//    {
//		AttackWithWeapon (ai);
//        return ActionResult.SUCCESS;
//    }
//
//	private void AttackWithWeapon(RAIN.Core.AI ai) {
//		Debug.Log ("2");
//		if (!string.IsNullOrEmpty (numberOfShots.VariableName)) {
//			amount = int.Parse (numberOfShots.VariableName);
//		}
//		ai.Body.gameObject.GetComponent<GunManager>().ShootTarget(amount);
//		Debug.Log ("assigned");
//	}
}