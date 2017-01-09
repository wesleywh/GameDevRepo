using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
public class FindCoverPosition : RAINAction
{
	public Expression StoreCoverVarInName = new Expression ();
	public Expression EnemyVariable = new Expression ();
	public Expression CoverEntityName = new Expression ();
	public Expression CoverDetector = new Expression();
	public Expression ForcePositionChange = new Expression();

	private VisualSensor CoverSensor = new VisualSensor ();
	private CoverSpot previousCoverSpot = null;
	private CoverSpot currentCoverPoint = null;
	private MoveLookTarget tTarget;								//current enemy target
	private Vector3 adjustment = new Vector3(0,2,0);

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		FindBestCoverPoint (ai);
        return ActionResult.SUCCESS;
    }

	private void FindBestCoverPoint(AI ai)
	{
		if (!StoreCoverVarInName.IsValid || !StoreCoverVarInName.IsVariable)
			return;

		//We don't actually have to have an enemy.  We can choose cover points that are simply near ourselves.
		Vector3 tEnemyPosition = ai.Kinematic.Position;

		//Assign a current enemy target
		if (EnemyVariable.IsValid && EnemyVariable.IsVariable)
		{
			//using a MoveLookTarget lets us automatically convert between gameObject, Vector3, etc., when getting
			//a position from AI memory
			tTarget = MoveLookTarget.GetTargetFromVariable(ai.WorkingMemory, EnemyVariable.VariableName, ai.Motor.CloseEnoughDistance);
			if (tTarget.IsValid) {
				tEnemyPosition = tTarget.Position;
			}
		}

		//Cover points should be marked with the "cover" entity
		for (int i=0; i<=ai.Senses.Sensors.Count; i++) {
			if (ai.Senses.Sensors [i].SensorName == CoverDetector.VariableName) {
				CoverSensor = ai.Senses.Sensors [i] as VisualSensor;
				break;
			}
		}
		CoverSensor.Sense(CoverEntityName.VariableName, RAINSensor.MatchType.ALL);

		Vector3 previousCoverAspect = ai.Kinematic.Position;;


		//Default cover point is our own position
		float bestCost = float.MaxValue;
		Vector3 tCoverPosition = ai.Kinematic.Position;

		//return the closest position out of all found positions
		//that the player cannot see.
		for (int i = 0; i < CoverSensor.Matches.Count; i++)
		{
			//if your current enemy can see the cover position skip it.
			//This makes the AI choose a more defensable position.
			Vector3 ray = (tTarget.Position + adjustment);
			Vector3 direction = (CoverSensor.Matches[i].Position - (tTarget.Position + adjustment));
			RaycastHit hit;
			if (Physics.Raycast (ray, direction, out hit, 100.0f)) {
				if (hit.collider.transform.gameObject.GetComponent<CoverSpot> ()) {
					if (hit.collider.transform.gameObject == CoverSensor.Matches [i].MountPoint.gameObject) {
						continue;
					}
				} 
			}

			//find all cover matches
			RAINAspect tCandidateAspect = CoverSensor.Matches[i];
			if (tCandidateAspect == null)
				continue;

			//Cover points are AIObjectives, which are objects that allow AI to "occupy" them
			CoverSpot tObjective = tCandidateAspect.Entity.Form.GetComponent<CoverSpot>();

			//Skip occupied cover points
			if((tObjective != null) && (tObjective.isTaken) && (tObjective.occupant == ai.Body)){
				previousCoverSpot = tObjective;
				previousCoverAspect = tCandidateAspect.Position;
				continue;
			}

			if ((tObjective == null) || (tObjective.isTaken && tObjective.occupant != ai.Body))
				continue;

			//Our cost function gives 50% more weight to points near the enemy
			//But then also adds in the AI distance to the point
			float tCost = 1.5f * Vector3.Distance(tEnemyPosition, tCandidateAspect.Position) + Vector3.Distance(ai.Kinematic.Position, tCandidateAspect.Position);
			if (tCost < bestCost)
			{
				currentCoverPoint = tObjective;
				tCoverPosition = tCandidateAspect.Position;
				bestCost = tCost;
			}
		}

		//If we found a cover point, then occupy it
		if (currentCoverPoint != null){
			if(previousCoverSpot != null)
				previousCoverSpot.LeaveSpot(ai.Body);
			currentCoverPoint.TakeSpot(ai.Body);
		}else if(previousCoverSpot != null){
			tCoverPosition = previousCoverAspect;
			previousCoverSpot.TakeSpot(ai.Body);
		}

		//Set the cover position in AI memory
		ai.WorkingMemory.SetItem<Vector3>(StoreCoverVarInName.VariableName, tCoverPosition);
	}
}