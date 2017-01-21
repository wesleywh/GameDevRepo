using UnityEngine;
using System.Collections;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Entities.Aspects;
using RAIN.Entities;
using RAIN.Navigation.Graph;
using RAIN.Navigation.Pathfinding;
using RAIN.Motion;					//For MoveLookTarget
using RAIN.Serialization;
using RAIN.Perception.Sensors;

[RAINAction]
public class TakedownSetter : RAINAction {

	public Expression SetState = new Expression ();

	public override ActionResult Execute(RAIN.Core.AI ai)
	{
		ai.Body.transform.root.GetComponent<CombatController> ().takedownAble = (SetState.ExpressionAsEntered == "true" || SetState.ExpressionAsEntered == "True" || SetState.ExpressionAsEntered == "TRUE") ? true : false;
		return ActionResult.SUCCESS;
	}
}
