using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Navigation;

[RAINAction("Choose Wander Position")]
public class ChooseWanderPosition : RAINAction
{
    /// <summary>
    /// Public Expressions are editable in the Behavior Editor
    /// WanderDistance is the max range to use when picking a wander target
    /// </summary>
    public Expression WanderDistance = new Expression();

    /// <summary>
    /// Public Expressions are editable in the Behavior Editor
    /// StayOnGraph is a boolean (true/false) that indicates whether the wander target must be on the nav graph
    /// </summary>
    public Expression StayOnGraph = new Expression();

    /// <summary>
    /// Public Expressions are editable in the Behavior Editor
    /// WanderTargetVariable is the name of the variable that the result will be assigned to
    /// *Don't use quotes when typing in the variable name
    /// </summary>
    public Expression WanderTargetVariable = new Expression();

    /// <summary>
    /// The default wander distance to use when the WanderDistance is invalid
    /// </summary>
    private float _defaultWanderDistance = 10f;

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        if (!WanderTargetVariable.IsVariable)
            throw new Exception("The Choose Wander Position node requires a valid Wander Target Variable");

        float tWanderDistance = 0f;
        if (WanderDistance.IsValid)
            tWanderDistance = WanderDistance.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory);

        if (tWanderDistance <= 0f)
            tWanderDistance = _defaultWanderDistance;

        Vector3 tDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
        tDirection *= tWanderDistance;

        Vector3 tDestination = ai.Kinematic.Position + tDirection;
        if (StayOnGraph.IsValid && (StayOnGraph.Evaluate<bool>(ai.DeltaTime, ai.WorkingMemory)))
        {
            if (NavigationManager.Instance.GraphForPoint(tDestination, ai.Motor.MaxHeightOffset, NavigationManager.GraphType.Navmesh, ((BasicNavigator)ai.Navigator).GraphTags).Count == 0)
                return ActionResult.FAILURE;
        }

        ai.WorkingMemory.SetItem<Vector3>(WanderTargetVariable.VariableName, tDestination);

        return ActionResult.SUCCESS;
    }
}